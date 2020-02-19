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
        /// <param name="fileName">Path to file</param>
        /// <param name="bytes">Data</param>
        void WriteToFile(string fileName, byte[] bytes);
    }
}
