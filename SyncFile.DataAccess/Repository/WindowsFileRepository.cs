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
        /// <summary>
        /// basa path
        /// </summary>
        string _basepath = "";
        
        /// <summary>
        /// 設定檔檔名
        /// </summary>
        string _syncsetting = "_syncsetting.xml";

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

        #region SyncSetting

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

        #endregion

        #region File 

        public byte[] GetFile(string path)
        {
            return File.ReadAllBytes(_basepath + path);
        }

        public void CreateFile(string folder, string name, byte[] file)
        {
            using (var fs = new FileStream(_basepath + folder + "\\" + name, FileMode.Create, FileAccess.Write))
            {
                fs.Write(file, 0, file.Length);
            }
        }

        public void DeleteFile(string folder, string file)
        {
            if (File.Exists(folder + "\\" + file))
                File.Delete(folder + "\\" + file);
        }

        public void UpdateFile(string folder, string file, byte[] data)
        {
            using (var fs = new FileStream(folder + "\\" + file, FileMode.Create, FileAccess.Write))
            {
                fs.Write(data, 0, data.Length);
            }
        }

        #endregion

        #region Folder

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

        public List<SyncFolderInfo> GetFolders(bool withfile)
        {
            List<SyncFolderInfo> result = new List<SyncFolderInfo>();
            string[] folders = Directory.GetDirectories(_basepath);

            foreach (string folder in folders)
                result.Add(GetFolder(folder.Replace(_basepath, ""), withfile));

            return result;
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

        #endregion

        #region Private

        SyncFolderInfo GetFolder(string path, bool withfile)
        {
            DirectoryInfo info = new DirectoryInfo(_basepath + path);

            SyncFolderInfo result = new SyncFolderInfo()
            {
                Name = info.Name,
                Path = path,
                CreateDate = info.CreationTime,
                UpdateDate = info.LastWriteTime
            };

            string[] folders = Directory.GetDirectories(_basepath + path);

            foreach (string folder in folders)
                result.Folders.Add(GetFolder(folder.Replace(_basepath, ""), withfile));

            if (withfile)
                result.Files = GetFiles(path);

            return result;
        }

        #endregion 
    }
}
