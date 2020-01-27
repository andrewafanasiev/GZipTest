namespace GZipTest.Dtos
{
    public class ChunkInfo
    {
        public ChunkInfo(int id, long offset, int bytesCount)
        {
            Id = id;
            Offset = offset;
            BytesCount = bytesCount;
        }

        public int Id { get; set; }
        public long Offset { get; set; }
        public int BytesCount { get; set; }
    }
}
