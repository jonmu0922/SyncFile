using System;
using System.Collections.Generic;
using System.Text;

namespace SysnFile.Domain.Model
{
    public class SyncFile
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
    } 
}
