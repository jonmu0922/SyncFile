using System;
using System.Diagnostics;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SyncFile.DataAccess.Repository;
using System.Threading;
using System.Configuration;
using SyncFile.Domain.Model;
using System.Collections.Generic;

namespace SyncFile.Test.DataAccess
{
    [TestClass]
    public class TrelloFileRepositoryTest
    {
        TrelloFileRepository trellofileRep = 
            new TrelloFileRepository(
                ConfigurationManager.AppSettings["key"],
                ConfigurationManager.AppSettings["token"],
                ConfigurationManager.AppSettings["list"]
            );

        //[TestMethod]
        //public void GetFolderTest()
        //{
        //    var folder = trellofileRep.GetFolders(false);

        //    foreach (var f in folder)
        //    {
        //        Debug.WriteLine(f.Name);
        //        Debug.WriteLine(f.Path);
        //        //Debug.WriteLine(f.CreateDate.ToString());
        //        //Debug.WriteLine(f.UpdateDate.ToString());
        //    }

        //    folder = trellofileRep.GetFolders(true);

        //    foreach (var f in folder)
        //    {
        //        Debug.WriteLine(f.Name);
        //        Debug.WriteLine(f.Path);
        //        //Debug.WriteLine(f.CreateDate.ToString());
        //        //Debug.WriteLine(f.UpdateDate.ToString());
        //        Debug.WriteLine("-----------------");
        //        foreach (var f2 in f.Files)
        //        {
        //            Debug.WriteLine(f2.Name);
        //            Debug.WriteLine(f2.Path);
        //            //Debug.WriteLine(f2.CreateDate.ToString());
        //            //Debug.WriteLine(f2.UpdateDate.ToString());
        //        }
        //    }
        //}

        [TestMethod]
        public void GetFoldersTest()
        {
            var folder = trellofileRep.GetFolders(true);

            DebugWriteFolder(folder);
        }

        void DebugWriteFolder(List<SyncFolderInfo> folder)
        {
            foreach (var f in folder)
            {
                Debug.WriteLine(f.Name);
                Debug.WriteLine(f.Path);
                //Debug.WriteLine(f.CreateDate.ToString());
                //Debug.WriteLine(f.UpdateDate.ToString());

                foreach (var f2 in f.Files)
                {
                    //Debug.WriteLine(f2.Name);
                    //Debug.WriteLine(f2.Path);
                    //Debug.WriteLine(f2.CreateDate.ToString());
                    //Debug.WriteLine(f2.UpdateDate.ToString());
                }

                DebugWriteFolder(f.Folders);
            }
        }

        [TestMethod]
        public void GetFileTest()
        {
            var file = trellofileRep.GetFiles("Test");

            foreach (var f in file)
            {
                Debug.WriteLine(f.Name);
                Debug.WriteLine(f.Path);
                Debug.WriteLine(f.Size);
                Debug.WriteLine(f.CreateDate.ToString());
                Debug.WriteLine(f.UpdateDate.ToString());
            }
        }

        [TestMethod]
        public void CreateFolderTest()
        {
            var folder = DateTime.Now.ToString("yyyyMMddHHmmss");
            
            trellofileRep.CreateFolder(folder);

            trellofileRep.DeleteFolder(folder);
        }

        [TestMethod]
        public void CreateFileTest()
        {
            string filename = "test.txt";
            string foldername = "Test";

            string text = "hello world " + DateTime.Now.ToString();

            trellofileRep.CreateFile(foldername, filename, Encoding.UTF8.GetBytes(text));
        }

        [TestMethod]
        public void DeleteFileTest()
        {
            string filename = "test.txt";
            string foldername = "Test";

            trellofileRep.DeleteFile(foldername, filename);
        }

        [TestMethod]
        public void UpdateFileTest()
        {
            string filename = "test.txt";
            string foldername = "Test";

            string text = "hello world 123" + DateTime.Now.ToString();

            trellofileRep.CreateFile(foldername, filename, Encoding.UTF8.GetBytes(text));

            System.Threading.Thread.Sleep(3000);

            text = "hello world ";

            trellofileRep.UpdateFile(foldername, filename, Encoding.UTF8.GetBytes(text));
        }
    }
}
