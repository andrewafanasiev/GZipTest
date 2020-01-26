using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GZipTest.Interfaces;

namespace GZipTest
{
    public class FileReader : IFileReader
    {
        private readonly string _fileName;

        public FileReader(string fileName)
        {
            _fileName = fileName;
        }

        public byte[] GetChunkBytes(ChunkInfo chunkInfo)
        {
            using (var reader = new BinaryReader(File.Open(_fileName, FileMode.Open, FileAccess.Read, FileShare.Read)))
            {
                reader.BaseStream.Seek(chunkInfo.Offset, SeekOrigin.Begin);

                return reader.ReadBytes(chunkInfo.BytesCount);
            }
        }
    }
}
