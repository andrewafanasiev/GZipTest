using System;
using GZipTest.Dtos;

namespace GZipTest.Interfaces
{
    /// <summary>
    /// Task for write chunks in receiver
    /// </summary>
    public interface IWriterTask : IDisposable
    {
        /// <summary>
        /// Add chunk to write to file
        /// </summary>
        void AddChunk(int id, ChunkWriteInfo chunk);

        /// <summary>
        /// Is file writer task does any work
        /// </summary>
        /// <returns>Result of checking</returns>
        bool IsActive();

        /// <summary>
        /// Is error occurred while the task was running
        /// </summary>
        /// <returns>Result of checking</returns>
        bool IsErrorExist(out Exception error);
    }
}