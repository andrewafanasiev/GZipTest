using GZipTest.Dtos;

namespace GZipTest.Interfaces
{
    public interface IFileSplitter
    {
        ChunksInfo GetChunks(string inFile, int chunkSize);
    }
}