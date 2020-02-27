using System;
using System.Collections.Generic;
using System.Text;
using GZipTest.Interfaces;

namespace GZipTest
{
    public class TaskFactory : ITaskFactory
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="workersCount"></param>
        /// <param name="reader"></param>
        /// <param name="compressor"></param>
        /// <param name="writerTask"></param>
        /// <returns></returns>
        public ChunksQueue CreateChunksQueue(int workersCount, ISourceReader reader, IGZipCompressor compressor, IWriterTask writerTask)
        {
            return new ChunksQueue(workersCount, reader, compressor, writerTask);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="chunkWriter"></param>
        /// <param name="chunksCount"></param>
        /// <returns></returns>
        public FileWriterTask CreatWriterTask(IChunkWriter chunkWriter, int chunksCount)
        {
            return new FileWriterTask(chunkWriter, chunksCount);
        }
    }
}
