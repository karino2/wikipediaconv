using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WikipediaConvTest
{
    using NUnit.Framework;
    using WikipediaConv;

    [TestFixture]
    public class SplitFolderTest
    {
        [Test]
        public void TestHello()
        {
            SplitFolder sf = new SplitFolder();
            sf.Hello();
        }
    }
}
