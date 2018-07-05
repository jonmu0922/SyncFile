using SysnFile.Domain.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace SysnFile.Domain.Interface.Repository
{
    public interface IFileRepository
    {
        List<SyncFolder> GetFiles(string path);
        List<SyncFile> GetFile(string path);

        bool DeleteFolder(string path);
        bool CreateFolder(string path);        

        bool DeleteFile(string path);
        bool CreateFile(string path, Byte[] data);
        bool UpdateFile(string path);

    }
}
