using FlashKit.Data.Records;
using FlashKit.IO;

namespace FlashKit.Data.Tags
{
    public class DefineBinaryDataTag : SwfTag
    {
        public ushort CharacterId;
        public byte[] Data = new byte[0];

        public override uint Type
        {
            get { return SwfTagType.DefineBinaryData; }
        }

        public override void Read(SwfReaderContext context, TagHeaderRecord header)
        {
            CharacterId = context.Bytes.ReadUI16();

            context.Bytes.ReadUI32();

            Data = new byte[header.Length - 6];
            context.Bytes.ReadBytes(ref Data);
        }

        public override void Write(SwfWriterContext context)
        {
            context.Bytes.WriteUI16(CharacterId);

            context.Bytes.WriteUI32(0);

            context.Bytes.WriteBytes(Data);
        }
    }
}
