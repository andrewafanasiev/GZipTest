using System;
using System.Collections.Generic;
using System.Linq;
using GZipTest.Interfaces;

namespace GZipTest.Exceptions
{
    /// <summary>
    /// Storing errors that occurred during execution
    /// </summary>
    public class ErrorLogs : IErrorLogs
    {
        private readonly List<Exception> _exceptions = new List<Exception>();
        private readonly object _lockExObj = new object();

        /// <summary>
        /// Add error to list
        /// </summary>
        /// <param name="error">Exception</param>
        public void Add(Exception error)
        {
            lock (_lockExObj)
            {
                _exceptions.Add(error);
            }
        }

        /// <summary>
        /// Errors that occurred at execution
        /// </summary>
        /// <returns>Result of checking</returns>
        public bool IsErrorExist(out List<Exception> errors)
        {
            errors = null;

            lock (_lockExObj)
            {
                if (!_exceptions.Any()) return false;

                errors = _exceptions;
                return true;
            }
        }
    }
}
