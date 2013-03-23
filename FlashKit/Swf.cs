using FlashKit.Data;
using FlashKit.Data.Tags;
using System.Collections.Generic;

namespace FlashKit
{
    public class Swf
    {
        public SwfHeader Header;
        public List<SwfTag> Tags;

        public Swf(SwfHeader header = null, List<SwfTag> tags = null)
        {
            Header = header;
            Tags = tags;
        }
    }
}
