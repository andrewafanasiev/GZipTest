using System.IO;
using GZipTest.Compression;
using GZipTest.Factories;
using GZipTest.Interfaces;
using NUnit.Framework;

namespace GZipTest.Tests.Factories
{
    [TestFixture]
    public class CompressorFactoryTests
    {
        private readonly ICompressorFactory _compressorFactory;

        public CompressorFactoryTests()
        {
            _compressorFactory = new CompressorFactory();
        }

        [Test]
        public void FactoryWillCreateGZipCompressObj()
        {
            var gzipCompressor = _compressorFactory.Create(Constants.Compress);

            Assert.IsAssignableFrom<GZipCompress>(gzipCompressor);
        }

        [Test]
        public void FactoryWillCreateGZipDecompressObj()
        {
            var gzipCompressor = _compressorFactory.Create(Constants.Decompress);

            Assert.IsAssignableFrom<GZipDecompress>(gzipCompressor);
        }

        [Test]
        public void ThrowExceptionOnInvalidActionType()
        {
            Assert.Throws(typeof(InvalidDataException), () => _compressorFactory.Create("myCustomActionType"));
        }
    }
}
