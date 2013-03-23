using FlashKit.Data.Records;
using FlashKit.IO;
using System.Collections.Generic;

namespace FlashKit.Data.Tags
{
    public class SymbolClassTag : SwfTag
    {
        public Dictionary<ushort, string> Symbols = new Dictionary<ushort, string>();

        public override uint Type
        {
            get { return SwfTagType.SymbolClass; }
        }

        public override void Read(SwfReaderContext context, TagHeaderRecord header)
        {
            uint numSymbols = context.Bytes.ReadUI16();

            for (int i = 0; i < numSymbols; i++)
            {
                Symbols.Add(context.Bytes.ReadUI16(), context.Bytes.ReadString());
            }
        }

        public override void Write(SwfWriterContext context)
        {
            context.Bytes.WriteUI16((ushort)Symbols.Count);

            foreach (var symbol in Symbols)
            {
                context.Bytes.WriteUI16(symbol.Key);
                context.Bytes.WriteString(symbol.Value);
            }
        }
    }
}
