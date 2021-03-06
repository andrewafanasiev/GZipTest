﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using GZipTest.Dtos;
using GZipTest.Helpers;
using GZipTest.Interfaces;

namespace GZipTest
{
    /// <summary>
    /// Queue for parallel processing of chunks
    /// </summary>
    public class ChunksReader : IChunksReader, IDisposable
    {
        private readonly List<Thread> _threads;
        private readonly IGZipCompressor _compressor;
        private readonly IWriterTask _writerTask;
        private readonly ISourceReader _fileReader;
        private readonly IErrorLogs _errorLogs;
        private readonly Queue<ChunkReadInfo> _chunks = new Queue<ChunkReadInfo>();
        private readonly object _lockQueueObj = new object();

        public ChunksReader(int workersCount, ISourceReader reader, IGZipCompressor compressor, IWriterTask writerTask, IErrorLogs errorLogs)
        {
            _compressor = compressor;
            _fileReader = reader;
            _writerTask = writerTask;
            _errorLogs = errorLogs;
            _threads = new List<Thread>();

            for (int i = 0; i < workersCount; ++i)
            {
                Thread thread = new Thread(Consume) {IsBackground = true, Name = $"Background worker (chunks queue): {i}"};

                _threads.Add(thread);
                thread.Start();
            }
        }

        /// <summary>
        /// Add a chunk to the queue
        /// </summary>
        /// <param name="chunkReadInfo">Chunk information for reading data from file</param>
        public void EnqueueChunk(ChunkReadInfo chunkReadInfo)
        {
            lock (_lockQueueObj)
            {
                _chunks.Enqueue(chunkReadInfo);
                Monitor.PulseAll(_lockQueueObj);
            }
        }

        /// <summary>
        /// Chunks processing
        /// </summary>
        void Consume()
        {
            try
            {
                while (true)
                {
                    ChunkReadInfo chunkReadInfo;

                    lock (_lockQueueObj)
                    {
                        while (!_chunks.Any()) Monitor.Wait(_lockQueueObj);
                        chunkReadInfo = _chunks.Dequeue();
                    }

                    if (chunkReadInfo == null) return;

                    ChunkWriteInfo chunkWriteInfo = new ChunkWriteInfo(chunkReadInfo.Id, _compressor.Execute(_fileReader.GetChunkBytes(chunkReadInfo)));
                    _writerTask.AddChunk(chunkReadInfo.Id, chunkWriteInfo);
                }
            }
            catch (Exception ex)
            {
                _errorLogs.Add(ex);
            }
        }

        /// <summary>
        /// Is queue does any work
        /// </summary>
        /// <returns>Result of checking</returns>
        public bool IsActive()
        {
            lock(_lockQueueObj)
            {
                return _chunks.Any() || _threads.Any(thread =>
                           thread.ThreadState.GetSimpleThreadState() == ThreadState.Running);
            }
        }

        public void Dispose()
        {
            _threads.ForEach(thread => EnqueueChunk(null));
            _threads.ForEach(thread =>
            {
                if (thread.ThreadState.GetSimpleThreadState() != ThreadState.Unstarted) thread.Join();
            });
        }
    }
}