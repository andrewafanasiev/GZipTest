namespace GZipTest.Interfaces
{
    public interface IGZipManager
    {
        void Execute(string actionType, int workersCount);
    }
}