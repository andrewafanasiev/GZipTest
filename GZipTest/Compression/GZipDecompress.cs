using System;
using System.IO;
using System.IO.Compression;
using GZipTest.Exceptions;
using GZipTest.Interfaces;

namespace GZipTest.Compression
{
    public class GZipDecompress : IGZipCompressor
    {
        /// <summary>
        /// Bytes decompression
        /// </summary>
        /// <param name="bytes">Content</param>
        /// <returns>Result of decompression</returns>
        public byte[] Execute(byte[] bytes)
        {
            try
            {
                using (MemoryStream output = new MemoryStream())
                {
                    using (MemoryStream input = new MemoryStream(bytes))
                    {
                        using (GZipStream decompressStream = new GZipStream(input, CompressionMode.Decompress))
                        {
                            decompressStream.CopyTo(output);
                        }

                        return output.ToArray();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new CompressorException("An error occurred during decompression", ex);
            }
        }
    }
}