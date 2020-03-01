using System;

namespace GZipTest.Exceptions
{
    /// <summary>
    /// The exception that is thrown when an input error occurs
    /// </summary>
    public class ReaderException : Exception
    {
        public ReaderException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}