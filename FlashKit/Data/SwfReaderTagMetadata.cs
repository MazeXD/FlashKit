using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlashKit.Data
{
    public class SwfReaderTagMetadata
    {
        public string Name;
        public long Start;
        public long Length;
        public long ContentStart;
        public long ContentLength;

        public SwfReaderTagMetadata(string name = "", long start = -1, long length = -1, long contentStart = -1, long contentLength = -1)
        {
            this.Name = name;
            this.Start = start;
            this.Length = length;
            this.ContentStart = contentStart;
            this.ContentLength = contentLength;
        }
    }
}
