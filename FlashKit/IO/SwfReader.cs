using FlashKit.Data;
using FlashKit.Data.Records;
using FlashKit.Data.Tags;
using Ionic.Zlib;
using System;
using System.Collections.Generic;
using System.IO;

namespace FlashKit.IO
{
    public class SwfReader
    {
        private readonly Dictionary<uint, SwfTag> _tags = new Dictionary<uint, SwfTag>
            {
            { SwfTagType.End, new EndTag() }
        };

        public bool CatchErrors = false;

        public SwfReader()
        {
            AddType(new DefineBitsLossless2Tag());
            AddType(new SymbolClassTag());
            AddType(new DefineBinaryDataTag());
        }

        public void AddType(SwfTag tag)
        {
            if (tag.Type == SwfTagType.End)
            {
                throw new NotSupportedException("Replacing the EndTag is not supported");
            }

            if (_tags.ContainsKey(tag.Type))
            {
                _tags[tag.Type] = tag;
            }
            else
            {
                _tags.Add(tag.Type, tag);
            }
        }

        public void RemoveType(uint type)
        {
            if (type == SwfTagType.End)
            {
                throw new NotSupportedException("Removing reading of EndTag is not supported");
            }
            if (_tags.ContainsKey(type))
            {
                _tags.Remove(type);
            }
        }

        public SwfReadResult Read(string fileName)
        {
            if (!File.Exists(fileName))
            {
                throw new FileNotFoundException("Can't find file: " + fileName);
            }

            return Read(File.ReadAllBytes(fileName));
        }

        public SwfReadResult Read(byte[] bytes)
        {
            return Read(new SwfByteArray(bytes));
        }

        public SwfReadResult Read(SwfByteArray bytes)
        {
            SwfReadResult result = new SwfReadResult();

            Swf swf = new Swf();
            swf.Header = new SwfHeader();
            swf.Tags = new List<SwfTag>();

            SwfReaderContext context = new SwfReaderContext(bytes, 0, result);

            ReadSwfHeader(context, ref swf.Header);

            context.FileVersion = swf.Header.FileVersion;
            
            while (bytes.BytesAvailable > 0)
            {
                int tagId = swf.Tags.Count;
                long preHeaderStart = bytes.BytePosition;

                TagHeaderRecord header = new TagHeaderRecord();
                header.Read(context);

                long startPosition = context.Bytes.BytePosition;
                long expectedEndPosition = startPosition + header.Length;

                context.TagId = tagId;

                SwfTag tag;
                if (CatchErrors)
                {
                    try
                    {
                        tag = ReadTag(context, header);
                    }
                    catch (Exception)
                    {
                        result.Errors.Add("Error parsing Tag #" + tagId + " (type: " + header.Type + ")");
                        bytes.BytePosition = startPosition;

                        tag = new UnknownTag();

                        tag.Read(context, header);
                    }
                }
                else
                {
                    tag = ReadTag(context, header);
                }

                tag.Header = header;

                swf.Tags.Add(tag);

                context.Bytes.AlignBytes();

                long newPosition = context.Bytes.BytePosition;

                if (newPosition > expectedEndPosition)
                {
                    result.Warnings.Add("Read overflow for Tag #" + tagId + " (type: " + tag.Header.Type + "). Read " + (newPosition - startPosition) + " bytes, expected " + tag.Header.Length + " bytes");
                }

                if (newPosition < expectedEndPosition)
                {
                    result.Warnings.Add("Read underflow for Tag #" + tagId + " (type: " + tag.Header.Type + "). Read " + (newPosition - startPosition) + " bytes, expected " + tag.Header.Length + " bytes");
                }

                bytes.BytePosition = expectedEndPosition;

                SwfReaderTagMetadata metadata = new SwfReaderTagMetadata(tag.GetType().Name, preHeaderStart, (expectedEndPosition - preHeaderStart), startPosition, tag.Header.Length);
                result.TagMetadata.Add(tagId, metadata);

                if (tag is UnknownTag)
                {
                    result.Warnings.Add("Unknown tag type: " + header.Type + " (id: " + tagId + ")");
                }

                if (tag is EndTag)
                {
                    break;
                }
            }

            result.Swf = swf;
            bytes.BytePosition = 0;

            return result;
        }

        public SwfTag ReadTag(SwfReaderContext context, TagHeaderRecord header)
        {
            SwfTag tag;

            if (_tags.ContainsKey(header.Type))
            {
                tag = (SwfTag)Activator.CreateInstance(_tags[header.Type].GetType());
            }
            else
            {
                tag = new UnknownTag();
            }

            /*
            switch (header.Type)
            {
                case SwfTagType.End:
                    tag = new EndTag();
                    break;
                case SwfTagType.DefineBitsLossless2:
                    tag = new DefineBitsLossless2Tag();
                    break;
                case SwfTagType.SymbolClass:
                    tag = new SymbolClassTag();
                    break;
                case SwfTagType.DefineBinaryData:
                    tag = new DefineBinaryDataTag();
                    break;
                default:
                    tag = new UnknownTag();
                    break;
            }
            */

            tag.Read(context, header);

            return tag;
        }

        public void ReadSwfHeader(SwfReaderContext context, ref SwfHeader header)
        {
            SwfByteArray bytes = context.Bytes;

            header.Signature = bytes.ReadStringWithLength(3);
            header.FileVersion = bytes.ReadUI8();
            header.UncompressedSize = bytes.ReadUI32();

            Decompress(header, ref bytes);

            header.FrameSize.Read(context);
            header.FrameRate = bytes.ReadFixed8();
            header.FrameCount = bytes.ReadUI16();
        }

        public void Decompress(SwfHeader header, ref SwfByteArray bytes)
        {
            if (header.Signature == SwfHeader.ZlibCompressedSignature)
            {
                Stream stream = bytes.Stream;

                long startPosition = bytes.BytePosition;
                byte[] buffer = new byte[header.UncompressedSize];

                using (ZlibStream zlibStream = new ZlibStream(stream, CompressionMode.Decompress, true))
                {
                    zlibStream.Read(buffer, 0, buffer.Length);
                    zlibStream.Close();
                }

                bytes.BytePosition = startPosition;
                stream.Write(buffer, 0, buffer.Length);
                bytes.BytePosition = startPosition;
            }
            else if (header.Signature == SwfHeader.LzmaCompressedSignature)
            {
                throw new NotSupportedException("LZMA compressed Swf files are not supported");
            }
        }
    }
}
