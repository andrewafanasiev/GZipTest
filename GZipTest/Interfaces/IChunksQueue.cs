using GZipTest.Dtos;

namespace GZipTest.Interfaces
{
    public interface IChunksQueue
    {
        void EnqueueChunk(ChunkInfo chunkInfo);
        bool IsActive();
    }
}