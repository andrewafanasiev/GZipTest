﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using GZipTest.Dtos;
using GZipTest.Helpers;
using GZipTest.Interfaces;

namespace GZipTest
{
    /// <summary>
    /// Task for write chunks in file
    /// </summary>
    public class FileWriterTask : IWriterTask, IDisposable
    {
        private readonly Thread _thread;
        private readonly Dictionary<int, ChunkWriteInfo> _chunks = new Dictionary<int, ChunkWriteInfo>();
        private readonly IChunkWriter _chunkWriter;
        private readonly IErrorLogs _errorLogs;
        private readonly object _lockChunksObj = new object();
        private const int DummyId = -1;
        private readonly int _chunksCount;

        public FileWriterTask(int chunksCount, IChunkWriter chunkWriter, IErrorLogs errorLogs)
        {
            _chunksCount = chunksCount;
            _chunkWriter = chunkWriter;
            _errorLogs = errorLogs;
            _thread = new Thread(Consume) {IsBackground = true, Name = "Background worker (file writer)" };
            _thread.Start();
        }

        /// <summary>
        /// Add chunk to write to file
        /// </summary>
        public void AddChunk(int id, ChunkWriteInfo chunk)
        {
            lock (_lockChunksObj)
            {
                _chunks.Add(id, chunk);
                Monitor.Pulse(_lockChunksObj);
            }
        }

        /// <summary>
        /// Chunks processing
        /// </summary>
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

                    _chunkWriter.WriteToFile(chunk.Content);
                    id++;
                }
            }
            catch (Exception ex)
            {
                _errorLogs.Add(ex);
            }
        }

        /// <summary>
        /// Is file writer task does any work
        /// </summary>
        /// <returns>Result of checking</returns>
        public bool IsActive()
        {
            lock (_lockChunksObj)
            {
                return _chunks.Any() || _thread.ThreadState.GetSimpleThreadState() == ThreadState.Running;
            }
        }

        public void Dispose()
        {
            if (_thread.ThreadState.GetSimpleThreadState() != ThreadState.Unstarted)
            {
                AddChunk(DummyId, null);
                _thread.Join();
            }
        }
    }
}