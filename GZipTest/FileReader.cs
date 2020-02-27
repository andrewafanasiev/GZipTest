using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GZipTest.Dtos;
using GZipTest.Interfaces;

namespace GZipTest
{
    /// <summary>
    /// Read data from file
    /// </summary>
    public class FileReader : ISourceReader
    {
        private readonly string _fileName;

        public FileReader(string fileName)
        {
            _fileName = fileName;
        }

        /// <summary>
        /// Get chunk content from file
        /// </summary>
        /// <param name="chunkReadInfo">Info about chunk</param>
        /// <returns>Chunk content</returns>
        public byte[] GetChunkBytes(ChunkReadInfo chunkReadInfo)
        {
            using (var reader = new BinaryReader(File.Open(_fileName, FileMode.Open, FileAccess.Read, FileShare.Read)))
            {
                reader.BaseStream.Seek(chunkReadInfo.Offset, SeekOrigin.Begin);

                return reader.ReadBytes(chunkReadInfo.BytesCount);
            }
        }
    }
}
