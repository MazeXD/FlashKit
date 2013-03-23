using FlashKit.IO;

namespace FlashKit.Data.Tags
{
    public class EndTag : SwfTag
    {
        public override uint Type
        {
            get { return SwfTagType.End; }
        }

        public override void Read(SwfReaderContext context, Records.TagHeaderRecord header) {}

        public override void Write(SwfWriterContext context) {}
    }
}
