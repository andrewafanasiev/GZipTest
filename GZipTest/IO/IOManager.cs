using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using GZipTest.Exceptions;

namespace GZipTest.IO
{
    public class IOManager
    {
        private static readonly List<Type> KnownExceptionTypes = new List<Type>
        {
            typeof(CompressorException),
            typeof(ReaderException),
            typeof(WriterException),
            typeof(SplitterException)
        };

        /// <summary>
        /// Display success message in console
        /// </summary>
        /// <param name="totalSeconds">Time for operaion in seconds</param>
        public static void OpSuccess(double totalSeconds)
        {
            Console.WriteLine("Process completed successfully in {0:0.00} seconds", totalSeconds);
        }

        /// <summary>
        /// Display validation message in console
        /// </summary>
        /// <param name="message">Validation message</param>
        public static void ValidationError(string message)
        {
            Console.WriteLine($"Validation error. {message}");
        }

        /// <summary>
        /// Display human readable error message in console
        /// </summary>
        /// <param name="errors">Exceptions</param>
        public static void OpError(List<Exception> errors)
        {
            if(errors == null) throw new InvalidDataException("Errors cannot be nullable");

            var knownException = errors.FirstOrDefault(x => KnownExceptionTypes.Contains(x.GetType()));
            Console.WriteLine(knownException != null
                ? $"{knownException.Message}. See logs for details"
                : "An unexpected error occurred while running the application. See logs for details");
        }

        /// <summary>
        /// Display human readable error message in console
        /// </summary>
        /// <param name="error">Exception</param>
        public static void OpError(Exception error)
        {
            if (error == null) throw new InvalidDataException("Error cannot be nullable");

            Console.WriteLine(KnownExceptionTypes.Contains(error.GetType())
                ? $"{error.Message}. See logs for details"
                : "An unexpected error occurred while running the application. See logs for details");
        }
    }
}
