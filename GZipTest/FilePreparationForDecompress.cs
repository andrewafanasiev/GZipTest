using System;
using System.Collections.Generic;
using System.IO;
using GZipTest.Dtos;
using GZipTest.Interfaces;

namespace GZipTest
{
    public class FilePreparationForDecompress : IFilePreparation
    {
        public static readonly byte[] DefaultHeader = { 0x1f, 0x8b, 0x08, 0x00, 0x00, 0x00, 0x00, 0x00, 0x04, 0x00 };

        public ChunksInfo GetChunks(string inFile, int chunkSize)
        {
            var chunks = new List<ChunkReadInfo>();

            using (var reader = new BinaryReader(File.Open(inFile, FileMode.Open, FileAccess.Read)))
            {
                long fileLength = new FileInfo(inFile).Length;
                byte[] header = reader.ReadBytes(DefaultHeader.Length);
                long availableBytes = fileLength - header.Length;
                var id = 0;

                while (availableBytes > 0)
                {
                    List<byte> chunk = GetChunk(reader, header, chunkSize, ref availableBytes);
                    int chunkLength = chunk.Count;
                    long startPosition = 0;

                    if (id > 0)
                    {
                        startPosition = fileLength - availableBytes - header.Length - chunkLength;

                        if (startPosition + header.Length + chunkLength == fileLength) startPosition += header.Length;
                    }

                    chunks.Add(new ChunkReadInfo(id, startPosition, chunkLength));
                    id++;
                }
            }

            //3.
            //using (var reader = new BinaryReader(File.Open(inFile, FileMode.Open, FileAccess.Read)))
            //{
            //    var fileLength = new FileInfo(inFile).Length;
            //    var gzipHeader = reader.ReadBytes(DefaultHeader.Length);
            //    var availableBytes = fileLength - gzipHeader.Length;
            //    var id = 0;

            //    while (availableBytes > 0)
            //    {
            //        var gzipBlock = new List<byte>(chunkSize);
            //        gzipBlock.AddRange(gzipHeader);

            //        var gzipHeaderMatchsCount = 0;
            //        while (availableBytes > 0)
            //        {
            //            var curByte = reader.ReadByte();
            //            gzipBlock.Add(curByte);
            //            availableBytes--;

            //            if (curByte == gzipHeader[gzipHeaderMatchsCount])
            //            {
            //                gzipHeaderMatchsCount++;
            //                if (gzipHeaderMatchsCount != gzipHeader.Length)
            //                    continue;

            //                gzipBlock.RemoveRange(gzipBlock.Count - gzipHeader.Length, gzipHeader.Length); // Remove gzip header of the next block from a rear of this one.
            //                break;
            //            }

            //            gzipHeaderMatchsCount = 0;
            //        }

            //        var gzipBlockStartPosition = 0L;
            //        var gzipBlockLength = gzipBlock.ToArray().Length;
            //        if (id > 0)
            //        {
            //            gzipBlockStartPosition = fileLength - availableBytes - gzipHeader.Length - gzipBlockLength;
            //            if (gzipBlockStartPosition + gzipHeader.Length + gzipBlockLength == fileLength) // The last gzip block in a file.
            //                gzipBlockStartPosition += gzipHeader.Length;
            //        }

            //        chunks.Add(new ChunkReadInfo(id, gzipBlockStartPosition, gzipBlockLength));

            //        id++;
            //    }
            //}

            return new ChunksInfo(chunks);
        }

        private List<byte> GetChunk(BinaryReader reader, byte[] header, int chunkSize, ref long availableBytes)
        {
            var headerMatchesCount = 0;
            var chunk = new List<byte>(chunkSize);
            chunk.AddRange(header);

            while (availableBytes > 0)
            {
                var curByte = reader.ReadByte();
                chunk.Add(curByte);
                availableBytes--;

                if (curByte == header[headerMatchesCount])
                {
                    headerMatchesCount++;
                    if (headerMatchesCount != header.Length) continue;

                    chunk.RemoveRange(chunk.Count - header.Length, header.Length);
                    break;
                }

                headerMatchesCount = 0;
            }

            return chunk;
        }
    }
}