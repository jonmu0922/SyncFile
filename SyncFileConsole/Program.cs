using Sync.Business;
using SyncFile.DataAccess.Repository;
using SyncFile.Domain.Interface.Business;
using SyncFile.Domain.Interface.Repository;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncFileConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            //http://blog.danskingdom.com/adding-and-accessing-custom-sections-in-your-c-app-config/

            /*
            var syncsource = ConfigurationManager.GetSection("SyncSource") as NameValueCollection;
            if (syncsource != null)
            {
                foreach (string key in syncsource)
                {
                    Console.WriteLine(syncsource[key].ToString());
                }
            }
            */

            ISyncBusiness syncbusiness = new SyncBusiness(
                FileRepositoryFactory((NameValueCollection)ConfigurationManager.GetSection("SyncSource")),
                FileRepositoryFactory((NameValueCollection)ConfigurationManager.GetSection("SyncDestination"))
            );

            var result = syncbusiness.Sync();

            Console.WriteLine("Folder:" + result.Folder);
            Console.WriteLine("File:" + result.File);

            Console.Read();
        }

        static IFileRepository FileRepositoryFactory(NameValueCollection nvc)
        {
            IFileRepository result = null;

            switch (nvc["Type"])
            {
                case "WindowsFileRepository":
                    result = new WindowsFileRepository(nvc["BasePath"]);
                    break;
                case "TrelloFileRepository":
                    result = new TrelloFileRepository(nvc["Key"], nvc["Token"], nvc["List"]);
                    break;
            }

            return result;
        }
    }
}
