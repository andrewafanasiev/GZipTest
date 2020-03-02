namespace GZipTest.Interfaces
{
    /// <summary>
    /// Writing data to file
    /// </summary>
    public interface IChunkWriter
    {
        /// <summary>
        /// Writing data to file
        /// </summary>
        /// <param name="bytes">Data</param>
        void WriteToFile(byte[] bytes);
    }
}
