using FlashKit.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlashKit.Data.Records
{
    public interface IDataRecord
    {
        void Read(SwfReaderContext context, byte colorTableSize, int dataSize);
        void Write(SwfWriterContext context);
    }
}
