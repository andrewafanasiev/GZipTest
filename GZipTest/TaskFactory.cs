using System;
using System.Collections.Generic;
using System.Text;
using GZipTest.Interfaces;

namespace GZipTest
{
    public class TaskFactory : ITaskFactory
    {
        /// <summary>
        /// Create queue for processing chunks from the source
        /// </summary>
        /// <param name="workersCount">Thread count in queue</param>
        /// <param name="reader">Reader from source</param>
        /// <param name="compressor">Interface for compression, decompression</param>
        /// <param name="writerTask">Task for write data</param>
        /// <returns>Chunks queue</returns>
        public IChunksQueue CreateChunksQueue(int workersCount, ISourceReader reader, IGZipCompressor compressor, IWriterTask writerTask)
        {
            return new ChunksQueue(workersCount, reader, compressor, writerTask);
        }

        /// <summary>
        /// Create task for writing chunks after compression, decompression
        /// </summary>
        /// <param name="chunksCount">Count of chunks for write</param>
        /// <param name="chunkWriter">Writer</param>
        /// <returns>Writer task</returns>
        public IWriterTask CreatWriterTask(int chunksCount, IChunkWriter chunkWriter)
        {
            return new FileWriterTask(chunksCount, chunkWriter);
        }
    }
}
