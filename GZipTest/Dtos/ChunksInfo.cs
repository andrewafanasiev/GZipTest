using System.Collections.Generic;

namespace GZipTest.Dtos
{
    public class ChunksInfo
    {
        public ChunksInfo(List<ChunkReadInfo> chunks)
        {
            Chunks = chunks;
            ChunksCount = chunks.Count;
        }

        public int ChunksCount { get; }
        public List<ChunkReadInfo> Chunks { get; }
    }
}