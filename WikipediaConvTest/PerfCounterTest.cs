using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using WikipediaConv;
using System.Threading;

namespace WikipediaConvTest
{
    [TestFixture]
    public class PerfCounterTest
    {
        [Test]
        public void TestStartStop()
        {
            var pc = new PerfCounter();
            var rec = new DateTimeRecoder (new DateTime(1000), new DateTime(2000),
                new DateTime(3000), new DateTime(4000));
            pc.Now = rec.Now;
            pc.Start("testPC");
            pc.Start("testPC2");
            pc.Stop("testPC2");
            pc.Stop("testPC");

            Assert.AreEqual(0.3, pc.GetStopWatch("testPC").Average, 0.05);
            Assert.AreEqual(0.1, pc.GetStopWatch("testPC2").Average, 0.05);

        }
    }

    public class DateTimeRecoder
    {
        List<DateTime> _dates = new List<DateTime>();
        int _pos = 0;
        public DateTimeRecoder(params DateTime[] dts)
        {
            Array.ForEach(dts, (dt) => Add(dt));
        }
        public DateTimeRecoder()
        {
        }
        public DateTime Now()
        {
            return _dates[_pos++];
        }
        public void Add(DateTime dt)
        {
            _dates.Add(dt);
        }
    }

    [TestFixture]
    public class StopWatchTest
    {
        [Test]
        public void TestName()
        {
            string expected = "hoge";
            var sw = new StopWatch(expected);
            Assert.AreEqual(expected, sw.Name);
        }

        [Test]
        public void TestDateTimeRecoder()
        {
            DateTime beg = new DateTime(1000);
            DateTime end = new DateTime(2000);
            var rec = new DateTimeRecoder();
            rec.Add(beg);
            rec.Add(end);
            Assert.AreEqual(beg, rec.Now());
            Assert.AreEqual(end, rec.Now());
        }

        StopWatch Create(params DateTime[] dts)
        {
            var rec = new DateTimeRecoder(dts);
            var sw = new StopWatch("test");
            sw.Now = rec.Now;
            return sw;
        }

        [Test]
        public void Test_StartEnd_One()
        {
            TimeSpan elapse = new TimeSpan(1000);
            DateTime beg = new DateTime(1000);
            DateTime end = new DateTime(2000);
            var sw = Create(beg, end);
            sw.Start();
            sw.Stop();
            Assert.AreEqual(elapse, sw.Max);
            Assert.AreEqual(elapse, sw.Min);
            Assert.AreEqual(1, sw.Count);
            Assert.AreEqual(0.1, sw.Average);

        }

        [Test]
        public void Test_StartEnd_Two()
        {
            TimeSpan elapse1 = new TimeSpan(1000);
            TimeSpan elapse2 = new TimeSpan(2000);

            DateTime beg1 = new DateTime(1000);
            DateTime end1 = new DateTime(2000);

            DateTime beg2 = new DateTime(3000);
            DateTime end2 = new DateTime(5000);

            var sw = Create(beg1, end1, beg2, end2);
            sw.Start();
            sw.Stop();
            sw.Start();
            sw.Stop();
            Assert.AreEqual(elapse2, sw.Max);
            Assert.AreEqual(elapse1, sw.Min);
            Assert.AreEqual(2, sw.Count);
            Assert.AreEqual(0.15, sw.Average);

        }
    }
}
