namespace GZipTest.Interfaces
{
    public interface ICompressorFactory
    {
        IGZipCompressor Create(string actionType);
    }
}