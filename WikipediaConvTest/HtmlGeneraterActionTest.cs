using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using WikipediaConv;

namespace WikipediaConvTest
{
    [TestFixture]
    public class HtmlGeneraterActionTest
    {
        [Test]
        public void TestWikiNameToFileBaseName_Slash()
        {
            string expect = "hoge／ika";
            string input = "hoge/ika";

            VerifyWikinameToFileBaseName(expect, input);
        }
        [Test]
        public void TestWikiNameToFileBaseName_Gt()
        {
            VerifyWikinameToFileBaseName("hoge＞ika", "hoge>ika");
        }

        [Test]
        public void TestWikiNameToFileBaseName_Lt()
        {
            VerifyWikinameToFileBaseName("hoge＜ika", "hoge<ika");
        }

        [Test]
        public void TestWikiNameToFileBaseName_Question()
        {
            VerifyWikinameToFileBaseName("Ain't I a Woman？ (book)", "Ain't I a Woman? (book)");
        }
        [Test]
        public void TestWikiNameToFileBaseName_Colon()
        {
            string expect = "Wikipedia：Adding Wikipedia articles to Nupedia";
            string input = "Wikipedia:Adding Wikipedia articles to Nupedia";

            VerifyWikinameToFileBaseName(expect, input);
        }
        [Test]
        public void TestWikiNameToFileBaseName_DblQuote()
        {
            VerifyWikinameToFileBaseName("hoge”ika", "hoge\"ika");
        }
        [Test]
        public void TestWikiNameToFileBaseName_BackSlash()
        {
            VerifyWikinameToFileBaseName("hoge￥ika", "hoge\\ika");
        }

        [Test]
        public void TestWikiNameToFileBaseName_Aster()
        {
            VerifyWikinameToFileBaseName("B＊-algebra", "B*-algebra");
        }

        [Test]
        public void TestWikiNameToFileBaseName_Pipe()
        {
            VerifyWikinameToFileBaseName("hoge｜ika", "hoge|ika");
        }

        [Test]
        public void TestWikiNameToFileBaseName_Semicolon()
        {
            VerifyWikinameToFileBaseName("hoge；ika", "hoge;ika");
        }

        private static void VerifyWikinameToFileBaseName(string expect, string input)
        {
            string actual = HtmlGeneraterAction.WikiNameToFileBaseName(input);
            Assert.AreEqual(expect, actual);
        }
    }
}
