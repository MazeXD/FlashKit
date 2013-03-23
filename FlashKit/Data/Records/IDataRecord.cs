using FlashKit.IO;

namespace FlashKit.Data.Records
{
    public interface IDataRecord
    {
        void Read(SwfReaderContext context, byte colorTableSize, int dataSize);
        void Write(SwfWriterContext context);
    }
}
