using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WikipediaConvTest
{
    using NUnit.Framework;
    using WikipediaConv;
    using System.IO;
    using EPubLib;
    
    [TestFixture]
    public class SplitFolderTest
    {
        // [Test]
        public void TestHelloEPub()
        {
            Book book = new Book();
            //You can also use the factory, FileItem.Create(FileItemType.XHTML).
            //For images, you must use the factory, FileItem.Create(FileItemType.JPEG), etc
            Chapter chapter1 = new Chapter()
            {
                Title = "Chapter 1!",

                //You are responsible for setting chapter numbers appropriately, starting with 1.
                Number = 1,

                //Be sure your Content is XHTML 1.1 compliant!
                Content = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.1//EN"" ""http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd"">
<html><head><title>Chapter 1</title></head>
<body><h1>Episode 1</h1><h2>The Phantom Menace</h2>
<p>Well look here, it's content in an epub!</p></body></html>"
            };

            //Book.FileItems holds all of the files in the package: chapters, images, CSS, etc. 
            //They all inherit from the abstract type FileItem and so it's easily extensible. 
            book.FileItems.Add(chapter1);

            //Repeat as necessary for each chapter
            //And then save to a file.
            //EPubLib will create a table of contents with an entry for each chapter,
            //and then package/zip everything into the proper format. 
            book.Save(@"output.epub");
        }

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
