using FlashKit.Data.Records;
using FlashKit.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlashKit.Data.Tags
{
    public abstract class SwfTag
    {
        public TagHeaderRecord Header = new TagHeaderRecord();

        public abstract uint Type { get; }

        public abstract void Read(SwfReaderContext context, TagHeaderRecord header);
        public abstract void Write(SwfWriterContext context);
    }
}
