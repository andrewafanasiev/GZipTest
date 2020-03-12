using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GZipTest.Exceptions;
using GZipTest.Interfaces;

namespace GZipTest.IO
{
    /// <summary>
    /// Writing data to file
    /// </summary>
    public class FileChunkWriter : IChunkWriter
    {
        private readonly string _outFile;

        public FileChunkWriter(string outFile)
        {
            _outFile = outFile;
        }

        /// <summary>
        /// Writing data to file
        /// </summary>
        /// <param name="bytes">Data</param>
        public void WriteToFile(byte[] bytes)
        {
            try
            {
                using (BinaryWriter writer = new BinaryWriter(File.Open(_outFile, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None)))
                {
                    writer.BaseStream.Seek(0, SeekOrigin.End);
                    writer.Write(bytes);
                }
            }
            catch (Exception ex)
            {
                throw new WriterException($"An error occurred while trying to write data block to file {_outFile}", ex);
            }
        }
    }
}