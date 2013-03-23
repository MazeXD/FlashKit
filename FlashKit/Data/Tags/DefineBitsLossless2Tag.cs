using FlashKit.Data.Records;
using FlashKit.IO;
using Ionic.Zlib;
using System;
using System.Drawing;

namespace FlashKit.Data.Tags
{
    public class DefineBitsLossless2Tag : SwfTag
    {
        public const byte ColorMappedImage = 3;
        public const byte ArgbImage = 5;

        public ushort CharacterId;
        public byte BitmapFormat;
        public ushort BitmapWidth;
        public ushort BitmapHeight;
        public byte BitmapColorTableSize;
        public IDataRecord BitmapData;

        private Image _cachedImage;

        public override uint Type
        {
            get { return SwfTagType.DefineBitsLossless2; }
        }

        public override void Read(SwfReaderContext context, TagHeaderRecord header)
        {
            long startPoisition = context.Bytes.BytePosition;

            CharacterId = context.Bytes.ReadUI16();
            BitmapFormat = context.Bytes.ReadUI8();
            BitmapWidth = context.Bytes.ReadUI16();
            BitmapHeight = context.Bytes.ReadUI16();

            if (BitmapFormat == ColorMappedImage)
            {
                BitmapColorTableSize = context.Bytes.ReadUI8();
            }

            if (BitmapFormat == ColorMappedImage || BitmapFormat == 4 || BitmapFormat == ArgbImage)
            {
                byte[] temp = new byte[header.Length - (context.Bytes.BytePosition - startPoisition)];
                context.Bytes.ReadBytes(ref temp);

                temp = ZlibStream.UncompressBuffer(temp);

                SwfReaderContext unzipContext = new SwfReaderContext(new SwfByteArray(temp), context.FileVersion, context.Result);

                if (BitmapFormat == ColorMappedImage)
                {
                    ushort paddedWidth = (ushort)(Math.Ceiling((double)BitmapWidth / 4) * 4);
                    int dataSize = paddedWidth * BitmapHeight;

                    BitmapData = new AlphaColorMapDataRecord();
                    BitmapData.Read(unzipContext, (byte)(BitmapColorTableSize + 1), dataSize);
                }
                else if (BitmapFormat == 4 || BitmapFormat == ArgbImage)
                {
                    int dataSize = BitmapWidth * BitmapHeight;

                    BitmapData = new AlphaBitmapDataRecord();
                    BitmapData.Read(unzipContext, 0, dataSize);
                }
            }
        }

        public override void Write(SwfWriterContext context)
        {
            context.Bytes.WriteUI16(CharacterId);
            context.Bytes.WriteUI8(BitmapFormat);
            context.Bytes.WriteUI16(BitmapWidth);
            context.Bytes.WriteUI16(BitmapHeight);

            if (BitmapFormat == ColorMappedImage)
            {
                context.Bytes.WriteUI8(BitmapColorTableSize);
            }

            if (BitmapFormat == ColorMappedImage || BitmapFormat == 5 || BitmapFormat == ArgbImage)
            {
                SwfWriterContext writeContext = new SwfWriterContext(new SwfByteArray(new byte[0]), context.FileVersion, context.Result);

                if (BitmapFormat == ColorMappedImage)
                {
                    BitmapData.Write(writeContext);
                }
                else if (BitmapFormat == 4 || BitmapFormat == ArgbImage)
                {
                    BitmapData.Write(writeContext);
                }

                byte[] buffer = writeContext.Bytes.Stream.GetBuffer();

                buffer = ZlibStream.CompressBuffer(buffer);

                context.Bytes.WriteBytes(buffer);
            }
        }

        public Image GetImage()
        {
            if (_cachedImage == null)
            {
                Bitmap image = new Bitmap(BitmapWidth, BitmapHeight);

                if (BitmapData is AlphaBitmapDataRecord)
                {
                    var alphaData = (AlphaBitmapDataRecord)BitmapData;

                    for (int y = 0; y < BitmapHeight; y++)
                    {
                        for (int x = 0; x < BitmapWidth; x++)
                        {
                            image.SetPixel(x, y, alphaData.PixelData[y * BitmapWidth + x]);
                        }
                    }
                }
                else if (BitmapData is AlphaColorMapDataRecord)
                {
                    var colorData = (AlphaColorMapDataRecord)BitmapData;

                    for (int y = 0; y < BitmapHeight; y++)
                    {
                        for (int x = 0; x < BitmapWidth; x++)
                        {
                            image.SetPixel(x, y, colorData.ColorTable[colorData.PixelData[y * BitmapWidth + x]]);
                        }
                    }
                }

                _cachedImage = image;
            }

            return _cachedImage;
        }

        public void SetImage(Image image)
        {
            _cachedImage = image;

            Bitmap bitmap = new Bitmap(image);

            BitmapWidth = (ushort)bitmap.Width;
            BitmapHeight = (ushort)bitmap.Height;

            if (BitmapFormat == ColorMappedImage)
            {
                AlphaColorMapDataRecord colorMap = new AlphaColorMapDataRecord();

                for (int y = 0; y < bitmap.Height; y++)
                {
                    for (int x = 0; x < bitmap.Width; x++)
                    {
                        Color color = bitmap.GetPixel(x, y);
                        
                        if (!colorMap.ColorTable.Contains(color))
                        {
                            colorMap.ColorTable.Add(color);
                        }

                        colorMap.PixelData.Add((byte)colorMap.ColorTable.IndexOf(color));
                    }
                }

                BitmapColorTableSize = (byte)colorMap.ColorTable.Count;
                BitmapData = colorMap;
            }
            else if (BitmapFormat == 4 || BitmapFormat == ArgbImage)
            {
                AlphaBitmapDataRecord bitmapMap = new AlphaBitmapDataRecord();

                for (int y = 0; y < bitmap.Height; y++)
                {
                    for (int x = 0; x < bitmap.Width; x++)
                    {
                        Color color = bitmap.GetPixel(x, y);
                        
                        bitmapMap.PixelData.Add(color);
                    }
                }

                BitmapData = bitmapMap;
            }
        }
    }
}
