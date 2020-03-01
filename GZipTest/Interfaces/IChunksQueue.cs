using System;
using System.Collections.Generic;
using GZipTest.Dtos;

namespace GZipTest.Interfaces
{
    /// <summary>
    /// Queue for parallel processing of chunks
    /// </summary>
    public interface IChunksQueue : IDisposable
    {
        /// <summary>
        /// Add a chunk to the queue
        /// </summary>
        /// <param name="chunkReadInfo">Chunk information for reading data from file</param>
        void EnqueueChunk(ChunkReadInfo chunkReadInfo);

        /// <summary>
        /// Is queue does any work
        /// </summary>
        /// <returns>Result of checking</returns>
        bool IsActive();

        /// <summary>
        /// Errors occurred while the queue was running
        /// </summary>
        /// <returns>Result of checking</returns>
        bool IsErrorExist();
    }
}