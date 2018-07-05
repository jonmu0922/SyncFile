using System;
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
                "462b6807022439549ae0f3a542bb62d5",
                "92dd852d5fb03f75b16aabb90234180ef226a990bbee6daacbdfa419ad958c92",
                "5b2e082f07e0a09b15a9e095"
                );

            _destination = new WindowsFileRepository(@"D:\CodeProject\SyncFile\temp1");

            ISyncBusiness isyncbusiness = new SyncBusiness(_source, _destination);

            isyncbusiness.Sync();
        }

        [TestMethod]
        public void WinToTrelloTest()
        {
            _source = new WindowsFileRepository(@"D:\CodeProject\SyncFile\temp2");

            _destination = new TrelloFileRepository(
                "462b6807022439549ae0f3a542bb62d5",
                "92dd852d5fb03f75b16aabb90234180ef226a990bbee6daacbdfa419ad958c92",
                "5b2e082f07e0a09b15a9e095"
                );

            ISyncBusiness isyncbusiness = new SyncBusiness(_source, _destination);

            isyncbusiness.Sync();
        }

    }
}
