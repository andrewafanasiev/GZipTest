using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using GZipTest.Interfaces;
using GZipTest.IO;

namespace GZipTest.Factories
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
