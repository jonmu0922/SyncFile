using System;
using System.Collections.Generic;
using System.Text;

namespace SyncFile.Domain.Model
{
    public class SyncFileInfo
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public int Size { get; set; }
    }
}
