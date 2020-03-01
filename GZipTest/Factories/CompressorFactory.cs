using System.IO;
using GZipTest.Compression;
using GZipTest.Interfaces;

namespace GZipTest.Factories
{
    public class CompressorFactory : ICompressorFactory
    {
        public IGZipCompressor Create(string actionType)
        {
            switch (actionType)
            {
                case Constants.Compress:
                    return new GZipCompress();
                case Constants.Decompress:
                    return new GZipDecompress();
                default:
                    throw new InvalidDataException($"Could not find action with name: {actionType}");
            }
        }
    }
}