using System;
using System.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sync.Business;
using SyncFile.DataAccess.Repository;
using SyncFile.Domain.Business;
using SyncFile.Domain.Interface.Repository;

namespace SyncFile.Test.Business
{
    [TestClass]
    public class SyncBusinessTest
    {
        IFileRepository _source, _destination;

        public SyncBusinessTest()
        {
            
        }

        public object SyncBusiness { get; private set; }

        [TestMethod]
        public void WinToWinTest()
        {
            _source = new WindowsFileRepository(@"D:\CodeProject\SyncFile\temp1");
            _destination = new WindowsFileRepository(@"D:\CodeProject\SyncFile\temp2");

            ISyncBusiness isyncbusiness = new SyncBusiness(_source, _destination);

            isyncbusiness.Sync();
        }

        [TestMethod]
        public void TrelloToWinTest()
        {
            _source = new TrelloFileRepository(
                ConfigurationManager.AppSettings["key"],
                ConfigurationManager.AppSettings["token"],
                ConfigurationManager.AppSettings["list"]
                );

            _destination = new WindowsFileRepository(@"D:\CodeProject\SyncFile\temp2");

            ISyncBusiness isyncbusiness = new SyncBusiness(_source, _destination);

            isyncbusiness.Sync();
        }

        [TestMethod]
        public void WinToTrelloTest()
        {
            _source = new WindowsFileRepository(@"D:\CodeProject\SyncFile\temp1");

            _destination = new TrelloFileRepository(
                ConfigurationManager.AppSettings["key"],
                ConfigurationManager.AppSettings["token"],
                ConfigurationManager.AppSettings["list"]
                );

            ISyncBusiness isyncbusiness = new SyncBusiness(_source, _destination);

            isyncbusiness.Sync();
        }

        [TestMethod]
        public void WinToTrelloToWinTest()
        {
            WinToTrelloTest();
            TrelloToWinTest();
        }
    }
}
