using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using GZipTest.Dtos;
using GZipTest.Interfaces;

namespace GZipTest
{
    /// <summary>
    /// Queue for parallel processing of chunks
    /// </summary>
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

        /// <summary>
        /// Is queue does any work
        /// </summary>
        /// <returns>Result of checking</returns>
        public bool IsActive()
        {
            lock(_lockQueueObj)
            { 
                return _chunks.Any() || !_threads.All(thread => (thread.ThreadState & ThreadState.WaitSleepJoin) != 0);
            }
        }

        /// <summary>
        /// Errors occurred while the queue was running
        /// </summary>
        /// <returns>Result of checking</returns>
        public bool IsErrorExist()
        {
            lock (_lockExObj)
            {
                if (_exceptions.Any())
                {
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