using SyncFile.Domain.Interface.Business;
using SyncFile.Domain.Interface.Repository;
using SyncFile.Domain.Model;
using System;

namespace Sync.Business
{
    public class SyncBusiness : ISyncBusiness
    {
        IFileRepository _source, _destination;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source">來源</param>
        /// <param name="destination">目標</param>
        public SyncBusiness(IFileRepository source, IFileRepository destination)
        {
            _source = source;
            _destination = destination;
        }

        /// <summary>
        /// 同步檔案
        /// </summary>
        /// <returns></returns>
        public SyncResult Sync()
        {
            SyncResult result = new SyncResult();

            // 取得上次同步時間
            DateTime? sourcelastrecord = _source.GetLastRecord(_destination.GetID());

            // 先做單向
            // 取得來源資料夾、檔案
            var folder = _source.GetFolders(true);

            foreach (var f in folder)
            {
                CreateFolderAndFile(f, sourcelastrecord);                
            }

            // 紀錄sync時間
            _source.SaveSync(_destination.GetID());
            _destination.SaveSync();

            return result;
        }

        /// <summary>
        /// 建立資料夾、檔案
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="sourcelastrecord"></param>
        void CreateFolderAndFile(SyncFolderInfo folder, DateTime? sourcelastrecord)
        {
            _destination.CreateFolder(folder.Path); // 新增資料夾

            foreach (var file in _source.GetFiles(folder.Path))
            {
                // 檢查上次sync時間是否為null或
                // 檔案是否在上次sync後有修改
                if (!sourcelastrecord.HasValue ||
                    file.UpdateDate > sourcelastrecord.Value)
                {
                    // 新增檔案
                    _destination.CreateFile(
                        folder.Path,
                        file.Name,
                        _source.GetFile(file.Path)
                    );                    
                }
            }

            folder.Folders.ForEach(o =>
            {
                CreateFolderAndFile(o, sourcelastrecord);
            });
        }        
    }
}
