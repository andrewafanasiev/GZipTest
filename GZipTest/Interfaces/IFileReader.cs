using System;
using System.Collections.Generic;
using System.Text;
using GZipTest.Dtos;

namespace GZipTest.Interfaces
{
    public interface IFileReader
    {
        byte[] GetChunkBytes(ChunkInfo chunkInfo);
    }
}
