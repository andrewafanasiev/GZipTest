using System;
using System.Collections.Generic;
using System.Text;

namespace GZipTest.Interfaces
{
    public interface IGZipCompressor
    {
        /// <summary>
        /// Bytes compression or decompression
        /// </summary>
        /// <param name="bytes">Content</param>
        /// <returns>Result of compression or decompression</returns>
        byte[] Execute(byte[] bytes);
    }
}