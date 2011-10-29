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

        public static Dumper CreateHtmlGenerater(string bzipPath, bool isJapanese, DirectoryInfo result, PerfCounter counter)
        {
            DumpAction htmlAction = DumpAction.CreateHtmlGeneraterAction(bzipPath);
            htmlAction.OutputRoot = result;
            var dumper = new Dumper(bzipPath, htmlAction, counter);
            SplitFolder sf = CreateSplitFolder(isJapanese, dumper);
            PostCreate(dumper, isJapanese, sf);
            return dumper;
        }

        private static SplitFolder CreateSplitFolder(bool isJapanese, Dumper dumper)
        {
            SplitFolder sf = new SplitFolder(dumper.OutputRoot, GetTactics(isJapanese));
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

        public static Dumper CreateRawDumper(string bzipPath, bool isJapanese, DirectoryInfo result, PerfCounter counter)
        {
            DumpAction rawDump = DumpAction.CreateRawDumpAction(bzipPath);
            rawDump.OutputRoot = result;
            Dumper dumper =  new Dumper(bzipPath, rawDump, counter);
            SplitFolder sf = CreateSplitFolder(isJapanese, dumper);
            sf.Extension = ".wiki";
            PostCreate(dumper, isJapanese, sf);
            return dumper;
        }
        public ILongTask LongTask { get { return _bzipReader; } }
        public DirectoryInfo OutputRoot { get { return _action.OutputRoot; } }
    }
}
