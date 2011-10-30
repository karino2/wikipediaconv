using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using NUnit;

namespace WikipediaConvTest
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            /*
            // "/framework=4.0.30319.239"
            string[] args2;
            if (args.Length == 1)
                args2 = new string[] { args[0], Assembly.GetExecutingAssembly().Location };
            else
                args2 = new string[] { Assembly.GetExecutingAssembly().Location };
            NUnit.ConsoleRunner.Runner.Main(args2);
             * */
            // for profiler
            if (args.Length == 1 && args[0] == "bench")
            {
                Console.WriteLine("start bench mark");
                var bm = new BenchMark();
                bm.EnableReport = false;
                bm.DoBenchmark();
                Console.WriteLine("finish bench mark");
            }
            else
                NUnit.ConsoleRunner.Runner.Main(new string[] { Assembly.GetExecutingAssembly().Location});
        }
    }
}
