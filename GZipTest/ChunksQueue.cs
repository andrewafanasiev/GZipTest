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
        private readonly List<Thread> _threads = new List<Thread>();
        private readonly IGZipCompressor _compressor;
        private readonly IFileWriterTask _fileWriterTask;
        private readonly IFileReader _fileReader;
        private readonly Queue<ChunkInfo> _chunkInfos = new Queue<ChunkInfo>();
        private readonly object _lockObj = new object();

        public ChunksQueue(string inFile, int workersCount, IGZipCompressor compressor, IFileWriterTask fileWriterTask)
        {
            _compressor = compressor;
            _fileReader = new FileReader(inFile);
            _fileWriterTask = fileWriterTask;

            for (int i = 0; i < workersCount; ++i)
            {
                var thread = new Thread(Consume) {IsBackground = true, Name = $"Background worker: {i}"};

                _threads.Add(thread);
                thread.Start();
            }
        }

        public void EnqueueChunk(ChunkInfo chunkInfo)
        {
            bool lockTaken = false;

            try
            {
                Monitor.Enter(_lockObj, ref lockTaken);

                _chunkInfos.Enqueue(chunkInfo);
                Monitor.PulseAll(_lockObj);
            }
            finally
            {
                if(lockTaken) Monitor.Exit(_lockObj);
            }
        }

        void Consume()
        {
            try
            {
                while (true)
                {
                    ChunkInfo chunkInfo;
                    bool lockTaken = false;

                    try
                    {
                        Monitor.Enter(_lockObj, ref lockTaken);

                        while (!_chunkInfos.Any()) Monitor.Wait(_lockObj);
                        chunkInfo = _chunkInfos.Dequeue();
                    }
                    finally
                    {
                        if (lockTaken) Monitor.Exit(_lockObj);
                    }

                    if (chunkInfo == null) return;

                    _fileWriterTask.AddChunk(chunkInfo.Id, _compressor.Execute(_fileReader.GetChunkBytes(chunkInfo)));
                }
            }
            catch (Exception ex)
            {
                //todo: handle exception
            }
        }

        public bool IsActive()
        {
            return _chunkInfos.Any() || _threads.All(thread => (thread.ThreadState & ThreadState.WaitSleepJoin) != 0);
        }

        public void Dispose()
        {
            _threads.ForEach(thread => EnqueueChunk(null));
            _threads.ForEach(thread => thread.Join());
        }
    }
}