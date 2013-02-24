using FlashKit.Data.Tags;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlashKit.IO
{
    public class SwfReaderContext
    {
        public uint FileVersion;
        public SwfByteArray Bytes;
        public SwfReadResult Result;

        // public Object FontGlyphCounts;
        public int TagId;
        public List<SwfTag> TagStack;

        public SwfReaderContext(SwfByteArray bytes, uint fileVersion, SwfReadResult result)
        {
            this.Bytes = bytes;
            this.FileVersion = fileVersion;
            this.Result = result;

            this.TagStack = new List<SwfTag>();
            // this.FontGlyphCounts = new Object();
        }
    }
}
