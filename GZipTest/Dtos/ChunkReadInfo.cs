namespace GZipTest.Dtos
{
    /// <summary>
    /// Chunk information for reading data from file
    /// </summary>
    public class ChunkReadInfo
    {
        public readonly int Id;
        public readonly long Offset;
        public readonly int BytesCount;

        public ChunkReadInfo(int id, long offset, int bytesCount)
        {
            Id = id;
            Offset = offset;
            BytesCount = bytesCount;
        }
    }
}
