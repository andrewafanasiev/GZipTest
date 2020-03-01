using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using GZipTest.Factories;
using GZipTest.IO;
using NUnit.Framework;

namespace GZipTest.Tests.Factories
{
    [TestFixture]
    public class FileSplitterFactoryTests
    {
        private FileSplitterFactory _splitterFactory;

        [SetUp]
        public void Init()
        {
            _splitterFactory = new FileSplitterFactory();
        }

        [Test]
        public void FactoryWillCreateCompressionFileSplitterObj()
        {
            Assert.IsInstanceOf<CompressionFileSplitter>(_splitterFactory.Create(Constants.Compress));
        }

        [Test]
        public void FactoryWillCreateDecompressionFileSplitterObj()
        {
            Assert.IsInstanceOf<DecompressionFileSplitter>(_splitterFactory.Create(Constants.Decompress));
        }

        [Test]
        public void ThrowExceptionOnInvalidActionType()
        {
            Assert.Throws<InvalidEnumArgumentException>(() => _splitterFactory.Create("myCustomActionType"));
        }

        [Test]
        public void ThrowExceptionOnNullableActionType()
        {
            Assert.Throws<InvalidEnumArgumentException>(() => _splitterFactory.Create(null));
        }
    }
}
