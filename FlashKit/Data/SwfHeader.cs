using FlashKit.Data.Records;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlashKit.Data
{
    public class SwfHeader
    {
        public const string UncompressedSignature = "FWS";
        public const string ZlibCompressedSignature = "CWS";
        public const string LzmaCompressedSignature = "ZWS";

        public string Signature = UncompressedSignature;
        public byte FileVersion = 0;
        public uint UncompressedSize = 0;

        public RectangleRecord FrameSize = new RectangleRecord();
        public float FrameRate = float.NaN;
        public ushort FrameCount = 0;
    }
}
