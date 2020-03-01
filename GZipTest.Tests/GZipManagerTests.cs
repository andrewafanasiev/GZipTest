using System;
using System.Collections.Generic;
using System.Text;
using GZipTest.Dtos;
using GZipTest.Interfaces;
using Moq;
using NUnit.Framework;

namespace GZipTest.Tests
{
    [TestFixture]
    public class GZipManagerTests
    {
        private Mock<ISourceReader> _sourceReaderMock;
        private Mock<IChunkWriter> _chunkWriterMock;
        private Mock<IFileSplitterFactory> _splitterFactoryMok;
        private Mock<ICompressorFactory> _compressorFactoryMock;
        private Mock<ITaskFactory> _taskFactoryMock;
        private GZipManager _gZipManager;
        private Mock<IChunksQueue> _chunksQueueMock;
        private Mock<IWriterTask> _writerTaskMock;
        private Mock<IFileSplitter> _fileSplitterMock;
        private List<Exception> _exceptions;
        private Exception _exception;

        [SetUp]
        public void Init()
        {
            _sourceReaderMock = new Mock<ISourceReader>();
            _chunkWriterMock = new Mock<IChunkWriter>();
            _splitterFactoryMok = new Mock<IFileSplitterFactory>();
            _compressorFactoryMock = new Mock<ICompressorFactory>();
            _taskFactoryMock = new Mock<ITaskFactory>();
            _gZipManager = new GZipManager(It.IsAny<string>(), _sourceReaderMock.Object, _chunkWriterMock.Object,
                _splitterFactoryMok.Object, _compressorFactoryMock.Object, _taskFactoryMock.Object);
            _chunksQueueMock = new Mock<IChunksQueue>();
            _writerTaskMock = new Mock<IWriterTask>();
            _fileSplitterMock = new Mock<IFileSplitter>();
            _exceptions = null;
            _exception = null;
        }

        [Test]
        public void IsActiveOpReturnsFalseIfQueueAndWriterNonActive()
        {
            _chunksQueueMock.Setup(x => x.IsActive()).Returns(false);
            _writerTaskMock.Setup(x => x.IsActive()).Returns(false);

            Assert.IsFalse(_gZipManager.IsActiveOp(_chunksQueueMock.Object, _writerTaskMock.Object));
        }

        [Test]
        public void IsActiveOpReturnsFalseIfWriterTaskNonActive()
        {
            _chunksQueueMock.Setup(x => x.IsActive()).Returns(false);
            _writerTaskMock.Setup(x => x.IsActive()).Returns(true);

            Assert.IsTrue(_gZipManager.IsActiveOp(_chunksQueueMock.Object, _writerTaskMock.Object));
        }

        [Test]
        public void IsActiveOpReturnsFalseIfQueueNonActive()
        {
            _chunksQueueMock.Setup(x => x.IsActive()).Returns(true);
            _writerTaskMock.Setup(x => x.IsActive()).Returns(false);

            Assert.IsTrue(_gZipManager.IsActiveOp(_chunksQueueMock.Object, _writerTaskMock.Object));
        }

        [Test]
        public void IsErrorExistReturnsFalseIfQueueFails()
        {
            _chunksQueueMock.Setup(x => x.IsErrorExist(out _exceptions)).Returns(true);
            _writerTaskMock.Setup(x => x.IsErrorExist(out _exception)).Returns(false);

            Assert.IsTrue(_gZipManager.IsErrorExist(_chunksQueueMock.Object, _writerTaskMock.Object, out List<Exception> errors));
        }

        [Test]
        public void IsErrorExistReturnsFalseIfWriterTaskFails()
        {
            _chunksQueueMock.Setup(x => x.IsErrorExist(out _exceptions)).Returns(false);
            _writerTaskMock.Setup(x => x.IsErrorExist(out _exception)).Returns(true);

            Assert.IsTrue(_gZipManager.IsErrorExist(_chunksQueueMock.Object, _writerTaskMock.Object, out List<Exception> errors));
        }

        [Test]
        public void IsErrorExistReturnsFalseIfQueueAndWriterTaskFails()
        {
            _chunksQueueMock.Setup(x => x.IsErrorExist(out _exceptions)).Returns(false);
            _writerTaskMock.Setup(x => x.IsErrorExist(out _exception)).Returns(false);

            Assert.IsFalse(_gZipManager.IsErrorExist(_chunksQueueMock.Object, _writerTaskMock.Object, out List<Exception> errors));
        }

        [Test]
        public void ExecuteReturnsTrueIfAllChunksAreProcessedSuccessfully()
        {
            _fileSplitterMock.Setup(x => x.GetChunks(It.IsAny<string>(), It.IsAny<int>())).Returns(new ChunksInfo(
                new List<ChunkReadInfo>
                {
                    new ChunkReadInfo(0, 0, 1),
                    new ChunkReadInfo(1, 1, 2),
                }));
            _taskFactoryMock.Setup(x => x.CreatWriterTask(It.IsAny<int>(), It.IsAny<IChunkWriter>()))
                .Returns(_writerTaskMock.Object);
            _taskFactoryMock.Setup(x => x.CreateChunksQueue(It.IsAny<int>(), It.IsAny<ISourceReader>(),
                It.IsAny<IGZipCompressor>(), It.IsAny<IWriterTask>())).Returns(_chunksQueueMock.Object);
            _splitterFactoryMok.Setup(x => x.Create(It.IsAny<string>())).Returns(_fileSplitterMock.Object);
            _chunksQueueMock.Setup(x => x.IsErrorExist(out _exceptions)).Returns(false);
            _writerTaskMock.Setup(x => x.IsErrorExist(out _exception)).Returns(false);
            _chunksQueueMock.Setup(x => x.IsActive()).Returns(false);
            _writerTaskMock.Setup(x => x.IsActive()).Returns(false);

            bool opResult = _gZipManager.Execute(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), out List<Exception> errors);

            _chunksQueueMock.Verify(x => x.EnqueueChunk(It.IsAny<ChunkReadInfo>()), Times.Exactly(2));
            Assert.IsTrue(opResult);
            Assert.IsNull(errors);
        }

        [Test]
        public void ExecuteReturnsFalseIfErrorsOccurredDuringProcessing()
        {
            _fileSplitterMock.Setup(x => x.GetChunks(It.IsAny<string>(), It.IsAny<int>())).Returns(new ChunksInfo(
                new List<ChunkReadInfo>
                {
                    new ChunkReadInfo(0, 0, 1),
                }));
            _taskFactoryMock.Setup(x => x.CreatWriterTask(It.IsAny<int>(), It.IsAny<IChunkWriter>()))
                .Returns(_writerTaskMock.Object);
            _taskFactoryMock.Setup(x => x.CreateChunksQueue(It.IsAny<int>(), It.IsAny<ISourceReader>(),
                It.IsAny<IGZipCompressor>(), It.IsAny<IWriterTask>())).Returns(_chunksQueueMock.Object);
            _splitterFactoryMok.Setup(x => x.Create(It.IsAny<string>())).Returns(_fileSplitterMock.Object);
            _chunksQueueMock.Setup(x => x.IsErrorExist(out _exceptions)).Returns(false);
            _writerTaskMock.Setup(x => x.IsErrorExist(out _exception)).Returns(true);
            _chunksQueueMock.Setup(x => x.IsActive()).Returns(false);
            _writerTaskMock.Setup(x => x.IsActive()).Returns(false);

            bool opResult = _gZipManager.Execute(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), out List<Exception> errors);

            _chunksQueueMock.Verify(x => x.EnqueueChunk(It.IsAny<ChunkReadInfo>()), Times.Once);
            Assert.IsFalse(opResult);
        }
    }
}
