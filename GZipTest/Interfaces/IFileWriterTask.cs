using GZipTest.Dtos;

namespace GZipTest.Interfaces
{
    public interface IFileWriterTask
    {
        void AddChunk(int id, ChunkWriteInfo chunk);

        bool IsActive();

        bool IsErrorExist();
    }
}