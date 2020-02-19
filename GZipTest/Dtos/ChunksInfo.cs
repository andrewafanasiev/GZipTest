using System.Collections.Generic;

namespace GZipTest.Dtos
{
    /// <summary>
    /// List of chunks in file
    /// </summary>
    public class ChunksInfo
    {
        public List<ChunkReadInfo> Chunks { get; }
        public int ChunksCount => Chunks.Count;

        public ChunksInfo(List<ChunkReadInfo> chunks)
        {
            Chunks = chunks;
        }
    }
}