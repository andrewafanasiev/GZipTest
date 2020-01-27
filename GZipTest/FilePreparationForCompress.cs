using System.Collections.Generic;
using System.IO;
using GZipTest.Dtos;
using GZipTest.Interfaces;

namespace GZipTest
{
    public class FilePreparationForCompress : IFilePreparation
    {
        public List<ChunkInfo> GetChunkInfos(string inFile)
        {
            var chunkInfos = new List<ChunkInfo>();
            long fileLength = new FileInfo(inFile).Length;
            long availableBytes = fileLength;
            int blockSize = 4096 * 1024;
            int offset = 0;
            int id = 0;

            while (availableBytes > 0)
            {
                int bytesCount = availableBytes < blockSize ? (int) availableBytes : blockSize;

                chunkInfos.Add(new ChunkInfo(id, offset, bytesCount));

                availableBytes -= blockSize;
                offset += blockSize;
            }

            return chunkInfos;
        }
    }
}