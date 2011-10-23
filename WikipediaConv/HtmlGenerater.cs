﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace WikipediaConv
{
    public class Dumper
    {
        BzipReader _bzipReader;
        DumpAction _action;
        public Dumper(string bzipPath, DumpAction act)
        {
            _action = act;
            _bzipReader = new BzipReader(bzipPath, _action);
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

        public static Dumper CreateHtmlGenerater(string bzipPath, bool isJapanese)
        {
            var dumper = new Dumper(bzipPath, DumpAction.CreateHtmlGeneraterAction(bzipPath));
            SplitFolder sf = CreateSplitFolder(isJapanese, dumper);
            PostCreate(dumper, isJapanese, sf);
            return dumper;
        }

        private static SplitFolder CreateSplitFolder(bool isJapanese, Dumper dumper)
        {
            SplitFolder sf = new SplitFolder(dumper.WorkingFolder, GetTactics(isJapanese));
            return sf;
        }

        private static void PostCreate(Dumper dumper, bool isJapanese, SplitFolder sf)
        {
            dumper._bzipReader.EnableYomi = isJapanese;
            dumper.SetSplitFolder(sf, 5000);
            // for debug
            /*
            sf.MaxFileNum = 2;
            dumper.SetSplitFolder(sf, 10);
             * */
        }

        public static Dumper CreateRawDumper(string bzipPath, bool isJapanese)
        {
            Dumper dumper =  new Dumper(bzipPath, DumpAction.CreateRawDumpAction(bzipPath));
            SplitFolder sf = CreateSplitFolder(isJapanese, dumper);
            sf.Extension = ".wiki";
            PostCreate(dumper, isJapanese, sf);
            return dumper;
        }
        public ILongTask LongTask { get { return _bzipReader; } }
        public DirectoryInfo WorkingFolder { get { return _action.WorkingFolder; } }
    }
}
