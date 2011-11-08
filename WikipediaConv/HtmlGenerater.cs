using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace WikipediaConv
{
    public class Dumper
    {
        internal BzipReader _bzipReader;
        DumpAction _action;
        public Dumper(string bzipPath, DumpAction act, PerfCounter counter)
        {
            _action = act;
            _bzipReader = new BzipReader(bzipPath, _action, counter);
            _action.Decoder = _bzipReader;
            _action._notify = _bzipReader;
        }
        public void SetSplitFolder(SplitFolder splitFolder, int startSplitLimit)
        {
            _bzipReader.SplitFolder = splitFolder;
            _bzipReader.StartSplitLimit = startSplitLimit;
        }

        static ISplitTactics GetTactics(bool isJapanese)
        {
            if (isJapanese)
                return new JapaneseTactics();
            return new EnglishTactics();
        }

        public static Dumper CreateHtmlGenerater(string bzipPath, bool isJapanese, DirectoryInfoCache result, PerfCounter counter)
        {
            DumpAction htmlAction = DumpAction.CreateHtmlGeneraterAction(bzipPath, result.Item);
            var dumper = new Dumper(bzipPath, htmlAction, counter);
            SplitFolder sf = CreateSplitFolder(isJapanese, result);
            PostCreate(dumper, isJapanese, sf);
            return dumper;
        }

        private static SplitFolder CreateSplitFolder(bool isJapanese, DirectoryInfoCache result)
        {
            SplitFolder sf = new SplitFolder(result, GetTactics(isJapanese));
            return sf;
        }

        private static void PostCreate(Dumper dumper, bool isJapanese, SplitFolder sf)
        {
            dumper._bzipReader.EnableYomi = isJapanese;
            dumper.SetSplitFolder(sf, Properties.Settings.Default.StartSplitLimit);
            // for debug
            /*
            sf.MaxFileNum = 2;
            dumper.SetSplitFolder(sf, 10);
             * */
        }

        public static Dumper CreateRawDumper(string bzipPath, bool isJapanese, DirectoryInfoCache result, PerfCounter counter)
        {
            DumpAction rawDump = DumpAction.CreateRawDumpAction(bzipPath, result.Item);
            Dumper dumper =  new Dumper(bzipPath, rawDump, counter);
            SplitFolder sf = CreateSplitFolder(isJapanese, result);
            // not DRY!
            sf.InterestedFilePattern = "*.wiki";
            PostCreate(dumper, isJapanese, sf);
            return dumper;
        }
        public ILongTask LongTask { get { return _bzipReader; } }
        public DirectoryInfo OutputRoot { get { return _action.OutputRoot; } }

        public bool EnableAutoLogging { set { _bzipReader.EnableAutoLogging = value; } }
    }
}
