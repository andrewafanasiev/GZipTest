using System;
using System.Collections.Generic;
using System.Text;

namespace GZipTest.Interfaces
{
    public interface IGZipCompressor
    {
        byte[] Execute(byte[] bytes);
    }
}