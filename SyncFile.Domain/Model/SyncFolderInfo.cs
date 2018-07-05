using System;
using System.Collections.Generic;
using System.Text;

namespace SyncFile.Domain.Model
{
    public class SyncFolderInfo
    {
        public SyncFolderInfo()
        {
            Files = new List<SyncFileInfo>();           
        }

        public string Name { get; set; }
        public string Path { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public List<SyncFileInfo> Files { get; set; }       
    }
}
