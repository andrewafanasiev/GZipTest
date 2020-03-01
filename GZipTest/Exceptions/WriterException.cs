using System;

namespace GZipTest.Exceptions
{
    /// <summary>
    /// The exception that is thrown when an output error occurs
    /// </summary>
    public class WriterException : Exception
    {
        public WriterException(string message) : base(message) { }

        public WriterException(string message, Exception innerException) : base(message, innerException) { }
    }
}