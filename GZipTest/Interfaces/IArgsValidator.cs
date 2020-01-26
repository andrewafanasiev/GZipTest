namespace GZipTest.Interfaces
{
    public interface IArgsValidator
    {
        bool IsArgsValid(string[] args, out string errorMessage);
        bool IsArgsCountValid(string[] args);
        bool IsActionTypeValid(string actionType);
        bool IsFilePathValid(string filePath);
    }
}