using FlashKit.Data;
using FlashKit.Data.Records;
using FlashKit.Data.Tags;
using Ionic.Zlib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace FlashKit.IO
{
    public class SwfWriter
    {
        public SwfWriteResult Write(Swf swf)
        {
            SwfWriterContext context = new SwfWriterContext(new SwfByteArray(new byte[0]), swf.Header.FileVersion, new SwfWriteResult());

            int tagCount = swf.Tags.Count;
            byte[][] tagBytes = new byte[tagCount][];

            int iter;

            for (iter = 0; iter < tagCount; iter++)
            {
                SwfTag tag = swf.Tags[iter];

                try
                {
                    context.TagId = iter;
                    context.Bytes = new SwfByteArray(new byte[0]);
                    
                    tag.Write(context);

                    byte[] temp = context.Bytes.Stream.GetBuffer();
                    byte[] currentTagBytes = new byte[context.Bytes.BytePosition];

                    Array.Copy(temp, currentTagBytes, currentTagBytes.Length);

                    tag.Header.Length = currentTagBytes.Length;
                    tagBytes[iter] = currentTagBytes;
                }
                catch (Exception e)
                {
                    context.Result.Errors.Add("Could not write Tag #" + iter + ": " + e.Message);
                }
            }

            context.Bytes = new SwfByteArray(new byte[0]);

            WriteSwfHeader(context, swf.Header);

            for (iter = 0; iter < tagCount; iter++)
            {
                if (tagBytes[iter] == null)
                {
                    continue;
                }

                context.Bytes.AlignBytes();

                SwfTag tag = swf.Tags[iter];
                
                TagHeaderRecord header = tag.Header;

                if (!(tag is UnknownTag))
                {
                    header.Type = tag.Type;
                }

                header.Write(context);
                context.Bytes.WriteBytes(tagBytes[iter]);
            }

            long length = context.Bytes.Length;

            Compress(swf.Header, ref context.Bytes);

            context.Bytes.BytePosition = 4;

            if (swf.Header.Signature == SwfHeader.LzmaCompressedSignature)
            {
                context.Bytes.WriteUI32((uint)(length - 4));
            }
            else
            {
                context.Bytes.WriteUI32((uint)length);
            }

            context.Bytes.BytePosition = 0;

            context.Result.Bytes = context.Bytes.Stream.GetBuffer();

            return context.Result;
        }

        public void WriteSwfHeader(SwfWriterContext context, SwfHeader header)
        {
            SwfByteArray bytes = context.Bytes;

            bytes.WriteStringWithLength(header.Signature, 3);
            bytes.WriteUI8(header.FileVersion);
            bytes.WriteUI32(0);

            header.FrameSize.Write(context);
            bytes.WriteFixed8(header.FrameRate);
            bytes.WriteUI16(header.FrameCount);
        }

        public void Compress(SwfHeader header, ref SwfByteArray bytes)
        {
            if (header.Signature == SwfHeader.ZlibCompressedSignature)
            {
                bytes.BytePosition = 8;

                byte[] content = new byte[bytes.BytesAvailable];
                bytes.ReadBytes(ref content);

                Stopwatch watch = new Stopwatch();
                
                MemoryStream stream = new MemoryStream();
                ZlibStream zlibStream = new ZlibStream(new MemoryStream(content), CompressionMode.Compress);

                byte[] buffer = new byte[4096];
                int read = 1;

                while ((read = zlibStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    stream.Write(buffer, 0, read);
                }
                
                byte[] compressed = new byte[stream.Length];
                Array.Copy(stream.GetBuffer(), compressed, stream.Length);
                
                bytes.BytePosition = 0;
                byte[] temp = new byte[8];
                bytes.ReadBytes(ref temp);

                SwfByteArray newBytes = new SwfByteArray(temp);
                newBytes.BytePosition = 8;
                newBytes.WriteBytes(compressed);

                bytes = newBytes;
            }
            else if (header.Signature == SwfHeader.LzmaCompressedSignature)
            {
                throw new NotSupportedException("LZMA compression is not supported!");
            }
        }
    }
}
