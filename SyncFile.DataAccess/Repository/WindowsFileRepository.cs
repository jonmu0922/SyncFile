using SyncFile.Domain.Interface.Repository;
using SyncFile.Domain.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Linq;
using System.Linq;

namespace SyncFile.DataAccess.Repository
{
    public class WindowsFileRepository : AFileRepository, IFileRepository
    {
        string _basepath = "", 
               _syncsetting = "_syncsetting.xml";

        public WindowsFileRepository(string basepath)
        {
            _basepath = basepath;

            if (!_basepath.EndsWith("\\"))
                _basepath += "\\";

            // 取得sync設定檔
            if (File.Exists(_basepath + _syncsetting))
            {
                // 讀取根目錄的設定檔
                _xdoc = XDocument.Load(_basepath + _syncsetting);
            }                
            else
            {
                // 初始化設定檔
                InitSyncSetting();
            }
        }
        
        /// <summary>
        /// 取得sync repo 的id
        /// </summary>
        /// <returns></returns>
        public string GetID()
        {
            return GetSyncID();
        }

        /// <summary>
        /// 新增一筆同步紀錄，並儲存sync setting
        /// </summary>
        /// <param name="id"></param>
        public void SaveSync(string id)
        {
            AddSyncRecord(id);

            SaveSync();
        }

        /// <summary>
        /// 儲存sync setting
        /// </summary>
        /// <param name="id"></param>
        public void SaveSync()
        {            
            File.WriteAllText(_basepath + _syncsetting, _xdoc.ToString());
        }

        public DateTime? GetLastRecord(string id)
        {
            return GetLastSyncRecord(id);
        }

        public List<SyncFileInfo> GetFiles(string folder)
        {
            List<SyncFileInfo> result = new List<SyncFileInfo>();
            string[] files = Directory.GetFiles(_basepath + folder);

            foreach (string name in files)
            {
                FileInfo info = new FileInfo(name);

                result.Add(new SyncFileInfo()
                {
                    Name = info.Name,
                    CreateDate = info.CreationTime,
                    UpdateDate = info.LastWriteTime,
                    Path = name.Replace(_basepath, "")  // 把路徑換成相對位置
                });
            }

            return result;
        }

        public byte[] GetFile(string path)
        {
            return File.ReadAllBytes(_basepath + path);
        }

        public List<SyncFolderInfo> GetFolders(bool withfile)
        {
            List<SyncFolderInfo> result = new List<SyncFolderInfo>();
            string[] folders = Directory.GetDirectories(_basepath);

            foreach (string folder in folders)
            {
                DirectoryInfo info = new DirectoryInfo(folder);

                var syncfolderinfo = new SyncFolderInfo()
                {
                    Name = info.Name,
                    Path = folder,
                    CreateDate = info.CreationTime,
                    UpdateDate = info.LastWriteTime
                };

                if (withfile)
                    syncfolderinfo.Files = GetFiles(syncfolderinfo.Name);

                result.Add(syncfolderinfo);
            }

            return result;
        }

        public void CreateFile(string folder, string name, byte[] file)
        {
            using (var fs = new FileStream(_basepath + folder + "\\" + name, FileMode.Create, FileAccess.Write))
            {
                fs.Write(file, 0, file.Length);            
            }
        }

        public void CreateFolder(string folder)
        {
            if (!Directory.Exists(_basepath + folder))
                Directory.CreateDirectory(_basepath + folder);
        }

        public void DeleteFolder(string folder)
        {
            if (Directory.Exists(_basepath + folder))
                Directory.Delete(_basepath + folder);
        }

        public void DeleteFile(string folder, string file)
        {
            if (File.Exists(_basepath + folder + "\\" + file))
                File.Delete(_basepath + folder + "\\" + file);
        }              

        public void UpdateFile(string folder, string file, byte[] data)
        {
            using (var fs = new FileStream(_basepath + folder + "\\" + file, FileMode.Create, FileAccess.Write))
            {
                fs.Write(data, 0, data.Length);
            }
        }
    }
}
