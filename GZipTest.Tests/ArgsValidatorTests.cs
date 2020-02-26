using System;
using NUnit.Framework;

namespace GZipTest.Tests
{
    [TestFixture]
    public class ArgsValidatorTests
    {
        [Test]
        public void CorrectArgsCount()
        {
            var args = new[] { "1", "2", "3" };
            var argsValidator = new ArgsValidator();

            Assert.IsTrue(argsValidator.IsArgsCountValid(args));
        }

        [Test]
        public void NotCorrectArgsCount()
        {
            var args = new[] { "1" };
            var argsValidator = new ArgsValidator();

            Assert.IsFalse(argsValidator.IsArgsCountValid(args));
        }

        [Test]
        public void NotValidArgs()
        {
            var argsValidator = new ArgsValidator();

            Assert.IsFalse(argsValidator.IsArgsCountValid(null));
        }

        [Test]
        public void ValidActionTypes()
        {
            var argsValidator = new ArgsValidator();

            Assert.IsTrue(argsValidator.IsActionTypeValid(Constants.Compress));
            Assert.IsTrue(argsValidator.IsActionTypeValid(Constants.Decompress));
        }

        [Test]
        public void NotValidActionType()
        {
            var argsValidator = new ArgsValidator();
            const string actionType = "myCustomActionType";

            Assert.IsFalse(argsValidator.IsActionTypeValid(actionType));
        }

        [Test]
        public void NotValidNullableActionType()
        {
            var argsValidator = new ArgsValidator();

            Assert.IsFalse(argsValidator.IsActionTypeValid(null));
        }
    }
}
