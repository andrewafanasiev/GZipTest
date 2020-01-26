namespace GZipTest.Interfaces
{
    public interface IFileWriterTask
    {
        void AddChunk(int id, byte[] bytes);

        bool IsActive();
    }
}