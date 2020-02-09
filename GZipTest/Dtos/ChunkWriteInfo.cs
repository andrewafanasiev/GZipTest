using System;
using System.Collections.Generic;
using System.Text;

namespace GZipTest.Dtos
{
    public class ChunkWriteInfo
    {
        public readonly int Id;
        public readonly byte[] Content;

        public ChunkWriteInfo(int id, byte[] content)
        {
            Id = id;
            Content = content;
        }
    }
}
