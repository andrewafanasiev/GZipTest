using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GZipTest.Interfaces;

namespace GZipTest
{
    public class ChunkWriter : IChunkWriter
    {
        public void WriteToFile(string fileName, byte[] bytes)
        {
            using (var writer = new BinaryWriter(File.Open(fileName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None)))
            {
                writer.BaseStream.Seek(0, SeekOrigin.End);
                writer.Write(bytes);
            }
        }
    }
}
