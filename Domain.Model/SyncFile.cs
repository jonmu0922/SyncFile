using System;
using System.Collections.Generic;

namespace Domain.Model
{
    public class SyncFile
    {
        public SyncFile()
        {
            Files = new List<SyncFile>();
        }

        public string Name { get; set; }
        public string Path { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }

        public List<SyncFile> Files { get; set; }
    }
}
