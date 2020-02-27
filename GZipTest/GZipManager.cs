﻿using GZipTest.Dtos;
using GZipTest.Interfaces;

namespace GZipTest
{
    /// <summary>
    /// Manager performing file compression or decompression
    /// </summary>
    public class GZipManager : IGZipManager
    {
        private readonly ICompressorFactory _compressorFactory;
        private readonly IFileSplitterFactory _fileSplitterFactory;
        private readonly string _inFile;
        private readonly ITaskFactory _taskFactory;
        private readonly ISourceReader _sourceReader;
        private readonly IChunkWriter _chunkWriter;

        public GZipManager(string inFile, ISourceReader reader, IChunkWriter chunkWriter,
            IFileSplitterFactory fileSplitterFactory, ICompressorFactory compressorFactory, ITaskFactory taskFactory)
        {
            _chunkWriter = chunkWriter;
            _sourceReader = reader;
            _taskFactory = taskFactory;
            _inFile = inFile;
            _fileSplitterFactory = fileSplitterFactory;
            _compressorFactory = compressorFactory;
        }

        /// <summary>
        /// Perform file compression or decompression action
        /// </summary>
        /// <param name="actionType">Action name. Possible values: compress, decompress</param>
        /// <param name="workersCount">Number of threads</param>
        /// <param name="chunkSize">Chunk size in bytes</param>
        /// <returns>Operation result</returns>
        public bool Execute(string actionType, int workersCount, int chunkSize)
        {
            var chunkInfos = _fileSplitterFactory.Create(actionType).GetChunks(_inFile, chunkSize);

            using (var fileWriterTask = _taskFactory.CreatWriterTask(_chunkWriter, chunkInfos.ChunksCount))
            using (var chunksQueue = _taskFactory.CreateChunksQueue(workersCount, _sourceReader, _compressorFactory.Create(actionType), fileWriterTask))
            {
                fileWriterTask.Start();
                chunksQueue.Start();

                foreach (var chunkInfo in chunkInfos.Chunks)
                {
                    chunksQueue.EnqueueChunk(new ChunkReadInfo(chunkInfo.Id, chunkInfo.Offset, chunkInfo.BytesCount));
                }

                while (true)
                {
                    if (!IsErrorExist(chunksQueue, fileWriterTask))
                    {
                        if (!IsActiveOp(chunksQueue, fileWriterTask))
                        {
                            return !IsErrorExist(chunksQueue, fileWriterTask);
                        }

                        continue;
                    }

                    return false;
                }
            }
        }

        /// <summary>
        /// Is operation active
        /// </summary>
        /// <returns>Result of checking</returns>
        public bool IsActiveOp(IChunksQueue chunksQueue, IWriterTask writerTask)
        {
            return chunksQueue.IsActive() || writerTask.IsActive();
        }

        /// <summary>
        /// An error occurred during the execution
        /// </summary>
        /// <returns>Result of checking</returns>
        public bool IsErrorExist(IChunksQueue chunksQueue, IWriterTask writerTask)
        {
            return chunksQueue.IsErrorExist() || writerTask.IsErrorExist();
        }
    }
}