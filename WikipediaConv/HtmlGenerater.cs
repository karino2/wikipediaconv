using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WikipediaConv
{
    public class EPubGenerater
    {
        BzipReader _bzipReader;
        EPubGeneraterAction _action;
        public EPubGenerater(string bzipPath)
        {
            _action = new EPubGeneraterAction(bzipPath);
            _bzipReader = new BzipReader(bzipPath, _action);
            _action.Decoder = _bzipReader;
            _action._notify = _bzipReader;
        }
        public ILongTask LongTask { get { return _bzipReader; } }
    }
}
