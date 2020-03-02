using System;

namespace GZipTest.Exceptions
{
    /// <summary>
    /// The exception that is thrown when an compression or decompression error occurs
    /// </summary>
    public class CompressorException : Exception
    {
        public CompressorException(string message) : base(message) { }

        public CompressorException(string message, Exception innerException) : base(message, innerException) { }
    }
}