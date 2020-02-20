using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using GZipTest.Dtos;
using GZipTest.Interfaces;

namespace GZipTest
{
    public class FileWriterTask : IFileWriterTask, IDisposable
    {
        private readonly Thread _thread;
        private volatile Exception _exception;
        private readonly Dictionary<int, ChunkWriteInfo> _chunks = new Dictionary<int, ChunkWriteInfo>();
        private readonly IChunkWriter _chunkWriter;
        private readonly object _lockChunksObj = new object();
        private readonly string _fileName;
        private const int DummyId = -1;
        private readonly int _chunksCount;

        public FileWriterTask(string fileName, int chunksCount)
        {
            _fileName = fileName;
            _chunksCount = chunksCount;
            _chunkWriter = new ChunkWriter();
            _thread = new Thread(Consume) {IsBackground = true, Name = "Background worker (file writer)" };
            _thread.Start();
        }

        public void AddChunk(int id, ChunkWriteInfo chunk)
        {
            lock (_lockChunksObj)
            {
                _chunks.Add(id, chunk);
                Monitor.Pulse(_lockChunksObj);
            }
        }

        void Consume()
        {
            try
            {
                int id = 0;

                while (true)
                {
                    ChunkWriteInfo chunk;

                    lock (_lockChunksObj)
                    {
                        while (!_chunks.TryGetValue(id, out chunk))
                        {
                            if (_chunks.ContainsKey(DummyId) || id > _chunksCount - 1) return;

                            Monitor.Wait(_lockChunksObj);
                        }

                        _chunks.Remove(id);
                    }

                    _chunkWriter.WriteToFile(_fileName, chunk.Content);
                    id++;
                }
            }
            catch (Exception ex)
            {
                _exception = ex;
            }
        }

        public bool IsActive()
        {
            lock (_lockChunksObj)
            {
                return _chunks.Any() && (_thread.ThreadState & (ThreadState.Stopped | ThreadState.Unstarted)) == 0;
            }
        }

        public bool IsErrorExist(out Exception exception)
        {
            exception = null;

            if (_exception != null)
            {
                exception = _exception;
                return true;
            }

            return false;
        }

        public void Dispose()
        {
            AddChunk(DummyId, null);
            _thread.Join();
        }
    }
}