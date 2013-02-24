using FlashKit.Data;
using FlashKit.Data.Tags;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlashKit
{
    public class Swf
    {
        public SwfHeader Header;
        public List<SwfTag> Tags;

        public Swf(SwfHeader header = null, List<SwfTag> tags = null)
        {
            this.Header = header;
            this.Tags = tags;
        }
    }
}
