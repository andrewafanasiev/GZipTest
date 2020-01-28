using GZipTest.Dtos;

namespace GZipTest.Interfaces
{
    public interface IFileWriterTask
    {
        void AddChunk(int id, Chunk chunk);

        bool IsActive();
    }
}