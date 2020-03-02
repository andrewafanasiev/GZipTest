using System;
using System.Collections.Generic;
using System.Text;
using GZipTest.Dtos;

namespace GZipTest.Interfaces
{
    /// <summary>
    /// Read data from input file
    /// </summary>
    public interface ISourceReader
    {
        /// <summary>
        /// Get chunk content
        /// </summary>
        /// <param name="chunkReadInfo">Info about chunk</param>
        /// <returns>Chunk content</returns>
        byte[] GetChunkBytes(ChunkReadInfo chunkReadInfo);
    }
}
