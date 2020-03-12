using System;
using System.Collections.Generic;

namespace GZipTest.Interfaces
{
    /// <summary>
    /// Storing errors that occurred during execution
    /// </summary>
    public interface IErrorLogs
    {
        /// <summary>
        /// Add error to list
        /// </summary>
        /// <param name="error">Exception</param>
        void Add(Exception error);

        /// <summary>
        /// Errors that occurred at execution
        /// </summary>
        /// <returns>Result of checking</returns>
        bool IsErrorExist(out List<Exception> errors);
    }
}