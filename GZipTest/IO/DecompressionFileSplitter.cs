using System;
using System.Collections.Generic;
using System.IO;
using GZipTest.Dtos;
using GZipTest.Interfaces;

namespace GZipTest.IO
{
    public class DecompressionFileSplitter : IFileSplitter
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