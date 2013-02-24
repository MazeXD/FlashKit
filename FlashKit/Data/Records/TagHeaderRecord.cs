using FlashKit.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlashKit.Data.Records
{
    public class TagHeaderRecord : IRecord
    {
        public const uint ShortHeaderMaxLength = 0x3f;

        public uint Type = 0;
        public int Length = 0;
        public bool ForceLong = false;

        public bool IsLong
        {
            get
            {
                return this.Length >= ShortHeaderMaxLength;
            }
        }

        public void Read(SwfReaderContext context)
        {
            uint tagInfo = context.Bytes.ReadUI16();

            Type = tagInfo >> 6;

            int length = (int)(tagInfo & ((1 << 6) - 1));

            if (length == 0x3f)
            {
                length = context.Bytes.ReadSI32();
                ForceLong = true;
            }

            Length = length;
        }

        public void Write(SwfWriterContext context)
        {
            int length = Length;
            bool longLength = false;

            if (length > 0x3d || ForceLong)
            {
                longLength = true;
                length = 0x3f;
            }

            ushort tagInfo = (ushort)((ushort)((Type & SwfByteArray.Filter10) << 6) | length);
            context.Bytes.WriteUI16(tagInfo);

            if (longLength)
            {
                context.Bytes.WriteSI32(Length);
            }
        }
    }
}
