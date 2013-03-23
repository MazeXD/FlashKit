using FlashKit.IO;
using System.Collections.Generic;
using System.Drawing;

namespace FlashKit.Data.Records
{
    public class AlphaBitmapDataRecord : IDataRecord
    {
        public List<Color> PixelData = new List<Color>();

        public void Read(SwfReaderContext context, byte colorTableSize, int dataSize)
        {
            for (int i = 0; i < dataSize; i++)
            {
                PixelData.Add(context.Bytes.ReadARGB());
            }
        }

        public void Write(SwfWriterContext context)
        {
            for (int i = 0; i < PixelData.Count; i++)
            {
                context.Bytes.WriteARGB(PixelData[i]);
            }
        }
    }
}
