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
        public void TestIsSkipCandidate_Redirect_mixed()
        {
            bool expected = true;
            bool actual = DumpAction.IsSkipCandidate("dummy", "#Redirect hoge");
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
            string actual = BzipReader.GetYomi("!dummy", "abcde");
            Assert.AreEqual(expect, actual);
        }

        [Test]
        public void TestGetYomi_WikiName()
        {
            string expect = "はじ";
            string actual = BzipReader.GetYomi("はじ目", "abcde");
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
        public void TestGetYomi_DefaultSort_Aster()
        {
            string expect = "わ";
            string input = "abc{{デフォルトソート:*}}ghi";
            VerifyGetYomi("!dummy", input, expect);
        }

        // obsolete wrapper.
        static string CallGetYomi(string wikiName, string input)
        {
            return BzipReader.GetYomi(wikiName, input);
        }

        [Test]
        public void TestGetYomi_Yomi_normal()
        {
            string input = @"{{適当文字}}
'''平仮名''' （ひらがな）
abc{{DEFAULTSORT:def}}ghi";
            string expect = "ひらがな";
            string actual = BzipReader.GetYomi("!dummy", input);
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
        public void TestGetYomi_Yomi_quote()
        {
            string input = @"'''両津市'''（'''りょうつし'''）は、[[1954年]]から
";
            string expect = "りょうつし";
            VerifyGetYomi("両津市", input, expect);
        }

        [Test]
        public void TestGetYomi_Yomi_notpure()
        {
            string input = @"『'''奇譚クラブ'''』（きたんくらぶ）
";
            string expect = "きたんくらぶ";
            VerifyGetYomi("奇譚クラブ", input, expect);
        }

        [Test]
        public void TestGetYomi_Yomi_betsuyomi()
        {
            string input = @"'''蒲焼'''（蒲焼き、かばやき）は、[[魚]]を
";
            string expect = "かばやき";
            VerifyGetYomi("蒲焼", input, expect);
        }

        #region sometime fail, but test success, why?
        [Test]
        public void TestGetYomi_Yomi_whyfail()
        {
            string input = @"'''濁音''' (だくおん) とは、
";
            string expect = "だくおん";
            VerifyGetYomi("濁音", input, expect);
        }

        [Test]
        public void TestGetYomi_Yomi_whyfail2()
        {
            string input = @"'''家庭教育''' (かていきょういく) とは、";
            string expect = "かていきょういく";
            VerifyGetYomi("家庭教育", input, expect);
        }
        #endregion

        [Test]
        public void TestGetYomi_Yomi_hankakuComma()
        {
            string input = @"'''自由電子近似''' (じゆうでんしきんじ, <span lang=""en"" xml:lang=""en"">Free electron approximation</span>)";
            string expect = "じゆうでんしきんじ";
            VerifyGetYomi("自由電子近似", input, expect);
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
        [Test]
        public void TestGetYomi_Wikiname_colon()
        {
            string input = @"dummy content";
            string expect = "WikipediaFAQ";
            string actual = BzipReader.GetYomi("Wikipedia:FAQ", input);
            Assert.AreEqual(expect, actual);
        }
    }
}
