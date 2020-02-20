using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using GZipTest.Interfaces;

namespace GZipTest
{
    public class FileSplitterFactory : IFileSplitterFactory
    {
        public IFileSplitter Create(string actionType)
        {
            switch (actionType)
            {
                case Constants.Compress:
                    return new CompressionFileSplitter();
                case Constants.Decompress:
                    return new DecompressionFileSplitter();
                default:
                    throw new InvalidEnumArgumentException($"Could not find action with name: {actionType}");
            }
        }
    }
}
