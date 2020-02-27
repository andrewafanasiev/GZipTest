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
        /// <returns>Operation result</returns>
        bool Execute(string actionType, int workersCount, int chunkSize);

        /// <summary>
        /// Is operation active
        /// </summary>
        /// <returns>Result of checking</returns>
        bool IsActiveOp(IChunksQueue chunksQueue, IWriterTask writerTask);

        /// <summary>
        /// An error occurred during the execution
        /// </summary>
        /// <returns>Result of checking</returns>
        bool IsErrorExist(IChunksQueue chunksQueue, IWriterTask writerTask);
    }
}