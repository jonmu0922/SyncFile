using SyncFile.Domain.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace SyncFile.Domain.Interface.Repository
{
    public interface IFileRepository
    {
        #region SyncSetting

        /// <summary>
        /// 取得sync repo 的id
        /// </summary>
        /// <returns></returns>
        string GetID();

        /// <summary>
        /// 新增一筆同步紀錄，並儲存sync setting
        /// </summary>
        /// <param name="id"></param>
        void SaveSync(string id);

        /// <summary>
        /// 儲存sync setting
        /// </summary>
        void SaveSync();

        /// <summary>
        /// 取得指定 sync repo 最後一次同步時間
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        DateTime? GetLastRecord(string id);

        #endregion

        #region File
        
        List<SyncFileInfo> GetFiles(string folder);
        void DeleteFile(string folder, string file);
        void CreateFile(string folder, string name, byte[] data);
        void UpdateFile(string folder, string file, byte[] data);
        byte[] GetFile(string path);

        #endregion

        #region Folder

        List<SyncFolderInfo> GetFolders(bool withfile);
        void DeleteFolder(string folder);
        void CreateFolder(string folder);

        #endregion
    }
}
