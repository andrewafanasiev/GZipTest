using System.IO;
using System.IO.Compression;
using GZipTest.Interfaces;

namespace GZipTest
{
    public class GZipDecompress : IGZipCompressor
    {
        public byte[] Execute(byte[] compressedBytes)
        {
            using (var output = new MemoryStream())
            {
                using (var input = new MemoryStream(compressedBytes))
                {
                    using (var decompressStream = new GZipStream(input, CompressionMode.Decompress))
                    {
                        decompressStream.CopyTo(output);
                    }

                    return output.ToArray();
                }
            }
        }
    }
}