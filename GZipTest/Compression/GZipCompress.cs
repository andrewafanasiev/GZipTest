using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using GZipTest.Exceptions;
using GZipTest.Interfaces;

namespace GZipTest.Compression
{
    public class GZipCompress : IGZipCompressor
    {
        /// <summary>
        /// Bytes compression
        /// </summary>
        /// <param name="bytes">Content</param>
        /// <returns>Result of compression</returns>
        public byte[] Execute(byte[] bytes)
        {
            try
            {
                using (var output = new MemoryStream())
                {
                    using (var compressStream = new GZipStream(output, CompressionMode.Compress))
                    {
                        compressStream.Write(bytes, 0, bytes.Length);
                    }

                    return output.ToArray();
                }
            }
            catch (Exception ex)
            {
                throw new CompressorException("An error occurred during compression", ex);
            }
        }
    }
}