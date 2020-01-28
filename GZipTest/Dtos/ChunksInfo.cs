using System.Collections.Generic;

namespace GZipTest.Dtos
{
    public class ChunksInfo
    {
        public ChunksInfo(int maxChunkId, List<Chunk> chunks)
        {
            MaxChunkId = maxChunkId;
            Chunks = chunks;
        }

        public int MaxChunkId { get; }
        public List<Chunk> Chunks { get; }
    }
}