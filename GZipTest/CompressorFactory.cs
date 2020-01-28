using System.ComponentModel;
using GZipTest.Interfaces;

namespace GZipTest
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
                    throw new InvalidEnumArgumentException($"Could not find action with name: {actionType}");
            }
        }
    }
}