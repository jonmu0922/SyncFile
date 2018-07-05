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
               _rootname = "_root", 
               _rootcard = "";

        public TrelloFileRepository(string key, string token, string list)
        {
            _key = key;
            _token = token;
            _list = list;
            
            JArray array = JArray.Parse(TrelloHelper.GetCardList(_key, _token, _list));
            JObject obj = array.Children<JObject>()
                            .FirstOrDefault(o => o["name"].ToString() == _rootname);

            if (obj == null)
                InitSyncSetting();  //無 _rootname 卡片
            else
            {
                // _rootname 卡片的 desc 儲存 syncsetting
                JObject root = JObject.Parse(TrelloHelper.GetCard(_key, _token, obj["id"].Value<string>()));
                _xdoc = XDocument.Parse(root["desc"].Value<string>());
                _rootcard = obj["id"].Value<string>();
            }
        }

        public string GetID()
        {
            return GetSyncID();
        }

        public void CreateFile(string folder, string name, byte[] file)
        {
            JArray array = JArray.Parse(TrelloHelper.GetCardList(_key, _token, _list));
            JObject obj = array.Children<JObject>()
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
            JArray array = JArray.Parse(TrelloHelper.GetCardList(_key, _token, _list)); // folder list
            JObject card = array.Children<JObject>()
                            .FirstOrDefault(o => o["name"].ToString() == folder);

            if (card == null)
                throw new Exception("資料夾不存在");

            JArray list = JArray.Parse(
                    TrelloHelper.GetCardAttachmentList(_key, _token, card["id"].Value<string>())); // file list

            JObject attachment = list.Children<JObject>()
                            .FirstOrDefault(o => o["name"].ToString() == file);

            if (attachment == null)
                throw new Exception("檔案不存在");

            TrelloHelper.DeleteAttachment(_key, _token, card["id"].Value<string>(), attachment["id"].Value<string>());
        }

        public void UpdateFile(string folder, string file, byte[] data)
        {
            JArray array = JArray.Parse(TrelloHelper.GetCardList(_key, _token, _list)); // folder list
            JObject card = array.Children<JObject>()
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
            JArray array = JArray.Parse(TrelloHelper.GetCardList(_key, _token, _list));
            JObject obj = array.Children<JObject>()
                            .FirstOrDefault(o => o["name"].ToString() == folder);

            // 不存在才新增
            if (obj == null)                
                TrelloHelper.CreateCard(_key, _token, _list, folder);
        }

        public void DeleteFolder(string folder)
        {
            JArray array = JArray.Parse(TrelloHelper.GetCardList(_key, _token, _list));
            JObject obj = array.Children<JObject>()
                            .FirstOrDefault(o => o["name"].ToString() == folder);

            if (obj == null)
                throw new Exception("資料夾不存在");

            TrelloHelper.DeleteCard(_key, _token, obj["id"].Value<string>());
        }

        public List<SyncFileInfo> GetFiles(string folder)
        {
            List<SyncFileInfo> result = new List<SyncFileInfo>();

            JArray array = JArray.Parse(TrelloHelper.GetCardList(_key, _token, _list));
            JObject obj = array.Children<JObject>()
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
            List<SyncFolderInfo> result = new List<SyncFolderInfo>();            

            JArray array = JArray.Parse(TrelloHelper.GetCardList(_key, _token, _list));

            foreach (JObject o in array.Children<JObject>())
            {
                // root 那張card要跳過
                if (o["name"].Value<string>() == _rootname)
                    continue;

                result.Add(new SyncFolderInfo()
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
                foreach (var folder in result)
                    folder.Files = GetFiles(folder.Name);
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
            if (!string.IsNullOrEmpty(_rootcard))
                TrelloHelper.DeleteCard(_key, _token, _rootcard);

            TrelloHelper.CreateCard(_key, _token, _list, _rootname, _xdoc.ToString());
        }

        public DateTime? GetLastRecord(string id)
        {
            return GetLastSyncRecord(id);
        }
    }
}
