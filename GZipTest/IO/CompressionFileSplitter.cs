using System;
using System.Collections.Generic;
using System.IO;
using GZipTest.Dtos;
using GZipTest.Exceptions;
using GZipTest.Interfaces;

namespace GZipTest.IO
{
    public class CompressionFileSplitter : IFileSplitter
    {
        /// <summary>
        /// Split file to chunks
        /// </summary>
        /// <param name="inFile">Input file</param>
        /// <param name="chunkSize">Chunk size</param>
        /// <returns>Split result</returns>
        public ChunksInfo GetChunks(string inFile, int chunkSize)
        {
            try
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
            catch (Exception ex)
            {
                throw new SplitterException($"Errors occurred while splitting the file {inFile} for compression", ex);
            }
        }
    }
}