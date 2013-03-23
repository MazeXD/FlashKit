using FlashKit.Data;
using System.Collections.Generic;

namespace FlashKit.IO
{
    public class SwfReadResult
    {
        public Swf Swf;
        public Dictionary<int, SwfReaderTagMetadata> TagMetadata = new Dictionary<int, SwfReaderTagMetadata>();
        // public Dictionary<int, AbcReaderMetadata> AbcMetadata = new Dictionary<int, AbcReaderMetadata>();
        public List<string> Warnings = new List<string>();
        public List<string> Errors = new List<string>();
    }
}
