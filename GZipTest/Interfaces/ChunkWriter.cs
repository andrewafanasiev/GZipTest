using System;
using System.Collections.Generic;
using System.Text;

namespace GZipTest.Interfaces
{
    public interface IChunkWriter
    {
        void WriteToFile(string fileName, byte[] bytes);
    }
}
