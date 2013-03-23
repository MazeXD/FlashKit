using FlashKit.Data.Records;
using FlashKit.IO;

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
