using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using WikipediaConv;
using System.IO;
using System.Threading;

namespace WikipediaConvTest
{
    [TestFixture]
    public class BenchMark
    {
        public BenchMark()
        {
            EnableReport = true;
        }
        public bool EnableReport { get; set; }

        // [Test]
        public void DoSomething()
        {
            /*
            using (var reader = new FileStream(@"../../../../jawiki-20110921-pages-articles.xml", FileMode.Open))
            {
                const int size = 1024 * 1024;
                byte[] buf = new byte[size];
                using (var headWriter = new FileStream(@"../../../../jahead.xml", FileMode.CreateNew))
                {
                    var res = reader.Read(buf, 0, size);
                    headWriter.Write(buf, 0, res);
                }

                using (var tailWriter = new FileStream(@"../../../../jatail.xml", FileMode.CreateNew))
                {
                    reader.Seek(-size, SeekOrigin.End);
                    var res = reader.Read(buf, 0, size);
                    tailWriter.Write(buf, 0, res);
                }
            }
             * */
            using (var reader = new FileStream(@"../../../../jawiki-20110921-pages-articles.xml", FileMode.Open))
            {
                const int size = 1024 * 1024*10;
                byte[] buf = new byte[size];
                using (var headWriter = new FileStream(@"../../../../jahead_10M.xml", FileMode.CreateNew))
                {
                    var res = reader.Read(buf, 0, size);
                    headWriter.Write(buf, 0, res);
                }
            }
        }

        [Test]
        public void TestIndexOf()
        {
            var target = "abcdefgdd";
            var actual = target.IndexOf("def", 3, 3);
            Assert.AreEqual(3, actual);
        }

        // this is not test, but use nunit!
        [Test]
        public void DoBenchmark()
        {
            var solutionDir = @"../../../";
            var benchDir = solutionDir + "bench/";
            var outputDir = new DirectoryInfo(solutionDir + "test_tmp_result");
            CleanUp(outputDir);
            var bzipPath = benchDir + "jahead.xml.bz2";
            // var bzipPath = benchDir + "jahead_10M.xml.bz2";
            PerfCounter counter = new PerfCounter();
            counter.Start("AllBench");
            var dumper = Dumper.CreateRawDumper(bzipPath, true, outputDir, counter);
            // simulate split folder for large data
            SetupParameterForBenchMark(dumper);
            dumper._bzipReader.DecodeAsync();

            bzipPath = benchDir + "jatail.xml.bz2";
            dumper = Dumper.CreateRawDumper(bzipPath, true, outputDir, counter);
            SetupParameterForBenchMark(dumper);
            dumper._bzipReader.DecodeAsync();

            counter.Stop("AllBench");
            if(EnableReport)
                Report(benchDir, counter);

            Console.WriteLine(counter.ToString());
        }



        private void Report(string benchDir, PerfCounter counter)
        {
            var csvPath = benchDir + "result.csv";
            var tmpPath = benchDir + "result_tmp.csv";
            using (var input = new StreamReader(csvPath))
            {
                using (var output = new StreamWriter(tmpPath))
                {
                    output.Write(input.ReadToEnd());
                    var bldr = new StringBuilder();
                    bldr.Append(DateTime.Now.ToString("yyyy/MM/dd HH:mm"));
                    bldr.Append(",");
                    bldr.AppendFormat(" {0:0.00}", counter.GetTotalSeconds("AllBench"));
                    bldr.Append(",");
                    bldr.AppendFormat(" {0:0.00}", counter.GetTotalSeconds("LocateBlock"));
                    bldr.Append(",");
                    bldr.AppendFormat(" {0:0.00}", counter.GetTotalSeconds("LoadBlock"));
                    bldr.Append(",");
                    bldr.AppendFormat(" {0:0.00}", counter.GetTotalSeconds("Action"));
                    bldr.Append(",");
                    bldr.AppendFormat(" {0:0.00}", counter.GetTotalSeconds("Split"));
                    bldr.Append(",");
                    bldr.AppendFormat(" {0:0.00}", counter.GetTotalSeconds("GetYomi"));
                    bldr.Append(",");
                    bldr.AppendFormat(" {0:0.00}", counter.GetTotalSeconds("Other"));
                    bldr.Append(", "); // for comment field.
                    output.WriteLine(bldr.ToString());                        
                }
            }
            var fi = new FileInfo(csvPath);
            fi.Delete();
            fi = new FileInfo(tmpPath);
            fi.MoveTo(csvPath);
        }

        private static void SetupParameterForBenchMark(Dumper dumper)
        {
            dumper._bzipReader.SplitFolder.MaxFileNum = 2;
            dumper._bzipReader.StartSplitLimit = 10;
        }

        private void CleanUp(DirectoryInfo outputDir)
        {
            if (outputDir.Exists)
            {
                var walker = SplitFolder.DirectoryForest(outputDir).Walker;
                while (walker.HasNext)
                {
                    walker.MoveNext();
                    var cur = walker.Current;
                    var dir = cur.Element;
                    if (cur.CurrentEdge == ForestNode<DirectoryInfo>.Edge.Leading)
                    {
                        var files = dir.GetFiles();
                        Array.ForEach(files, (f) => f.Delete());
                    }
                    if (cur.CurrentEdge == ForestNode<DirectoryInfo>.Edge.Trailing)
                    {
                        var children = dir.GetDirectories();
                        Array.ForEach(children, (d) => d.Delete());
                    }
                }
            }
            else{
                outputDir.Create();
            }
       }
    }
}
