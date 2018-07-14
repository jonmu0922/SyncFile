using Newtonsoft.Json.Linq;
using SyncFile.Domain.Interface.Repository;
using SyncFile.Domain.Model;
using SyncFile.Infrastructure.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace SyncFile.DataAccess.Repository
{
    public class TrelloFileRepository : AFileRepository, IFileRepository
    {
        string _key = "", _token = "", _list = "", 
               _rootname = "_root", // root card name 
               _rootcard = "";      // root card id

        JArray cardarray = new JArray();

        public TrelloFileRepository(string key, string token, string list)
        {
            _key = key;
            _token = token;
            _list = list;

            RefreshCardArray();
            JObject obj = cardarray.Children<JObject>()
                            .FirstOrDefault(o => o["name"].ToString() == _rootname);

            if (obj == null)
                InitSyncSetting();  //無 _root 卡片
            else
            {
                // _rootname 卡片的 desc 儲存 syncsetting
                JObject root = JObject.Parse(TrelloHelper.GetCard(_key, _token, obj["id"].Value<string>()));

                // 把 card 的 description 轉成 xml document
                _xdoc = XDocument.Parse(root["desc"].Value<string>());

                // 紀錄 root card id
                _rootcard = obj["id"].Value<string>();
            }
        }

        public string GetID()
        {
            return GetSyncID();
        }

        public void CreateFile(string folder, string name, byte[] file)
        {
            JObject obj = cardarray.Children<JObject>()
                            .FirstOrDefault(o => o["name"].ToString() == folder);

            if (obj == null)
                throw new Exception("資料夾不存在");

            JArray list = JArray.Parse(
                   TrelloHelper.GetCardAttachmentList(_key, _token, obj["id"].Value<string>())); // file list

            JObject attachment = list.Children<JObject>()
                            .FirstOrDefault(o => o["name"].ToString() == name);

            if (attachment != null)
                DeleteFile(folder, name);   //已存在，刪除檔案

            TrelloHelper.CreateAttachment(_key, _token, obj["id"].Value<string>(), name, file);
        }

        public void DeleteFile(string folder, string file)
        {
            JObject card = cardarray.Children<JObject>()
                            .FirstOrDefault(o => o["name"].ToString() == folder);

            if (card == null)
                throw new Exception("資料夾不存在");

            // fetch file list
            JArray list = JArray.Parse(
                    TrelloHelper.GetCardAttachmentList(_key, _token, card["id"].Value<string>())); 

            JObject attachment = list.Children<JObject>()
                            .FirstOrDefault(o => o["name"].ToString() == file);

            if (attachment == null)
                throw new Exception("檔案不存在");

            TrelloHelper.DeleteAttachment(_key, _token, card["id"].Value<string>(), attachment["id"].Value<string>());
        }

        public void UpdateFile(string folder, string file, byte[] data)
        {
            JObject card = cardarray.Children<JObject>()
                            .FirstOrDefault(o => o["name"].ToString() == folder);

            if (card == null)
                throw new Exception("資料夾不存在");

            JArray list = JArray.Parse(
                    TrelloHelper.GetCardAttachmentList(_key, _token, card["id"].Value<string>())); // file list

            JObject attachment = list.Children<JObject>()
                            .FirstOrDefault(o => o["name"].ToString() == file);

            // 已存在，刪除檔案
            if (attachment != null)
                TrelloHelper.DeleteAttachment(_key, _token, 
                    card["id"].Value<string>(), attachment["id"].Value<string>());

            TrelloHelper.CreateAttachment(_key, _token, card["id"].Value<string>(), file, data);
        }

        public void CreateFolder(string folder)
        {
            JObject obj = cardarray.Children<JObject>()
                            .FirstOrDefault(o => o["name"].ToString() == folder);

            // 不存在才新增
            if (obj == null)
            {
                TrelloHelper.CreateCard(_key, _token, _list, folder);
                RefreshCardArray(); // 重新取 card array
            }                
        }

        public void DeleteFolder(string folder)
        {
            JObject obj = cardarray.Children<JObject>()
                            .FirstOrDefault(o => o["name"].ToString() == folder);

            if (obj == null)
                throw new Exception("資料夾不存在");

            TrelloHelper.DeleteCard(_key, _token, obj["id"].Value<string>());
            RefreshCardArray(); // 重新取 card array
        }

        public List<SyncFileInfo> GetFiles(string folder)
        {
            List<SyncFileInfo> result = new List<SyncFileInfo>();
           
            JObject obj = cardarray.Children<JObject>()
                            .FirstOrDefault(o => o["name"].ToString() == folder);

            if (obj != null)
            {
                JArray list = JArray.Parse(
                    TrelloHelper.GetCardAttachmentList(_key, _token, obj["id"].Value<string>()));

                foreach (JObject o in list.Children<JObject>())
                {
                    result.Add(new SyncFileInfo()
                        {
                            Name = o["name"].Value<string>(),
                            Path = o["url"].Value<string>(),
                            CreateDate = TrelloHelper.IdToDatetime(o["id"].Value<string>()),
                            UpdateDate = TrelloHelper.IdToDatetime(o["id"].Value<string>()),
                            Size = o["bytes"].Value<int>()
                        }
                    );
                }
            }

            return result;            
        }

        public List<SyncFolderInfo> GetFolders(bool withfile)
        {            
            //cards in list            
            List<SyncFolderInfo> folders = new List<SyncFolderInfo>();        
            
            foreach (JObject o in cardarray.Children<JObject>())
            {
                // 排除 root card
                if (o["name"].Value<string>() == _rootname)
                    continue;

                folders.Add(new SyncFolderInfo()
                    {
                        Name = o["name"].Value<string>(),
                        Path  = o["url"].Value<string>(),
                        CreateDate = TrelloHelper.IdToDatetime(o["id"].Value<string>()),
                        UpdateDate = TrelloHelper.IdToDatetime(o["id"].Value<string>()),
                    }
                );
            }

            if (withfile)
            {
                foreach (var folder in folders)
                    folder.Files = GetFiles(folder.Name);
            }

            List<SyncFolderInfo> result = new List<SyncFolderInfo>();

            foreach (var f in folders)
            {
                // 先找出第一層，名字沒有 \ 的
                if (f.Name.Split('\\').Count() == 1)
                {
                    result.Add(GetFolder(f.Name, withfile, folders));
                }
            }

            return result;
        }
               
        /// <summary>
        /// 取得附件 byte[]
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public byte[] GetFile(string path)
        {
            return TrelloHelper.GetAttachment(path);
        }

        public void SaveSync(string id)
        {
            AddSyncRecord(id);

            SaveSync();
        }

        public void SaveSync()
        {
            // 刪除 root card
            if (!string.IsNullOrEmpty(_rootcard))
                TrelloHelper.DeleteCard(_key, _token, _rootcard);

            // 把sync xml record 寫到 root card 的 description
            TrelloHelper.CreateCard(_key, _token, _list, _rootname, _xdoc.ToString());
            RefreshCardArray(); // 重新取 card array
        }

        public DateTime? GetLastRecord(string id)
        {
            return GetLastSyncRecord(id);
        }

        #region Private       

        SyncFolderInfo GetFolder(string path, bool withfile, List<SyncFolderInfo> folders)
        {
            var root = folders.Where(o => o.Name == path).First();
            var namearr = root.Name.Split('\\');

            SyncFolderInfo result = new SyncFolderInfo()
            {
                Name = namearr[namearr.Length - 1], //陣列最後一個是資料夾名稱
                Path = path,                        
                CreateDate = root.CreateDate,
                UpdateDate = root.UpdateDate,
                Files = root.Files
            };

            foreach (var folder in folders)
            {
                // 找出這個目錄下的子目錄
                if (folder.Name != path &&  // 排除不是自己
                    folder.Name.Split('\\').Count() > 1 && // 排除不是第一層目錄
                    folder.Name.Replace(path + "\\", "").Split('\\').Count() == 1) //只要這個目錄下的第一層子目錄
                {
                    // 往下找
                    result.Folders.Add(GetFolder(folder.Name, withfile, folders));
                }
            }

            return result;
        }

        /// <summary>
        /// 更新 trello card list
        /// </summary>
        void RefreshCardArray()
        {
            cardarray = JArray.Parse(TrelloHelper.GetCardList(_key, _token, _list));
        }

        #endregion
    }
}
