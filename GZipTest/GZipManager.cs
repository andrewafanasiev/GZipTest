using System;
using GZipTest.Dtos;
using GZipTest.Interfaces;

namespace GZipTest
{
    public class GZipManager : IGZipManager
    {
        private readonly IFileWriterTask _fileWriterTask;
        private readonly ICompressorFactory _compressorFactory;
        private readonly IFilePreparation _filePreparation;
        private readonly string _inFile;

        public GZipManager(string inFile, string outFile)
        {
            _inFile = inFile;
            _filePreparation = new FilePreparationForCompress();
            _fileWriterTask = new FileWriterTask(outFile);
            _compressorFactory = new CompressorFactory();
        }

        public void Execute(string actionType, int workersCount)
        {
            using (var chunksQueue = new ChunksQueue(_inFile, workersCount, _compressorFactory.Create(actionType), _fileWriterTask))
            {
                var chunkInfos = _filePreparation.GetChunkInfos(_inFile);

                foreach (var chunkInfo in chunkInfos)
                {
                    chunksQueue.EnqueueChunk(new ChunkInfo(chunkInfo.Id, chunkInfo.Offset, chunkInfo.BytesCount));
                }
            }

            while (true)
            {
                if (!_fileWriterTask.IsActive()) break;
            }

            Console.WriteLine("operation success");
        }
    }
}