using System;
using System.Collections.Generic;
using System.Text;

namespace SysnFile.Domain.Model
{
    public class SyncFolder
    {
        public SyncFolder()
        {
            Files = new List<SyncFile>();
            Folders = new List<SyncFolder>();
        }

        public string Name { get; set; }
        public string Path { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public List<SyncFile> Files { get; set; }
        public List<SyncFolder> Folders { get; set; }
    }
}
