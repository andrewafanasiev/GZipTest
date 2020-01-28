﻿using System.IO;
using System.Linq;
using GZipTest.Interfaces;

namespace GZipTest
{
    public class ArgsValidator : IArgsValidator
    {
        private const string MessageTemplate = "{0} file path invalid";

        public bool IsArgsValid(string[] args, out string errorMessage)
        {
            errorMessage = default(string);

            if (!IsArgsCountValid(args))
            {
                errorMessage = "Parameters ara expected:\n1. Action. Possible values: compress or decompress\n2. Input file path\n3. Output file path\n";
                return false;
            }

            if (!IsActionTypeValid(args[0]))
            {
                errorMessage = "Unsupported action type";
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

        public bool IsArgsCountValid(string[] args)
        {
            if (args?.Length != 3) return false;

            return true;
        }

        public bool IsActionTypeValid(string actionType)
        {
            if (actionType != Constants.Compress && actionType != Constants.Decompress) return false;

            return true;
        }

        public bool IsFilePathValid(string filePath)
        {
            if (filePath.IndexOfAny(Path.GetInvalidPathChars()) != -1) return false;

            return true;
        }

        public bool IsFileExists(string filePath)
        {
            return File.Exists(filePath);
        }
    }
}