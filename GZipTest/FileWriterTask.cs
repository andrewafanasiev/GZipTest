﻿using System;
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
        private readonly Dictionary<int, Chunk> _chunks = new Dictionary<int, Chunk>();
        private readonly object _lockObj = new object();
        private readonly string _fileName;
        private const int DummyId = -1;
        private readonly int _maxChunkId;

        public FileWriterTask(string fileName, int maxChunkId)
        {
            _fileName = fileName;
            _maxChunkId = maxChunkId;
            _exception = null;
            _thread = new Thread(Consume) {IsBackground = true, Name = $"Background worker" };
            _thread.Start();
        }

        public void AddChunk(int id, Chunk chunk)
        {
            bool lockTaken = false;

            try
            {
                Monitor.Enter(_lockObj, ref lockTaken);

                _chunks.Add(id, chunk);
                Monitor.Pulse(_lockObj);
            }
            finally
            {
                if (lockTaken) Monitor.Exit(_lockObj);
            }
        }

        void Consume()
        {
            try
            {
                int id = 0;

                while (true)
                {
                    Chunk chunk;
                    bool lockTaken = false;

                    try
                    {
                        Monitor.Enter(_lockObj, ref lockTaken);

                        while (!_chunks.TryGetValue(id, out chunk))
                        {
                            if (_chunks.ContainsKey(DummyId)) return;

                            Monitor.Wait(_lockObj);
                        }
                    }
                    finally
                    {
                        if (lockTaken) Monitor.Exit(_lockObj);
                    }

                    WriteChunkToFile(chunk.Content);
                    chunk.IsWriteToFile = true;
                    id++;
                }
            }
            catch (Exception ex)
            {
                _exception = ex;
            }
        }

        void WriteChunkToFile(byte[] bytes)
        {
            using (var writer = new BinaryWriter(File.Open(_fileName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None)))
            {
                writer.BaseStream.Seek(0, SeekOrigin.End);
                writer.Write(bytes);
            }
        }

        public bool IsActive()
        {
            bool lockTaken = false;

            try
            {
                Monitor.Enter(_lockObj, ref lockTaken);

                if (_chunks.ContainsKey(_maxChunkId) && _chunks[_maxChunkId].IsWriteToFile)
                {
                    return false;
                }

                return true;
            }
            finally
            {
                if (lockTaken) Monitor.Exit(_lockObj);
            }
        }

        public bool IsErrorExists(out Exception exception)
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