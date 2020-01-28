using System.Collections.Generic;
using GZipTest.Dtos;

namespace GZipTest.Interfaces
{
    public interface IFilePreparation
    {
        ChunksInfo GetChunks(string inFile, int chunkSize);
    }
}