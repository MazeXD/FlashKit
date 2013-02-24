using FlashKit.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlashKit.Data.Records
{
    public interface IRecord
    {
        void Read(SwfReaderContext context);
        void Write(SwfWriterContext context);
    }
}
