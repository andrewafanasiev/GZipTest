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

            //1.
            //long fileLength = new FileInfo(inFile).Length;
            //long availableBytes = fileLength;
            //int offset = 0;
            //int id = 0;

            //while (availableBytes > 0)
            //{
            //    int bytesCount = availableBytes < chunkSize ? (int)availableBytes : chunkSize;

            //    chunks.Add(new ChunkReadInfo(id, offset, bytesCount));

            //    availableBytes -= chunkSize;
            //    offset += chunkSize;
            //    id++;
            //}

            //2.
            //int id = 0;

            //using (var compressedFile = new FileStream(inFile, FileMode.Open, FileAccess.Read))
            //{
            //    while (compressedFile.Position < compressedFile.Length)
            //    {
            //        var lengthBuffer = new byte[8];
            //        compressedFile.Read(lengthBuffer, 0, lengthBuffer.Length);
            //        int blockLength = BitConverter.ToInt32(lengthBuffer, 4);
            //        var compressedData = new byte[blockLength];
            //        //lengthBuffer.CopyTo(compressedData, 0);

            //        chunks.Add(new ChunkReadInfo(id, compressedFile.Position, blockLength));

            //        if(blockLength - 8 >= 0) compressedFile.Read(compressedData, 8, blockLength - 8);
            //        //int _dataSize = BitConverter.ToInt32(compressedData, blockLength - 4);
            //        //byte[] lastBuffer = new byte[_dataSize];

            //        id++;

            //        //compressedFile.Position += lengthBuffer.Length;
            //    }
            //}

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

            //        while (availableBytes > 0)
            //        {
            //            var curByte = reader.ReadByte();
            //            availableBytes--;

            //            if (curByte == gzipHeader[0])
            //            {
            //                foreach (var el in gzipHeader)
            //                {
                                
            //                }
            //            }
            //        }

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

            //        long startPosition = 0;
            //        int blockLength = gzipBlock.ToArray().Length;

            //        if (id > 0)
            //        {
            //            startPosition = fileLength - availableBytes - gzipHeader.Length - blockLength;

            //            if (startPosition + gzipHeader.Length + blockLength == fileLength) // The last gzip block in a file.
            //                startPosition += gzipHeader.Length;
            //        }

            //        chunks.Add(new ChunkReadInfo(id, startPosition, blockLength));
            //        id++;
            //    }
            //}

            //3.
            using (var reader = new BinaryReader(File.Open(inFile, FileMode.Open, FileAccess.Read)))
            {
                var fileLength = new FileInfo(inFile).Length;
                var gzipHeader = reader.ReadBytes(DefaultHeader.Length);
                var availableBytes = fileLength - gzipHeader.Length;
                var id = 0;

                while (availableBytes > 0)
                {
                    var gzipBlock = new List<byte>(chunkSize);
                    gzipBlock.AddRange(gzipHeader);

                    var gzipHeaderMatchsCount = 0;
                    while (availableBytes > 0)
                    {
                        var curByte = reader.ReadByte();
                        gzipBlock.Add(curByte);
                        availableBytes--;

                        if (curByte == gzipHeader[gzipHeaderMatchsCount])
                        {
                            gzipHeaderMatchsCount++;
                            if (gzipHeaderMatchsCount != gzipHeader.Length)
                                continue;

                            gzipBlock.RemoveRange(gzipBlock.Count - gzipHeader.Length, gzipHeader.Length); // Remove gzip header of the next block from a rear of this one.
                            break;
                        }

                        gzipHeaderMatchsCount = 0;
                    }

                    var gzipBlockStartPosition = 0L;
                    var gzipBlockLength = gzipBlock.ToArray().Length;
                    if (id > 0)
                    {
                        gzipBlockStartPosition = fileLength - availableBytes - gzipHeader.Length - gzipBlockLength;
                        if (gzipBlockStartPosition + gzipHeader.Length + gzipBlockLength == fileLength) // The last gzip block in a file.
                            gzipBlockStartPosition += gzipHeader.Length;
                    }

                    chunks.Add(new ChunkReadInfo(id, gzipBlockStartPosition, gzipBlockLength));

                    id++;
                }
            }

            //4.
            //int id = 0;

            //using (var reader = new FileStream(inFile, FileMode.Open))
            //{
            //    while (reader.Position < reader.Length)
            //    {
            //        byte[] blockLenghtBytes = new byte[chunkSize];
            //        reader.Read(blockLenghtBytes, 0, blockLenghtBytes.Length);
            //        int blockLength = BitConverter.ToInt32(blockLenghtBytes, 4);

            //        var buffer = new byte[blockLength];
            //        var position = reader.Position;
            //        reader.Read(buffer, 0, blockLength);

            //        chunks.Add(new ChunkReadInfo(id, position, blockLength));
            //        id++;
            //    }
            //}

            //int id = 0;

            //using (var reader = new FileStream(inFile, FileMode.Open))
            //{
            //    while (reader.Position < reader.Length)
            //    {
            //        var lengthBuffer = new byte[8];
            //        reader.Read(lengthBuffer, 0, lengthBuffer.Length);
            //        var blockLength = BitConverter.ToInt32(lengthBuffer, 4);

            //        if(blockLength == 0) continue;

            //        var compressedData = new byte[blockLength];
            //        lengthBuffer.CopyTo(compressedData, 0);

            //        var position = reader.Position;
            //        reader.Read(compressedData, 8, blockLength - 8);

            //        //int dataSize = BitConverter.ToInt32(compressedData, blockLength - 4);
            //        //byte[] lastBuffer = new byte[dataSize];

            //        //int dataSize = BitConverter.ToInt32(compressedData, blockLength - 4);
            //        //byte[] lastBuffer = new byte[dataSize];
            //        //var _block = new CompressedData { Buffer = compressedData, UncompressedSize = dataSize };
            //        chunks.Add(new ChunkReadInfo(id, position, blockLength - 8));
            //        id++;
            //    }

            //    reader.Close();
            //}

            return new ChunksInfo(chunks);
        }
    }
}