namespace GZipTest.Dtos
{
    /// <summary>
    /// Chunk information for writing data in file
    /// </summary>
    public class ChunkWriteInfo
    {
        public readonly int Id;
        public readonly byte[] Content;

        public ChunkWriteInfo(int id, byte[] content)
        {
            Id = id;
            Content = content;
        }
    }
}
