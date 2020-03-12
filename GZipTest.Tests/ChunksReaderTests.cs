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
    public class ChunksReaderTests
    {
        private Mock<ISourceReader> _fileReaderMock;
        private Mock<IGZipCompressor> _gzipCompressMock;
        private Mock<IWriterTask> _fileWriterTaskMock;
        private Mock<IErrorLogs> _errorLogsMock;

        [SetUp]
        public void Init()
        {
            _fileReaderMock = new Mock<ISourceReader>();
            _gzipCompressMock = new Mock<IGZipCompressor>();
            _fileWriterTaskMock = new Mock<IWriterTask>();
            _errorLogsMock = new Mock<IErrorLogs>();
        }

        [Test]
        public void ReaderReturnsErrorIfCompressorFails()
        {
            _fileReaderMock.Setup(x => x.GetChunkBytes(It.IsAny<ChunkReadInfo>())).Returns(It.IsAny<byte[]>());
            _gzipCompressMock.Setup(x => x.Execute(It.IsAny<byte[]>())).Throws(new CompressorException("An unexpected error occurred"));
            _fileWriterTaskMock.Setup(x => x.AddChunk(It.IsAny<int>(), It.IsAny<ChunkWriteInfo>()));

            using (ChunksReader chunksReader = new ChunksReader(Environment.ProcessorCount, _fileReaderMock.Object,
                _gzipCompressMock.Object, _fileWriterTaskMock.Object, _errorLogsMock.Object))
            {
                chunksReader.EnqueueChunk(new ChunkReadInfo(It.IsAny<int>(), It.IsAny<long>(), It.IsAny<int>()));

                Thread.Sleep(100);

                _fileReaderMock.Verify(x => x.GetChunkBytes(It.IsAny<ChunkReadInfo>()), Times.Once);
                _fileWriterTaskMock.Verify(x => x.AddChunk(It.IsAny<int>(), It.IsAny<ChunkWriteInfo>()), Times.Never);
            }
        }

        [Test]
        public void ReaderReturnsErrorIfReaderFails()
        {
            _fileReaderMock.Setup(x => x.GetChunkBytes(It.IsAny<ChunkReadInfo>())).Throws(new ReaderException("An unexpected error occurred"));
            _gzipCompressMock.Setup(x => x.Execute(It.IsAny<byte[]>())).Returns(It.IsAny<byte[]>());
            _fileWriterTaskMock.Setup(x => x.AddChunk(It.IsAny<int>(), It.IsAny<ChunkWriteInfo>()));

            using (ChunksReader chunksReader = new ChunksReader(Environment.ProcessorCount, _fileReaderMock.Object,
                _gzipCompressMock.Object, _fileWriterTaskMock.Object, _errorLogsMock.Object))
            {
                chunksReader.EnqueueChunk(new ChunkReadInfo(It.IsAny<int>(), It.IsAny<long>(), It.IsAny<int>()));

                Thread.Sleep(100);

                _fileReaderMock.Verify(x => x.GetChunkBytes(It.IsAny<ChunkReadInfo>()), Times.Once);
                _gzipCompressMock.Verify(x => x.Execute(It.IsAny<byte[]>()), Times.Never);
            }
        }

        [Test]
        public void WriterTaskExecutesWhenAddingChunkToQueue()
        {
            _fileReaderMock.Setup(x => x.GetChunkBytes(It.IsAny<ChunkReadInfo>())).Returns(It.IsAny<byte[]>());
            _gzipCompressMock.Setup(x => x.Execute(It.IsAny<byte[]>())).Returns(It.IsAny<byte[]>());
            _fileWriterTaskMock.Setup(x => x.AddChunk(It.IsAny<int>(), It.IsAny<ChunkWriteInfo>()));

            using (ChunksReader chunksReader = new ChunksReader(Environment.ProcessorCount, _fileReaderMock.Object,
                _gzipCompressMock.Object, _fileWriterTaskMock.Object, _errorLogsMock.Object))
            {
                chunksReader.EnqueueChunk(new ChunkReadInfo(It.IsAny<int>(), It.IsAny<long>(), It.IsAny<int>()));

                Thread.Sleep(100);

                _fileWriterTaskMock.Verify(x => x.AddChunk(It.IsAny<int>(), It.IsAny<ChunkWriteInfo>()), Times.Once);
            }
        }

        [Test]
        public void ReaderIsNotActiveIfNoChunks()
        {
            using (ChunksReader chunksReader = new ChunksReader(Environment.ProcessorCount, _fileReaderMock.Object,
                _gzipCompressMock.Object, _fileWriterTaskMock.Object, _errorLogsMock.Object))
            {
                Assert.IsFalse(chunksReader.IsActive());
            }
        }
    }
}