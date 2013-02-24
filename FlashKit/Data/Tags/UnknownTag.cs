using FlashKit.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlashKit.Data.Tags
{
    public class UnknownTag : SwfTag
    {
        public byte[] Content = new byte[0];

        public override uint Type
        {
            get { return 0; }
        }

        public override void Read(SwfReaderContext context, Records.TagHeaderRecord header)
        {
            if (header.Length > 0)
            {
                context.Bytes.ReadBytes(ref Content, (int)header.Length);
            }
        }

        public override void Write(SwfWriterContext context)
        {
            context.Bytes.WriteBytes(Content);
        }

        public override string ToString()
        {
            return "[UnknownTag Type: " + Header.Type + "]";
        }
    }
}
