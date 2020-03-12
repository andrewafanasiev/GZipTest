using GZipTest.Interfaces;
using GZipTest.Validation;
using NUnit.Framework;

namespace GZipTest.Tests.Validation
{
    [TestFixture]
    public class ArgsValidatorTests
    {
        private readonly IArgsValidator _argsValidator;

        public ArgsValidatorTests()
        {
            _argsValidator = new ArgsValidator();
        }

        [Test]
        public void ValidationFailsOnArgsCount()
        {
            ArgsValidator argsValidator = new ArgsValidator();

            Assert.IsFalse(argsValidator.IsArgsValid(null, out string message));
            Assert.IsTrue(message.Contains("Parameters are expected:\n1. Action. Possible values"));
        }

        [Test]
        public void ValidationFailsOnUnsupportedActionType()
        {
            string[] args = { "myActionType", "filePath1", "filePath2" };

            Assert.IsFalse(_argsValidator.IsArgsValid(args, out string message));
            Assert.IsTrue(message.Contains("Unsupported action type"));
        }

        [Test]
        public void ValidationFailsOnInvalidInputFileName()
        {
            string[] args = { Constants.Compress, "C:\\f|ile.zip", "filePath2" };

            Assert.IsFalse(_argsValidator.IsArgsValid(args, out string message));
            Assert.IsTrue(message.Contains("Input file path invalid"));
        }

        [Test]
        public void ValidationFailsOnInvalidOutputFileName()
        {
            string[] args = { Constants.Compress, "C:\\file1.zip", "C:\\fi|le2.zip.gz" };

            Assert.IsFalse(_argsValidator.IsArgsValid(args, out string message));
            Assert.IsTrue(message.Contains("Output file path invalid"));
        }

        [Test]
        public void ValidArgsCount()
        {
            string[] args = { "1", "2", "3" };

            Assert.IsTrue(_argsValidator.IsArgsCountValid(args));
        }

        [Test]
        public void NotValidArgsCount()
        {
            string[] args = { "1" };

            Assert.IsFalse(_argsValidator.IsArgsCountValid(args));
        }

        [Test]
        public void NotValidNullableArgs()
        {
            Assert.IsFalse(_argsValidator.IsArgsCountValid(null));
        }

        [Test]
        public void ValidActionTypes()
        {
            Assert.IsTrue(_argsValidator.IsActionTypeValid(Constants.Compress));
            Assert.IsTrue(_argsValidator.IsActionTypeValid(Constants.Decompress));
        }

        [Test]
        public void NotValidActionType()
        {
            const string actionType = "myCustomActionType";

            Assert.IsFalse(_argsValidator.IsActionTypeValid(actionType));
        }

        [Test]
        public void NotValidNullableActionType()
        {
            Assert.IsFalse(_argsValidator.IsActionTypeValid(null));
        }

        [Test]
        public void PathContainsInvalidChars()
        {
            const string path = @"C:\Users\SomeUser\Documents\fil|e.zip";

            Assert.IsFalse(_argsValidator.IsFilePathValid(path));
        }

        [Test]
        public void ValidFilePath()
        {
            const string path = @"C:\Users\SomeUser\Documents\file.zip";

            Assert.IsTrue(_argsValidator.IsFilePathValid(path));
        }
    }
}