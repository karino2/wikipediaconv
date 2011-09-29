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
            NUnit.ConsoleRunner.Runner.Main(new string[] {Assembly.GetExecutingAssembly().Location});
        }
    }
}
