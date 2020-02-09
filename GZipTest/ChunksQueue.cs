﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using GZipTest.Dtos;
using GZipTest.Interfaces;

namespace GZipTest
{
    public class ChunksQueue : IChunksQueue, IDisposable
    {
        private readonly List<Thread> _threads;
        private readonly List<Exception> _exceptions;
        private readonly IGZipCompressor _compressor;
        private readonly IFileWriterTask _fileWriterTask;
        private readonly IFileReader _fileReader;
        private readonly Queue<ChunkReadInfo> _chunks = new Queue<ChunkReadInfo>();
        private readonly object _lockQueueObj = new object();
        private readonly object _lockExObj = new object();

        public ChunksQueue(string inFile, int workersCount, IGZipCompressor compressor, IFileWriterTask fileWriterTask)
        {
            _compressor = compressor;
            _fileReader = new FileReader(inFile);
            _fileWriterTask = fileWriterTask;
            _exceptions = new List<Exception>();
            _threads = new List<Thread>();

            for (int i = 0; i < workersCount; ++i)
            {
                var thread = new Thread(Consume) {IsBackground = true, Name = $"Background worker (chunks queue): {i}"};

                _threads.Add(thread);
                thread.Start();
            }
        }

        public void EnqueueChunk(ChunkReadInfo chunkReadInfo)
        {
            lock (_lockQueueObj)
            {
                _chunks.Enqueue(chunkReadInfo);
                Monitor.PulseAll(_lockQueueObj);
            }
        }

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

                    var chunkWriteInfo = new ChunkWriteInfo(chunkReadInfo.Id, _compressor.Execute(_fileReader.GetChunkBytes(chunkReadInfo)));
                    _fileWriterTask.AddChunk(chunkReadInfo.Id, chunkWriteInfo);
                }
            }
            catch (Exception ex)
            {
                lock (_lockExObj)
                {
                    _exceptions.Add(ex);
                }
            }
        }

        public bool IsActive()
        {
            lock(_lockQueueObj)
            { 
                return _chunks.Any() || !_threads.All(thread => (thread.ThreadState & ThreadState.WaitSleepJoin) != 0);
            }
        }

        public bool IsErrorExist(out List<Exception> exceptions)
        {
            exceptions = null;

            lock (_lockExObj)
            {
                if (_exceptions.Any())
                {
                    exceptions = _exceptions;
                    return true;
                }

                return false;
            }
        }

        public void Dispose()
        {
            _threads.ForEach(thread => EnqueueChunk(null));
            _threads.ForEach(thread => thread.Join());
        }
    }
}