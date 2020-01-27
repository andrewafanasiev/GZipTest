using System;
using System.Collections.Generic;
using GZipTest.Dtos;
using GZipTest.Interfaces;

namespace GZipTest
{
    public class FilePreparationForDecompress : IFilePreparation
    {
        public List<ChunkInfo> GetChunkInfos(string inFile)
        {
            throw new NotImplementedException();
        }
    }
}