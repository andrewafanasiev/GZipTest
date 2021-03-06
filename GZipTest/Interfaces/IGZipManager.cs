﻿using System;
using System.Collections.Generic;

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
        /// <param name="errors">Exceptions</param>
        /// <returns>Operation result</returns>
        bool Execute(string actionType, int workersCount, int chunkSize, out List<Exception> errors);

        /// <summary>
        /// Is operation active
        /// </summary>
        /// <returns>Result of checking</returns>
        bool IsActiveOp(IChunksReader chunksReader, IWriterTask writerTask);
    }
}