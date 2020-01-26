using System;
using System.Collections.Generic;
using System.Text;

namespace GZipTest.Interfaces
{
    public interface IFileReader
    {
        byte[] GetChunkBytes(ChunkInfo chunkInfo);
    }
}
