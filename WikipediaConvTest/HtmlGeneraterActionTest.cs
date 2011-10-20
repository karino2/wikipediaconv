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
            string actual = DumpAction.WikiNameToFileBaseName(input);
            Assert.AreEqual(expect, actual);
        }

        [Test]
        public void TestIsSkipCandidate_Not()
        {
            bool expected = false;
            bool actual = DumpAction.IsSkipCandidate("dummy", "here is normal content #hoge");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void TestIsSkipCandidate_Redirect_lower()
        {
            bool expected = true;
            bool actual = DumpAction.IsSkipCandidate("dummy", "#redirect hoge");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void TestIsSkipCandidate_Redirect_upper()
        {
            bool expected = true;
            bool actual = DumpAction.IsSkipCandidate("dummy", "#REDIRECT hoge");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void TestIsSkipCandidate_Redirect_Japanese()
        {
            bool expected = true;
            bool actual = DumpAction.IsSkipCandidate("dummy", "#転送 hoge");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void TestIsSkipCandidate_SakujoIrai()
        {
            bool expected = true;
            bool actual = DumpAction.IsSkipCandidate("Wikipedia:削除依頼/Ugg", "normal content");
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void TestIsSkipCandidate_NormalWikipedia()
        {
            bool expected = false;
            bool actual = DumpAction.IsSkipCandidate("Wikipedia:FAQ", "normal content");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void TestIsSkipCandidate_TemplateFile()
        {
            bool expected = true;
            bool actual = DumpAction.IsSkipCandidate("Template:010号線 (チェコ)", "normal content");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void TestIsSkipCandidate_BeginWithTemplateButNormal()
        {
            bool expected = false;
            bool actual = DumpAction.IsSkipCandidate("Template Methodパターン", "normal content");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void TestIsSkipCandidate_Category()
        {
            bool expected = true;
            bool actual = DumpAction.IsSkipCandidate("Category:007シリーズ (映画)のスタッフ.wiki", "normal content");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void TestIsSkipCandidate_BeginWithCategoryButNormal()
        {
            bool expected = false;
            bool actual = DumpAction.IsSkipCandidate("Category Theory", "normal content");
            Assert.AreEqual(expected, actual);
        }

        // BzipReader test. place here for a while
        [Test]
        public void TestGetYomi_nothing()
        {
            string expect = "わ";
            string actual = BzipReader.GetYomi("!dummy", "abcde", 1, 5);
            Assert.AreEqual(expect, actual);
        }

        [Test]
        public void TestGetYomi_WikiName()
        {
            string expect = "はじ";
            string actual = BzipReader.GetYomi("はじ目", "abcde", 1, 5);
            Assert.AreEqual(expect, actual);
        }

        [Test]
        public void TestGetYomi_DefaultSort_roman()
        {
            string expect = "def";
            string actual = CallGetYomi("!dummy", "abc{{DEFAULTSORT:def}}ghi");
            Assert.AreEqual(expect, actual);
        }

        [Test]
        public void TestGetYomi_DefaultSort_roman_multiline()
        {
            string expect = "def";
            string actual = CallGetYomi("!dummy", "a\nbc{{DEFAULTSORT:def}}ghi");
            Assert.AreEqual(expect, actual);
        }
        [Test]
        public void TestGetYomi_DefaultSort_katakana()
        {
            string expect = "def";
            string actual = CallGetYomi("!dummy", "abc{{デフォルトソート:def}}ghi");
            Assert.AreEqual(expect, actual);
        }
        [Test]
        public void TestGetYomi_DefaultSort_containColon()
        {
            string input = "abc{{DEFAULTSORT::かんたみようしん}}ghi";
            string expect = "かんたみようしん";
            VerifyGetYomi("!dummy", input, expect);
        }

        [Test]
        public void TestGetYomi_DefaultSort_containSlash()
        {
            string input = "abc{{DEFAULTSORT:JPCERT/CC}}ghi";
            string expect = "JPCERTCC";
            VerifyGetYomi("!dummy", input, expect);
        }

        private static void VerifyGetYomi(string wikiName, string input, string expect)
        {
            string actual = CallGetYomi(wikiName, input);
            Assert.AreEqual(expect, actual);
        }


        [Test]
        public void TestGetYomi_DefaultSort_afterEnd()
        {
            string expect = "わ";
            string actual = BzipReader.GetYomi("!dummy", "abc{{デフォルトソート:def}}ghi", 0, 2);
            Assert.AreEqual(expect, actual);
        }

        [Test]
        public void TestGetYomi_DefaultSort_Aster()
        {
            string expect = "わ";
            string input = "abc{{デフォルトソート:*}}ghi";
            VerifyGetYomi("!dummy", input, expect);
        }

        // call with default start end.
        static string CallGetYomi(string wikiName, string input)
        {
            return BzipReader.GetYomi(wikiName, input, 0, input.Length);
        }

        [Test]
        public void TestGetYomi_Yomi_normal()
        {
            string input = @"{{適当文字}}
'''平仮名''' （ひらがな）
abc{{DEFAULTSORT:def}}ghi";
            string expect = "ひらがな";
            string actual = BzipReader.GetYomi("!dummy",
                input, 0, input.Length);
            Assert.AreEqual(expect, actual);
        }

        [Test]
        public void TestGetYomi_Yomi_halfParen()
        {
            string input = @"'''冨樫'''（とがし）
*日本人の姓のひとつ。 
";
            string expect = "とがし";
            VerifyGetYomi("富樫", input, expect);
        }

        [Test]
        public void TestGetYomi_DefaultSearch_TooLong()
        {
            string input = @"{{適当文字}}
VeryLoooong default search
abc{{DEFAULTSORT:ていいひいえすけいとらまはなよめはやくとしつおりしなるさうんととらつくすいいとおふあんらつきいいやあふおおさふらいとふれせんてつとはいすえみつあんとさすえみす}}ghi";
            string expect = "ていいひいえすけいとらまはなよめはやく";
            string actual = CallGetYomi("!dummy", input);
            Assert.IsTrue(actual.StartsWith(expect));
            Assert.LessOrEqual(actual.Length, 50); // tekito-
        }
    }
}
