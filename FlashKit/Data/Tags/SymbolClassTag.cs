using System.Globalization;
using System.Linq;
using FlashKit.Data.Records;
using FlashKit.IO;
using System.Collections.Generic;

namespace FlashKit.Data.Tags
{
    public class SymbolClassTag : SwfTag
    {
        public List<SymbolRecord> Symbols = new List<SymbolRecord>();

        public override uint Type
        {
            get { return SwfTagType.SymbolClass; }
        }

        public string GetClassForCharacter(ushort characterId)
        {
            foreach (var symbol in Symbols.Where(symbol => symbol.CharacterId == characterId))
            {
                return symbol.ClassName;
            }

            return characterId.ToString(CultureInfo.InvariantCulture);
        }

        public override void Read(SwfReaderContext context, TagHeaderRecord header)
        {
            uint numSymbols = context.Bytes.ReadUI16();

            for (int i = 0; i < numSymbols; i++)
            {
                var symbol = new SymbolRecord();
                symbol.Read(context);

                Symbols.Add(symbol);
            }
        }

        public override void Write(SwfWriterContext context)
        {
            context.Bytes.WriteUI16((ushort)Symbols.Count);

            foreach (var symbol in Symbols)
            {
                symbol.Write(context);
            }
        }
    }
}
