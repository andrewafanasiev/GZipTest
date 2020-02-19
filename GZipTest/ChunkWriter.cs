using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GZipTest.Interfaces;

namespace GZipTest
{
    /// <summary>
    /// Writing data to file
    /// </summary>
    public class ChunkWriter : IChunkWriter
    {
        /// <summary>
        /// Writing data to file
        /// </summary>
        /// <param name="fileName">Path to file</param>
        /// <param name="bytes">Data</param>
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
