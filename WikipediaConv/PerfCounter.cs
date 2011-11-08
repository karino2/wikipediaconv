using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace WikipediaConv
{
    public class DisposableWatch : IDisposable
    {
        StopWatch _sw;
        public DisposableWatch(StopWatch sw)
        {
            _sw = sw;
            _sw.Start();
        }
        public void Dispose()
        {
            _sw.Stop();
        }
    }
    public class PerfCounter
    {
        public Func<DateTime> Now = () => DateTime.Now;
        Dictionary<string, StopWatch> _stopWatches = new Dictionary<string, StopWatch>();
        public StopWatch GetStopWatch(string name)
        {
            if (!_stopWatches.ContainsKey(name))
                _stopWatches[name] = new StopWatch(name) { Now = Now };
            return _stopWatches[name];
        }
        public double GetTotalSeconds(string name)
        {
            return GetStopWatch(name).Total.TotalMilliseconds / 1000.0;
        }
        public DisposableWatch UsingWatch(string name)
        {
            // Start() is called in constructer.
            return new DisposableWatch(GetStopWatch(name));
        }
        public void Start(string name)
        {
            GetStopWatch(name).Start();
        }
        public void Stop(string name)
        {
            GetStopWatch(name).Stop();
        }

        public override string ToString()
        {
            StringBuilder bldr = new StringBuilder();
            foreach (var sw in _stopWatches.Values) { bldr.Append(sw.ToString()); }
            return bldr.ToString();
        }

        private string _outputPath = null;
        public void SetOutputFolder(string outputDir)
        {
            _outputPath = Path.Combine(outputDir, DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".log");
        }

        public void DumpToLog()
        {
            try
            {
                using (var sw = new StreamWriter(_outputPath, true))
                {
                    sw.Write(ToString());
                    sw.WriteLine("-----");
                    sw.WriteLine();
                    sw.WriteLine();
                }
            }
            catch (Exception)
            {
                // this is just logging, not important task.
            }
            
        }

    }
    public class StopWatch
    {
        public string Name { get; set; }
        public TimeSpan Max { get; set; }
        public TimeSpan Min { get; set; }
        public int Count { get; set; }
        public TimeSpan Total { get; set; }

        public TimeSpan _last;
        public TimeSpan Last
        {
            get
            {
                if (_isStarted)
                    return Now().Subtract(_start);
                return _last;
            }
            set
            {
                _last = value;
            }
        }

        // seconds
        public double Average
        {
            get
            {
                return Total.TotalMilliseconds / (double)(Count*1000);
            }
        }
        // DateTime.Now, for test purpose.
        public Func<DateTime> Now;

        public override string ToString()
        {
            StringBuilder bldr = new StringBuilder();
            bldr.Append(Name);
            bldr.AppendLine(":");

            bldr.Append("call count: ");
            bldr.Append(Count);
            bldr.AppendLine();

            bldr.Append("avg: ");
            bldr.Append(Average);
            bldr.AppendLine();

            bldr.Append("min: ");
            bldr.Append(Min);
            bldr.AppendLine();

            bldr.Append("max: ");
            bldr.Append(Max);
            bldr.AppendLine();

            bldr.Append("last: ");
            bldr.Append(Last);
            bldr.AppendLine();

            bldr.Append("total: ");
            bldr.Append(Total);
            bldr.AppendLine();
            bldr.AppendLine();

            return bldr.ToString();
        }


        DateTime _start;
        bool _isStarted;
        public StopWatch(string name)
        {
            _isStarted = false;
            Name = name;
            Max = TimeSpan.Zero;
            Min = TimeSpan.MaxValue;
            Count = 0;
            Total = new TimeSpan(0);
            Now = () => DateTime.Now;
        }
        public void Start()
        {
            _isStarted = true;
            _start = Now();
        }

        public void Stop()
        {
            if (!_isStarted)
                return; // just ignore.
            _isStarted = false;
            Count++;
            TimeSpan elapsed = Now().Subtract(_start);
            Max = MaxTimeSpan(Max, elapsed);
            Min = MinTimeSpan(Min, elapsed);
            Total += elapsed;
            Last = elapsed;
        }

        public TimeSpan MaxTimeSpan(TimeSpan a, TimeSpan b)
        {
            if (a > b)
                return a;
            return b;
        }
        public TimeSpan MinTimeSpan(TimeSpan a, TimeSpan b)
        {
            if (a < b)
                return a;
            return b;
        }
    }
}
