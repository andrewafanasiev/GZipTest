using System.Collections.Generic;
using System.IO;
using GZipTest.Dtos;
using GZipTest.Interfaces;

namespace GZipTest
{
    public class FilePreparationForCompress : IFilePreparation
    {
        public ChunksInfo GetChunks(string inFile, int chunkSize)
        {
            var chunks = new List<ChunkReadInfo>();
            long fileLength = new FileInfo(inFile).Length;
            long availableBytes = fileLength;
            int offset = 0;
            int id = 0;

            while (availableBytes > 0)
            {
                int bytesCount = availableBytes < chunkSize ? (int) availableBytes : chunkSize;

                chunks.Add(new ChunkReadInfo(id, offset, bytesCount));

                availableBytes -= chunkSize;
                offset += chunkSize;
                id++;
            }

            return new ChunksInfo(chunks);
        }
    }
}