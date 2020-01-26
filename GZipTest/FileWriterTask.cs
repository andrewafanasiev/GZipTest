using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using GZipTest.Interfaces;

namespace GZipTest
{
    public class FileWriterTask : IFileWriterTask, IDisposable
    {
        private readonly Thread _thread;
        private readonly Dictionary<int, byte[]> _chunks = new Dictionary<int, byte[]>();
        private readonly object _lockObj = new object();
        private readonly string _fileName;
        private const int DummyId = -1;

        public FileWriterTask(string fileName)
        {
            _fileName = fileName;
            _thread = new Thread(Consume) {IsBackground = true, Name = $"Background worker" };
            _thread.Start();
        }

        public void AddChunk(int id, byte[] bytes)
        {
            bool lockTaken = false;

            try
            {
                Monitor.Enter(_lockObj, ref lockTaken);

                _chunks.Add(id, bytes);
                Monitor.Pulse(_lockObj);
            }
            finally
            {
                if (lockTaken) Monitor.Exit(_lockObj);
            }
        }

        void Consume()
        {
            bool lockTaken = false;
            int id = 0;

            while (true)
            {
                byte[] chunk;

                try
                {
                    Monitor.Enter(_lockObj, ref lockTaken);

                    while (!_chunks.TryGetValue(id, out chunk))
                    {
                        if(_chunks.ContainsKey(DummyId)) return;

                        Monitor.Wait(_lockObj);
                    }
                }
                finally
                {
                    if(lockTaken) Monitor.Exit(_lockObj);
                }

                WriteChunkToFile(chunk);
                id++;
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
            return !_chunks.Any() && (_thread.ThreadState & ThreadState.WaitSleepJoin) == 0;
        }

        public void Dispose()
        {
            AddChunk(DummyId, null);
            _thread.Join();
        }
    }
}