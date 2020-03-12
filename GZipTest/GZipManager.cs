using System;
using System.Collections.Generic;
using System.Linq;
using GZipTest.Dtos;
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
        private readonly IErrorLogs _errorLogs;

        public GZipManager(string inFile, ISourceReader reader, IChunkWriter chunkWriter,
            IFileSplitterFactory fileSplitterFactory, ICompressorFactory compressorFactory, ITaskFactory taskFactory, IErrorLogs errorLogs)
        {
            _chunkWriter = chunkWriter;
            _sourceReader = reader;
            _taskFactory = taskFactory;
            _inFile = inFile;
            _fileSplitterFactory = fileSplitterFactory;
            _compressorFactory = compressorFactory;
            _errorLogs = errorLogs;
        }

        /// <summary>
        /// Perform file compression or decompression action
        /// </summary>
        /// <param name="actionType">Action name. Possible values: compress, decompress</param>
        /// <param name="workersCount">Number of threads</param>
        /// <param name="chunkSize">Chunk size in bytes</param>
        /// <param name="errors">Exceptions</param>
        /// <returns>Operation result</returns>
        public bool Execute(string actionType, int workersCount, int chunkSize, out List<Exception> errors)
        {
            ChunksInfo chunkInfos = _fileSplitterFactory.Create(actionType).GetChunks(_inFile, chunkSize);

            using (IWriterTask fileWriterTask = _taskFactory.CreatWriterTask(chunkInfos.ChunksCount, _chunkWriter, _errorLogs))
            using (IChunksReader chunksReader = _taskFactory.CreateChunksReader(workersCount, _sourceReader,
                _compressorFactory.Create(actionType), fileWriterTask, _errorLogs))
            {
                foreach (ChunkReadInfo chunkInfo in chunkInfos.Chunks)
                {
                    chunksReader.EnqueueChunk(new ChunkReadInfo(chunkInfo.Id, chunkInfo.Offset, chunkInfo.BytesCount));
                }

                while (true)
                {
                    if (!_errorLogs.IsErrorExist(out List<Exception> exceptions))
                    {
                        if (!IsActiveOp(chunksReader, fileWriterTask))
                        {
                            return !_errorLogs.IsErrorExist(out errors);
                        }

                        continue;
                    }

                    errors = exceptions;
                    return false;
                }
            }
        }

        /// <summary>
        /// Is operation active
        /// </summary>
        /// <returns>Result of checking</returns>
        public bool IsActiveOp(IChunksReader chunksReader, IWriterTask writerTask)
        {
            return chunksReader.IsActive() || writerTask.IsActive();
        }
    }
}