using System;
using System.Collections.Generic;
using System.Linq;
using GZipTest.Dtos;
using GZipTest.Interfaces;

namespace GZipTest
{
    public class GZipManager : IGZipManager
    {
        private readonly ICompressorFactory _compressorFactory;
        private readonly IFilePreparation _filePreparation;
        private readonly string _inFile;
        private readonly string _outFile;

        public GZipManager(string inFile, string outFile)
        {
            _inFile = inFile;
            _outFile = outFile;
            _filePreparation = new FilePreparationForCompress();
            _compressorFactory = new CompressorFactory();
        }

        public void Execute(string actionType, int workersCount, int chunkSize)
        {
            var chunkInfos = _filePreparation.GetChunks(_inFile, chunkSize);

            using (var fileWriterTask = new FileWriterTask(_outFile, chunkInfos.ChunksCount))
            using (var chunksQueue = new ChunksQueue(_inFile, workersCount, _compressorFactory.Create(actionType), fileWriterTask))
            {
                foreach (var chunkInfo in chunkInfos.Chunks)
                {
                    chunksQueue.EnqueueChunk(new ChunkReadInfo(chunkInfo.Id, chunkInfo.Offset, chunkInfo.BytesCount));
                }

                while (true)
                {
                    //todo: double check patter on success
                    if (chunksQueue.IsErrorExist(out List<Exception> queueExceptions) | fileWriterTask.IsErrorExist(out Exception writerException))
                    {
                        //todo: log for exceptions
                        Console.WriteLine("Something wrong");
                        Console.WriteLine("Queue exception: {0}", queueExceptions.FirstOrDefault());
                        Console.WriteLine("FileWriter exception: {0}", writerException);
                        break;
                    }

                    if (!chunksQueue.IsActive() && !fileWriterTask.IsActive())
                    {
                        //todo: log for success operation
                        Console.WriteLine("Operation success");
                        break;
                    }
                }
            }
        }
    }
}