using System;
using CommandLine;
using GZipTest.Dtos;
using GZipTest.Interfaces;

namespace GZipTest
{
    class Program
    {
        static void Main(string[] args)
        {
            IArgsValidator argsValidator = new ArgsValidator();

            if (argsValidator.IsArgsValid(args, out string errorMessage))
            {
                string actionType = args[0], inFile = args[1], outFile = args[2];

                IFileReader fileReader = new FileReader(inFile);
                IFileWriterTask fileWriterTask = new FileWriterTask(outFile);
                ICompressorFactory compressorFactory = new CompressorFactory();
                IChunksQueue chunksQueue = new ChunksQueue(Environment.ProcessorCount, compressorFactory.Create(actionType), fileReader, fileWriterTask);

                //todo: add chunk info to queue

                if (chunksQueue.IsActive() && fileWriterTask.IsActive())
                {
                    Console.WriteLine("operation success");
                }
            }
            else
            {
                Console.WriteLine(errorMessage);
            }

            #if DEBUG
            Console.ReadLine();
            #endif
        }
    }
}