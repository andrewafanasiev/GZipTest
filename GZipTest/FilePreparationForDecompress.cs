using System;
using System.Collections.Generic;
using GZipTest.Dtos;
using GZipTest.Interfaces;

namespace GZipTest
{
    public class FilePreparationForDecompress : IFilePreparation
    {
        public ChunksInfo GetChunks(string inFile, int chunkSize)
        {
            throw new NotImplementedException();
        }
    }
}