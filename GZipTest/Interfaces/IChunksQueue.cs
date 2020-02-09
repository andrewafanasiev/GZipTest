using GZipTest.Dtos;

namespace GZipTest.Interfaces
{
    public interface IChunksQueue
    {
        void EnqueueChunk(ChunkReadInfo chunkReadInfo);
        bool IsActive();
    }
}