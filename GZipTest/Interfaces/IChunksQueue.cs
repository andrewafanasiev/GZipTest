using GZipTest.Dtos;

namespace GZipTest.Interfaces
{
    public interface IChunksQueue
    {
        void EnqueueChunk(Chunk chunk);
        bool IsActive();
    }
}