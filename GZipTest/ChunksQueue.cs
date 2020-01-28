using System;
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
        private readonly List<Exception> _exceptions; //todo: volatile without lock?
        private readonly IGZipCompressor _compressor;
        private readonly IFileWriterTask _fileWriterTask;
        private readonly IFileReader _fileReader;
        private readonly Queue<Chunk> _chunks = new Queue<Chunk>();
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
                var thread = new Thread(Consume) {IsBackground = true, Name = $"Background worker: {i}"};

                _threads.Add(thread);
                thread.Start();
            }
        }

        public void EnqueueChunk(Chunk chunk)
        {
            bool lockTaken = false;

            try
            {
                Monitor.Enter(_lockQueueObj, ref lockTaken);

                _chunks.Enqueue(chunk);
                Monitor.PulseAll(_lockQueueObj);
            }
            finally
            {
                if(lockTaken) Monitor.Exit(_lockQueueObj);
            }
        }

        void Consume()
        {
            try
            {
                while (true)
                {
                    Chunk chunk;
                    bool lockQueueTaken = false;

                    try
                    {
                        Monitor.Enter(_lockQueueObj, ref lockQueueTaken);

                        while (!_chunks.Any()) Monitor.Wait(_lockQueueObj);
                        chunk = _chunks.Dequeue();
                    }
                    finally
                    {
                        if (lockQueueTaken) Monitor.Exit(_lockQueueObj);
                    }

                    if (chunk == null) return;

                    chunk.SetContentForRecording(_compressor.Execute(_fileReader.GetChunkBytes(chunk)));
                    _fileWriterTask.AddChunk(chunk.Id, chunk);
                }
            }
            catch (Exception ex)
            {
                bool lockExTaken = false;

                try
                {
                    Monitor.Enter(_lockExObj, ref lockExTaken);

                    //todo: volatile without lock?
                    _exceptions.Add(ex);
                }
                finally
                {
                    if (lockExTaken) Monitor.Exit(_lockExObj);
                }
            }
        }

        public bool IsActive()
        {
            bool lockTaken = false;

            try
            {
                Monitor.Enter(_lockQueueObj, ref lockTaken);

                return _chunks.Any() || !_threads.All(thread => (thread.ThreadState & ThreadState.WaitSleepJoin) != 0);
            }
            finally
            {
                if (lockTaken) Monitor.Exit(_lockQueueObj);
            }
        }

        public bool IsErrorExists(out List<Exception> exceptions)
        {
            exceptions = null;
            bool lockTaken = false;

            try
            {
                Monitor.Enter(_lockExObj, ref lockTaken);

                //todo: volatile without lock?
                if (_exceptions.Any())
                {
                    exceptions = _exceptions;
                    return true;
                }
                else
                {
                    return false;
                }
                
            }
            finally
            {
                if (lockTaken) Monitor.Exit(_lockExObj);
            }
        }

        public void Dispose()
        {
            _threads.ForEach(thread => EnqueueChunk(null));
            _threads.ForEach(thread => thread.Join());
        }
    }
}