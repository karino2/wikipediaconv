using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace WikipediaConv
{
    public class HtmlGenerater
    {
        BzipReader _bzipReader;
        HtmlGeneraterAction _action;
        public HtmlGenerater(string bzipPath)
        {
            _action = new HtmlGeneraterAction(bzipPath);
            _bzipReader = new BzipReader(bzipPath, _action);
            _action.Decoder = _bzipReader;
            _action._notify = _bzipReader;
        }
        public ILongTask LongTask { get { return _bzipReader; } }
        public DirectoryInfo WorkingFolder { get { return _action.WorkingFolder; } }
    }
}
