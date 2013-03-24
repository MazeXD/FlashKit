using FlashKit.IO;

namespace FlashKit.Data.Records
{
    public class SymbolRecord : IRecord
    {
        public ushort CharacterId;
        public string ClassName;
        
        public void Read(SwfReaderContext context)
        {
            context.Bytes.AlignBytes();

            CharacterId = context.Bytes.ReadUI16();
            ClassName = context.Bytes.ReadString();
        }

        public void Write(SwfWriterContext context)
        {
            context.Bytes.AlignBytes();

            context.Bytes.WriteUI16(CharacterId);
            context.Bytes.WriteString(ClassName);
        }
    }
}
