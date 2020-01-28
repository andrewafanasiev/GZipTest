namespace GZipTest.Dtos
{
    public class Chunk
    {
        public Chunk(int id, long offset, int bytesCount)
        {
            Id = id;
            Offset = offset;
            BytesCount = bytesCount;
        }

        public int Id { get; }
        public long Offset { get; }
        public int BytesCount { get; }
        public byte[] Content { get; private set; }
        public bool IsWriteToFile { get; set; }

        public void SetContentForRecording(byte[] content)
        {
            Content = content;
            IsWriteToFile = false;
        }
    }
}
