using System.Collections.Generic;
using GZipTest.Dtos;

namespace GZipTest.Interfaces
{
    public interface IFilePreparation
    {
        List<ChunkInfo> GetChunkInfos(string inFile);
    }
}