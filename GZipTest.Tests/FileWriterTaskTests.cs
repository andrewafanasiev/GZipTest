﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using GZipTest.Dtos;
using GZipTest.Exceptions;
using GZipTest.Interfaces;
using Moq;
using NUnit.Framework;

namespace GZipTest.Tests
{
    [TestFixture]
    public class FileWriterTaskTests
    {
        private const int ChunksCount = 10;

        [Test]
        public void WriterTaskIsActiveIfNotAllChunksAreProcessed()
        {
            using (FileWriterTask writerTask = new FileWriterTask(ChunksCount, It.IsAny<IChunkWriter>(), It.IsAny<IErrorLogs>()))
            {
                writerTask.AddChunk(1, new ChunkWriteInfo(1, It.IsAny<byte[]>()));

                Thread.Sleep(100);

                Assert.IsTrue(writerTask.IsActive());
            }
        }

        [Test]
        public void SequentialChunkProcessing()
        {
            Mock<IChunkWriter> chunkWriterMock = new Mock<IChunkWriter>();
            chunkWriterMock.Setup(x => x.WriteToFile(It.IsAny<byte[]>()));

            using (FileWriterTask writerTask = new FileWriterTask(ChunksCount, chunkWriterMock.Object, It.IsAny<IErrorLogs>()))
            {
                writerTask.AddChunk(2, new ChunkWriteInfo(2, It.IsAny<byte[]>()));
                writerTask.AddChunk(1, new ChunkWriteInfo(1, It.IsAny<byte[]>()));
                writerTask.AddChunk(0, new ChunkWriteInfo(0, It.IsAny<byte[]>()));

                Thread.Sleep(100);

                chunkWriterMock.Verify(x => x.WriteToFile(It.IsAny<byte[]>()), Times.Exactly(3));
                Assert.IsFalse(writerTask.IsActive());
            }
        }
    }
}
