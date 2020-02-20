using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        private readonly string _outFile;

        public GZipManager(string inFile, string outFile)
        {
            _inFile = inFile;
            _outFile = outFile;
            _fileSplitterFactory = new FileSplitterFactory();
            _compressorFactory = new CompressorFactory();
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

            using (var fileWriterTask = new FileWriterTask(_outFile, chunkInfos.ChunksCount))
            using (var chunksQueue = new ChunksQueue(_inFile, workersCount, _compressorFactory.Create(actionType), fileWriterTask))
            {
                foreach (var chunkInfo in chunkInfos.Chunks)
                {
                    chunksQueue.EnqueueChunk(new ChunkReadInfo(chunkInfo.Id, chunkInfo.Offset, chunkInfo.BytesCount));
                }

                while (true)
                {
                    if (!IsErrorExist(chunksQueue, fileWriterTask))
                    {
                        if (IsActiveOp(chunksQueue, fileWriterTask))
                        {
                            if (!IsErrorExist(chunksQueue, fileWriterTask)) return true;

                            return false;
                        }
                    }

                    return false;
                }
            }
        }

        private bool IsActiveOp(IChunksQueue chunksQueue, IFileWriterTask fileWriterTask)
        {
            return chunksQueue.IsActive() || fileWriterTask.IsActive();
        }

        private bool IsErrorExist(IChunksQueue chunksQueue, IFileWriterTask fileWriterTask)
        {
            return chunksQueue.IsErrorExist() || fileWriterTask.IsErrorExist();
        }
    }
}