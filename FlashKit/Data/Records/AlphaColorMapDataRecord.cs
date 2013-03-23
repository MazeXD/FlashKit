using FlashKit.IO;
using System.Collections.Generic;
using System.Drawing;

namespace FlashKit.Data.Records
{
    public class AlphaColorMapDataRecord : IDataRecord
    {
        public List<Color> ColorTable = new List<Color>();
        public List<byte> PixelData = new List<byte>();

        public void Read(SwfReaderContext context, byte colorTableSize, int dataSize)
        {
            ColorTable = new List<Color>(colorTableSize);
            
            for (int i = 0; i < colorTableSize; i++)
            {
                ColorTable.Add(context.Bytes.ReadRGBA());
            }

            PixelData = new List<byte>(dataSize);
            for (int i = 0; i < dataSize; i++)
            {
                PixelData.Add(context.Bytes.ReadUI8());
            }
        }

        public void Write(SwfWriterContext context)
        {
            for (int i = 0; i < ColorTable.Count; i++)
            {
                context.Bytes.WriteRGBA(ColorTable[i]);
            }

            for (int i = 0; i < PixelData.Count; i++)
            {
                context.Bytes.WriteUI8(PixelData[i]);
            }
        }
    }
}
