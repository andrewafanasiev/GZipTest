using GZipTest.Dtos;

namespace GZipTest.Interfaces
{
    public interface IFileSplitter
    {
        /// <summary>
        /// Split file to chunks
        /// </summary>
        /// <param name="inFile">Input file</param>
        /// <param name="chunkSize">Chunk size</param>
        /// <returns>Split result</returns>
        ChunksInfo GetChunks(string inFile, int chunkSize);
    }
}