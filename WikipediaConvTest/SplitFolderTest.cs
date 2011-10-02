using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WikipediaConvTest
{
    using NUnit.Framework;
    using WikipediaConv;
    using System.IO;
    
    [TestFixture]
    public class SplitFolderTest
    {

        [Test]
        public void TestGetFileNameHeadUntilCurrent_Root()
        {
            string expected = "";
            SplitFolder sf = new SplitFolder(new DirectoryInfo(@"I:\hoge"));
            string actual = sf.FileNameHeadUntilCurrent;
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void TestGetFileNameHeadUntilCurrent_TwoLevel()
        {
            string expected = "ika";
            SplitFolder sf = new SplitFolder(new DirectoryInfo(@"I:\hoge"), new DirectoryInfo(@"I:\hoge\i\k\a"));
            string actual = sf.FileNameHeadUntilCurrent;
            Assert.AreEqual(expected, actual);
        }


        [Test]
        public void TestGetMatchedSubdirectoryPath_Root()
        {
            string expected = @"I:\hoge\i";
            SplitFolder sf = new SplitFolder(new DirectoryInfo(@"I:\hoge"));
            string actual = sf.GetMatchedSubdirectoryPath(new FileInfo(@"I:hoge\ika.txt"));
            Assert.AreEqual(expected, actual);
        }
    }
}
