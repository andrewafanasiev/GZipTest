using System;
using System.Collections.Generic;
using System.Text;

namespace GZipTest.Interfaces
{
    public interface ITaskFactory
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="workersCount"></param>
        /// <param name="reader"></param>
        /// <param name="compressor"></param>
        /// <param name="writerTask"></param>
        /// <returns></returns>
        ChunksQueue CreateChunksQueue(int workersCount, ISourceReader reader, IGZipCompressor compressor,
            IWriterTask writerTask);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="chunkWriter"></param>
        /// <param name="chunksCount"></param>
        /// <returns></returns>
        FileWriterTask CreatWriterTask(IChunkWriter chunkWriter, int chunksCount);
    }
}
