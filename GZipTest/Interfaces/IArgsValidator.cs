namespace GZipTest.Interfaces
{
    /// <summary>
    /// Parameters validation
    /// </summary>
    public interface IArgsValidator
    {
        /// <summary>
        /// Parameters validation
        /// </summary>
        /// <param name="args">Arguments</param>
        /// <param name="errorMessage">Message for invalid arguments</param>
        /// <returns>Result of checking</returns>
        bool IsArgsValid(string[] args, out string errorMessage);

        /// <summary>
        /// Parameter number validation
        /// </summary>
        /// <param name="args">Arguments</param>
        /// <returns>Result of checking</returns>
        bool IsArgsCountValid(string[] args);

        /// <summary>
        /// Action name validation
        /// </summary>
        /// <param name="actionType">Action name</param>
        /// <returns>Result of checking</returns>
        bool IsActionTypeValid(string actionType);

        /// <summary>
        /// File path validation
        /// </summary>
        /// <param name="filePath">Path to file</param>
        /// <returns>Result of checking</returns>
        bool IsFilePathValid(string filePath);

        /// <summary>
        /// Checking the existence of a file in the system
        /// </summary>
        /// <param name="filePath">Path to file</param>
        /// <returns>Result of checking</returns>
        bool IsFileExists(string filePath);
    }
}