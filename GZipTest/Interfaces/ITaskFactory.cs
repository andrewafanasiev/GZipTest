using System;
using System.Collections.Generic;
using System.Text;

namespace GZipTest.Interfaces
{
    public interface ITaskFactory
    {
        /// <summary>
        /// Create queue for processing chunks from the source
        /// </summary>
        /// <param name="workersCount">Thread count in queue</param>
        /// <param name="reader">Reader from source</param>
        /// <param name="compressor">Interface for compression, decompression</param>
        /// <param name="writerTask">Task for write data</param>
        /// <returns>Chunks queue</returns>
        IChunksReader CreateChunksReader(int workersCount, ISourceReader reader, IGZipCompressor compressor,
            IWriterTask writerTask);

        /// <summary>
        /// Create task for writing chunks after compression, decompression
        /// </summary>
        /// <param name="chunksCount">Count of chunks for write</param>
        /// <param name="chunkWriter">Writer</param>
        /// <returns>Writer task</returns>
        IWriterTask CreatWriterTask(int chunksCount, IChunkWriter chunkWriter);
    }
}
