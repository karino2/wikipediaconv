using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using WikipediaConv;
using System.IO;

namespace WikipediaConvTest
{
    public class DirectoryInfoCacheForTest : DirectoryInfoCache
    {
        public struct MovedInfo
        {
            public string DestPath;
            public string NewName;
        }
        public DirectoryInfoCacheForTest(DirectoryInfoCacheForTest parent, DirectoryInfo di)
            : base(parent, di)
        {
            CreatedSubdirectories = new Dictionary<string, bool>();
            MovedFileInfos = new Dictionary<FileInfo, MovedInfo>();
        }

        public Dictionary<string, bool> CreatedSubdirectories { get; set; }
        public Dictionary<FileInfo, MovedInfo> MovedFileInfos { get; set; }

        public static DirectoryInfo ReturnSubdirectory = null;

        protected override DirectoryInfo RawCreateSubdirectory(string name)
        {
            CreatedSubdirectories[name] = true;
            if (ReturnSubdirectory != null)
                return ReturnSubdirectory;
            return new DirectoryInfo(name);
        }

        protected override void RawMoveTo(FileInfo target, string destPath, string newName)
        {
            MovedFileInfos[target] = new MovedInfo() { DestPath = destPath, NewName = newName };
        }
    }

    [TestFixture]
    public class DirectoryInfoCacheTest
    {
        [Test]
        public void Test_EnsureSubdirectory_AlreadyExist()
        {
            var child = new DirectoryInfo("C:\\root\\child");

            var root = CreateDICForTest(null, new DirectoryInfo("C:\\root"));
            root.Children.Add(CreateDICForTest(root, child));
            root.EnsureSubdirectory("child");

            Assert.AreEqual(0, root.CreatedSubdirectories.Keys.Count);
        }

        [Test]
        public void Test_EnsureSubdirectory_NotExist()
        {
            var child = new DirectoryInfo("C:\\root\\child");

            try
            {
                DirectoryInfoCacheForTest.ReturnSubdirectory = child;
                var root = CreateDICForTest(null, new DirectoryInfo("C:\\root"));
                root.EnsureSubdirectory("child");

                Assert.AreEqual(1, root.CreatedSubdirectories.Keys.Count);
            }
            finally
            {
                DirectoryInfoCacheForTest.ReturnSubdirectory = null;
            }
        }
        [Test]
        public void Test_MoveTo()
        {
            var dicFrom = CreateDICForTest(null, new DirectoryInfo("C:\\from"));
            var dicTo = CreateDICForTest(null, new DirectoryInfo("C:\\to"));
            var file = new FileInfo("C:\\from\\some.wiki");
            var newName = "newname.wiki";
            dicFrom.FileCount = 1;

            dicFrom.MoveTo(file, dicTo, newName);

            Assert.AreEqual(0, dicFrom.FileCount);
            Assert.AreEqual(1, dicTo.FileCount);
            Assert.AreEqual("C:\\to", dicFrom.MovedFileInfos[file].DestPath);
            Assert.AreEqual(newName, dicFrom.MovedFileInfos[file].NewName);
        }

        [Test]
        public void Test_CreateSubdirectory()
        {
            var child = new DirectoryInfo("test");

            try
            {
                DirectoryInfoCacheForTest.ReturnSubdirectory = child;
                var root = CreateDICForTest(null, new DirectoryInfo("C:\\dummy"));
                root.CreateSubdirectory("dummy");

                Assert.AreEqual(child, root.Children[0].Item);
                Assert.AreEqual(1, root.ChildrenLength);
                Assert.AreEqual(root, root.Children[0].Parent);
            }
            finally
            {
                DirectoryInfoCacheForTest.ReturnSubdirectory = null;
            }
        }

        public static  DirectoryInfoCacheForTest CreateDICForTest(DirectoryInfoCacheForTest parent, DirectoryInfo di)
        {
            return new DirectoryInfoCacheForTest(parent, di);
        }

        [Test]
        public void Test_Constructor()
        {
            var dic = new DirectoryInfoCache(null, new DirectoryInfo("C:\\dummy"));
            Assert.NotNull(dic);
        }
    }
}
