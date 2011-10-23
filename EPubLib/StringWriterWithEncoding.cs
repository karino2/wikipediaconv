using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace EPubLib
{
    public class StringWriterWithEncoding : StringWriter
    {
        private Encoding _encoding;
        public StringWriterWithEncoding(Encoding encoding)
            : base()
        {
            _encoding = encoding;
        }
        public override Encoding Encoding
        {
            get
            {
                return _encoding;
            }
        }
    }

}
