using System;
using System.Diagnostics;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SyncFile.DataAccess.Repository;
using SyncFile.Infrastructure.Utility;

namespace SyncFile.Test.Utility
{
    [TestClass]
    public class TrelloHelperTest
    {
        [TestMethod]
        public void IdToDatetimeTest()
        {
            DateTime dt = TrelloHelper.IdToDatetime("4d5ea62fd76aa1136000000c");
            Debug.WriteLine(dt.ToString());
        }        
    }
}
