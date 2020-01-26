using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using GZipTest.Interfaces;

namespace GZipTest
{
    public class GZipCompress : IGZipCompressor
    {
        public byte[] Execute(byte[] originalBytes)
        {
            using (var output = new MemoryStream())
            {
                using (var compressStream = new GZipStream(output, CompressionMode.Compress))
                {
                    compressStream.Write(originalBytes, 0, originalBytes.Length);
                }

                return output.ToArray();
            }
        }
    }
}
