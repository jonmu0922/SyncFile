using System;
using System.Diagnostics;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SyncFile.DataAccess.Repository;

namespace SyncFile.Test.DataAccess
{
    [TestClass]
    public class WindowsFileRepositoryTest
    {
        WindowsFileRepository windowsfileRep = 
            new WindowsFileRepository(@"D:\CodeProject\SyncFile\temp1");

        [TestMethod]
        public void FolderTest()
        {
            string foldername = "testtest";
            windowsfileRep.CreateFolder(foldername);
            windowsfileRep.DeleteFolder(foldername);
        }

        [TestMethod]
        public void FileTest()
        {
            string filename = "test.txt";
            string foldername = "test";

            string text = "hello world " + DateTime.Today.ToString();

            windowsfileRep.CreateFolder(foldername);
            windowsfileRep.CreateFile(foldername, filename, Encoding.UTF8.GetBytes(text));

            windowsfileRep.UpdateFile(foldername, filename, Encoding.UTF8.GetBytes("update " + text));

            windowsfileRep.DeleteFile(foldername, filename);
        }

        [TestMethod]
        public void SaveSyncTest()
        {
            Debug.WriteLine(windowsfileRep.GetID());
            windowsfileRep.SaveSync("test");
        }

        [TestMethod]
        public void SaveSync2Test()
        {
            var date1 = windowsfileRep.GetLastRecord("test");

            if(date1 == null)
                Debug.WriteLine("null");
            else
                Debug.WriteLine(date1.Value.ToString());

            windowsfileRep.SaveSync("test");

            var date2 = windowsfileRep.GetLastRecord("test");

            if (date2 == null)
                Debug.WriteLine("null");
            else
                Debug.WriteLine(date2.Value.ToString());
        }

        [TestMethod]
        public void GetFilesTest()
        {
            var files = windowsfileRep.GetFiles("test");

            foreach (var f in files)
            {
                Debug.WriteLine(f.Name);
                Debug.WriteLine(f.Path);
                Debug.WriteLine(f.CreateDate.ToString());
                Debug.WriteLine(f.UpdateDate.ToString());
            }              
        }

        [TestMethod]
        public void GetFoldersTest()
        {
            var folder = windowsfileRep.GetFolders(false);

            foreach (var f in folder)
            {
                Debug.WriteLine(f.Name);
                Debug.WriteLine(f.Path);
                Debug.WriteLine(f.CreateDate.ToString());
                Debug.WriteLine(f.UpdateDate.ToString());
            }

            folder = windowsfileRep.GetFolders(true);

            foreach (var f in folder)
            {
                Debug.WriteLine(f.Name);
                Debug.WriteLine(f.Path);
                Debug.WriteLine(f.CreateDate.ToString());
                Debug.WriteLine(f.UpdateDate.ToString());
                Debug.WriteLine("-----------------");
                foreach (var f2 in f.Files)
                {
                    Debug.WriteLine(f2.Name);
                    Debug.WriteLine(f2.Path);
                    Debug.WriteLine(f2.CreateDate.ToString());
                    Debug.WriteLine(f2.UpdateDate.ToString());
                }
            }
        }
    }
}
