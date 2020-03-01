using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GZipTest.Dtos;
using GZipTest.Exceptions;
using GZipTest.Interfaces;

namespace GZipTest.IO
{
    /// <summary>
    /// Read data from file
    /// </summary>
    public class FileChunkReader : ISourceReader
    {
        private readonly string _fileName;

        public FileChunkReader(string fileName)
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
            try
            {
                using (var reader = new BinaryReader(File.Open(_fileName, FileMode.Open, FileAccess.Read, FileShare.Read)))
                {
                    reader.BaseStream.Seek(chunkReadInfo.Offset, SeekOrigin.Begin);

                    return reader.ReadBytes(chunkReadInfo.BytesCount);
                }
            }
            catch (Exception ex)
            {
                throw new ReaderException($"An error occurred while trying to read a data block from a file {_fileName}", ex);
            }
        }
    }
}