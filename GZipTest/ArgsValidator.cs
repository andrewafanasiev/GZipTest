using System.IO;
using System.Linq;
using GZipTest.Interfaces;

namespace GZipTest
{
    /// <summary>
    /// Parameters validation
    /// </summary>
    public class ArgsValidator : IArgsValidator
    {
        private const string MessageTemplate = "{0} file path invalid";

        /// <summary>
        /// Parameters validation
        /// </summary>
        /// <param name="args">Arguments</param>
        /// <param name="errorMessage">Message for invalid arguments</param>
        /// <returns>Result of checking</returns>
        public bool IsArgsValid(string[] args, out string errorMessage)
        {
            errorMessage = null;

            if (!IsArgsCountValid(args))
            {
                errorMessage = "Parameters are expected:\n1. Action. Possible values: compress or decompress\n2. Input file path\n3. Output file path\n";
                return false;
            }

            if (!IsActionTypeValid(args[0]))
            {
                errorMessage = "Unsupported action type. Possible values: compress or decompress";
                return false;
            }

            if (!IsFilePathValid(args[1]))
            {
                errorMessage = string.Format(MessageTemplate, "Input");
                return false;
            }

            if (!IsFilePathValid(args[2]))
            {
                errorMessage = string.Format(MessageTemplate, "Output");
                return false;
            }

            if (!IsFileExists(args[1]))
            {
                errorMessage = $"Input file with path: {args[1]} does not exists";
                return false;
            }

            if (IsFileExists(args[2]))
            {
                errorMessage = $"Output file with path: {args[2]} is already exists. Choose another name, path or remove file";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Parameter number validation
        /// </summary>
        /// <param name="args">Arguments</param>
        /// <returns>Result of checking</returns>
        public bool IsArgsCountValid(string[] args)
        {
            return args?.Length == 3;
        }

        /// <summary>
        /// Action name validation
        /// </summary>
        /// <param name="actionType">Action name</param>
        /// <returns>Result of checking</returns>
        public bool IsActionTypeValid(string actionType)
        {
            return actionType == Constants.Compress || actionType == Constants.Decompress;
        }

        /// <summary>
        /// File path validation
        /// </summary>
        /// <param name="filePath">Path to file</param>
        /// <returns>Result of checking</returns>
        public bool IsFilePathValid(string filePath)
        {
            return filePath.IndexOfAny(Path.GetInvalidPathChars()) == -1;
        }

        /// <summary>
        /// Checking the existence of a file in the system
        /// </summary>
        /// <param name="filePath">Path to file</param>
        /// <returns>Result of checking</returns>
        public bool IsFileExists(string filePath)
        {
            return File.Exists(filePath);
        }
    }
}