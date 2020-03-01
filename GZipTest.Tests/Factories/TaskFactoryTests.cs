using System;
using System.Collections.Generic;
using System.Text;
using GZipTest.Factories;
using GZipTest.Interfaces;
using Moq;
using NUnit.Framework;

namespace GZipTest.Tests.Factories
{
    [TestFixture]
    public class TaskFactoryTests
    {
        private TaskFactory _taskFactory;

        [SetUp]
        public void Init()
        {
            _taskFactory = new TaskFactory();
        }

        [Test]
        public void FactoryWillCreateChunksQueueObj()
        {
            var chunksQueue = _taskFactory.CreateChunksQueue(It.IsAny<int>(), It.IsAny<ISourceReader>(),
                It.IsAny<IGZipCompressor>(), It.IsAny<IWriterTask>());

            Assert.IsInstanceOf<ChunksQueue>(chunksQueue);
        }

        [Test]
        public void FactoryWillCreateFileWriterTaskObj()
        {
            var writerTask = _taskFactory.CreatWriterTask(It.IsAny<int>(), It.IsAny<IChunkWriter>());

            Assert.IsInstanceOf<FileWriterTask>(writerTask);
        }
    }
}
