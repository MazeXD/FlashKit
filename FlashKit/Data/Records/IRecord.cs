using FlashKit.IO;

namespace FlashKit.Data.Records
{
    public interface IRecord
    {
        void Read(SwfReaderContext context);
        void Write(SwfWriterContext context);
    }
}
