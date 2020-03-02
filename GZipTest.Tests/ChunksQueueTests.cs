using System;
using System.Collections.Generic;
using System.Linq;
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
    public class ChunksQueueTests
    {
        private Mock<ISourceReader> _fileReaderMock;
        private Mock<IGZipCompressor> _gzipCompressMock;
        private Mock<IWriterTask> _fileWriterTaskMock;

        [SetUp]
        public void Init()
        {
            _fileReaderMock = new Mock<ISourceReader>();
            _gzipCompressMock = new Mock<IGZipCompressor>();
            _fileWriterTaskMock = new Mock<IWriterTask>();
        }

        [Test]
        public void QueueReturnsErrorIfCompressorFails()
        {
            _fileReaderMock.Setup(x => x.GetChunkBytes(It.IsAny<ChunkReadInfo>())).Returns(It.IsAny<byte[]>());
            _gzipCompressMock.Setup(x => x.Execute(It.IsAny<byte[]>())).Throws(new CompressorException("An unexpected error occurred"));
            _fileWriterTaskMock.Setup(x => x.AddChunk(It.IsAny<int>(), It.IsAny<ChunkWriteInfo>()));

            using (var chunksQueue = new ChunksQueue(Environment.ProcessorCount, _fileReaderMock.Object,
                _gzipCompressMock.Object, _fileWriterTaskMock.Object))
            {
                chunksQueue.EnqueueChunk(new ChunkReadInfo(It.IsAny<int>(), It.IsAny<long>(), It.IsAny<int>()));

                Thread.Sleep(100);

                _fileReaderMock.Verify(x => x.GetChunkBytes(It.IsAny<ChunkReadInfo>()), Times.Once);
                _fileWriterTaskMock.Verify(x => x.AddChunk(It.IsAny<int>(), It.IsAny<ChunkWriteInfo>()), Times.Never);
                Assert.IsTrue(chunksQueue.IsErrorExist(out List<Exception> exceptions));
                Assert.IsTrue(exceptions.Any(x => x.GetType() == typeof(CompressorException)));
            }
        }

        [Test]
        public void QueueReturnsErrorIfReaderFails()
        {
            _fileReaderMock.Setup(x => x.GetChunkBytes(It.IsAny<ChunkReadInfo>())).Throws(new ReaderException("An unexpected error occurred"));
            _gzipCompressMock.Setup(x => x.Execute(It.IsAny<byte[]>())).Returns(It.IsAny<byte[]>());
            _fileWriterTaskMock.Setup(x => x.AddChunk(It.IsAny<int>(), It.IsAny<ChunkWriteInfo>()));

            using (var chunksQueue = new ChunksQueue(Environment.ProcessorCount, _fileReaderMock.Object,
                _gzipCompressMock.Object, _fileWriterTaskMock.Object))
            {
                chunksQueue.EnqueueChunk(new ChunkReadInfo(It.IsAny<int>(), It.IsAny<long>(), It.IsAny<int>()));

                Thread.Sleep(100);

                _fileReaderMock.Verify(x => x.GetChunkBytes(It.IsAny<ChunkReadInfo>()), Times.Once);
                _gzipCompressMock.Verify(x => x.Execute(It.IsAny<byte[]>()), Times.Never);
                Assert.IsTrue(chunksQueue.IsErrorExist(out List<Exception> exceptions));
                Assert.IsTrue(exceptions.Any(x => x.GetType() == typeof(ReaderException)));
            }
        }

        [Test]
        public void WriterTaskExecutesWhenAddingChunkToQueue()
        {
            _fileReaderMock.Setup(x => x.GetChunkBytes(It.IsAny<ChunkReadInfo>())).Returns(It.IsAny<byte[]>());
            _gzipCompressMock.Setup(x => x.Execute(It.IsAny<byte[]>())).Returns(It.IsAny<byte[]>());
            _fileWriterTaskMock.Setup(x => x.AddChunk(It.IsAny<int>(), It.IsAny<ChunkWriteInfo>()));

            using (var chunksQueue = new ChunksQueue(Environment.ProcessorCount, _fileReaderMock.Object,
                _gzipCompressMock.Object, _fileWriterTaskMock.Object))
            {
                chunksQueue.EnqueueChunk(new ChunkReadInfo(It.IsAny<int>(), It.IsAny<long>(), It.IsAny<int>()));

                Thread.Sleep(100);

                _fileWriterTaskMock.Verify(x => x.AddChunk(It.IsAny<int>(), It.IsAny<ChunkWriteInfo>()), Times.Once);
                Assert.IsFalse(chunksQueue.IsErrorExist(out List<Exception> exceptions));
            }
        }

        [Test]
        public void QueueIsNotActiveIfNoChunks()
        {
            using (var chunksQueue = new ChunksQueue(Environment.ProcessorCount, _fileReaderMock.Object,
                _gzipCompressMock.Object, _fileWriterTaskMock.Object))
            {
                Assert.IsFalse(chunksQueue.IsActive());
            }
        }
    }
}