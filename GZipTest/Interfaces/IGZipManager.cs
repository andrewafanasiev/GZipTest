namespace GZipTest.Interfaces
{
    /// <summary>
    /// Manager performing file compression or decompression
    /// </summary>
    public interface IGZipManager
    {
        /// <summary>
        /// Perform file compression or decompression action
        /// </summary>
        /// <param name="actionType">Action name. Possible values: compress, decompress</param>
        /// <param name="workersCount">Number of threads</param>
        /// <param name="chunkSize">Chunk size in bytes</param>
        void Execute(string actionType, int workersCount, int chunkSize);
    }
}