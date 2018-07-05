using System;
using System.Collections.Generic;
using System.Text;

namespace SyncFile.Domain.Model
{
    public class SyncResult
    {
        /// <summary>
        /// 影響資料夾數
        /// </summary>
        public int Folder { get; set; }

        /// <summary>
        /// 影響檔案數
        /// </summary>
        public int File { get; set; }
    }
}
