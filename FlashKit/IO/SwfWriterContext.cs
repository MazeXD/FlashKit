using FlashKit.Data.Tags;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlashKit.IO
{
    public class SwfWriterContext
    {
        public uint FileVersion;
        public SwfByteArray Bytes;
        public List<byte[]> TagBytes;
        public SwfWriteResult Result;

        //public Object FontGlyphCounts;
        public int TagId;
        public List<SwfTag> TagStack;

        public SwfWriterContext(SwfByteArray bytes, uint fileVersion, SwfWriteResult result)
        {
            this.Bytes = bytes;
            this.FileVersion = fileVersion;
            this.Result = result;

            this.TagStack = new List<SwfTag>();
            // this.FontGlyphCounts = new Object();
        }
    }
}
