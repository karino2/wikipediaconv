using System;
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
        public static Dumper CreateHtmlGenerater(string bzipPath, bool isJapanese)
        {
            var dumper = new Dumper(bzipPath, DumpAction.CreateHtmlGeneraterAction(bzipPath));
            dumper._bzipReader.EnableYomi = isJapanese;
            return dumper;
        }
        public static Dumper CreateRawDumper(string bzipPath, bool isJapanese)
        {
            Dumper dumper =  new Dumper(bzipPath, DumpAction.CreateRawDumpAction(bzipPath));
            dumper._bzipReader.EnableYomi = isJapanese;
            return dumper;
        }
        public ILongTask LongTask { get { return _bzipReader; } }
        public DirectoryInfo WorkingFolder { get { return _action.WorkingFolder; } }
    }
}
