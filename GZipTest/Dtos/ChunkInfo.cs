using System;
using System.Collections.Generic;
using System.Text;

namespace GZipTest
{
    public class ChunkInfo
    {
        public int Id { get; set; }
        public long Offset { get; set; }
        public int BytesCount { get; set; }
    }
}
