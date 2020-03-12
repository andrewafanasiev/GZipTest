using System;
using System.Collections.Generic;
using GZipTest.Exceptions;
using GZipTest.Interfaces;
using NUnit.Framework;

namespace GZipTest.Tests.Exceptions
{
    [TestFixture]
    public class ErrorLogsTests
    {
        private IErrorLogs _errorLogs;

        [SetUp]
        public void Init()
        {
            _errorLogs = new ErrorLogs();
        }

        [Test]
        public void LogsReturnsTrueIfExceptionWasAdded()
        {
            _errorLogs.Add(new Exception("Something wrong"));

            Assert.IsTrue(_errorLogs.IsErrorExist(out List<Exception> errors));
            Assert.IsNotNull(errors);
        }

        [Test]
        public void LogsReturnsFalseIfNoExceptions()
        {
            Assert.IsFalse(_errorLogs.IsErrorExist(out List<Exception> errors));
            Assert.IsNull(errors);
        }
    }
}