using System.Collections.Generic;

namespace FlashKit.IO
{
    public class SwfWriteResult
    {
        public List<string> Warnings = new List<string>();
        public List<string> Errors = new List<string>();
        public byte[] Bytes;
        // public Dictionary<int, SwfReaderTagMetadata> TagMetadata = new Dictionary<int, SwfReaderTagMetadata>();
        // public List<AbcWriterMetadata> = new List<AbcWriterMetadata>();
    }
}
