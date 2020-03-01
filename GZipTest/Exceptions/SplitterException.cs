using System;

namespace GZipTest.Exceptions
{
    /// <summary>
    /// The exception that is thrown when an splitting file to chunks error occurs
    /// </summary>
    public class SplitterException : Exception
    {
        public SplitterException(string message, Exception innerException) : base(message, innerException) { }
    }
}