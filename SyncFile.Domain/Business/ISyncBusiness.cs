using SyncFile.Domain.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace SyncFile.Domain.Business
{
    public interface ISyncBusiness
    {
        SyncResult Sync();
    }
}
