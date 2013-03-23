using FlashKit.IO;

namespace FlashKit.Data.Records
{
    public class RectangleRecord : IRecord
    {
        public int MinX = 0;
        public int MaxX = 0;

        public int MinY = 0;
        public int MaxY = 0;

        public int Width
        {
            get
            {
                return (MaxX - MinX) / 20;
            }
        }

        public int Height
        {
            get
            {
                return (MaxY - MinY) / 20;
            }
        }

        public void Read(SwfReaderContext context)
        {
            context.Bytes.AlignBytes();

            int nBits = (int)context.Bytes.ReadUB(5);

            MinX = context.Bytes.ReadSB(nBits);
            MaxX = context.Bytes.ReadSB(nBits);
            MinY = context.Bytes.ReadSB(nBits);
            MaxY = context.Bytes.ReadSB(nBits);
        }

        public void Write(SwfWriterContext context)
        {
            context.Bytes.AlignBytes();

            int nBits = SwfByteArray.CalculateSBBits(MinX, MaxX, MinY, MaxY);

            context.Bytes.WriteUB(5, (uint)nBits);
            context.Bytes.WriteSB(nBits, MinX);
            context.Bytes.WriteSB(nBits, MaxX);
            context.Bytes.WriteSB(nBits, MinY);
            context.Bytes.WriteSB(nBits, MaxY);
        }
    }
}
