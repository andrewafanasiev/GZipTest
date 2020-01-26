using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using CommandLine;
using GZipTest.Dtos;
using GZipTest.Interfaces;

namespace GZipTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Compress, Decompress>(args).MapResult(
                (Compress options) =>
                {
                    Console.WriteLine("compression start");

                    //todo: validate args

                    try
                    {
                        IFileReader fileReader = new FileReader(options.InFile);
                        IFileWriterTask fileWriterTask = new FileWriterTask(options.OutFile);
                        IGZipCompressor compressor = new GZipCompress();
                        IChunksQueue chunksQueue = new ChunksQueue(Environment.ProcessorCount, compressor, fileReader, fileWriterTask);

                        //todo: add chunk info to queue

                        if (chunksQueue.IsActive() && fileWriterTask.IsActive())
                        {
                            Console.WriteLine("compression success");
                        }
                    }
                    catch (Exception ex)
                    {
                        return 1;
                    }

                    return 0;
                },
                (Decompress options) =>
                {
                    Console.WriteLine("decompression start");

                    try
                    {
                        //todo: validate args

                        IFileReader fileReader = new FileReader(options.InFile);
                        IFileWriterTask fileWriterTask = new FileWriterTask(options.OutFile);
                        IGZipCompressor compressor = new GZipDecompress();
                        IChunksQueue chunksQueue = new ChunksQueue(Environment.ProcessorCount, compressor, fileReader, fileWriterTask);

                        //todo: add chunk info to queue

                        if (chunksQueue.IsActive() && fileWriterTask.IsActive())
                        {
                            Console.WriteLine("decompression success");
                        }
                    }
                    catch (Exception ex)
                    {
                        return 1;
                    }

                    return 0;
                },
                errs => 1
            );

            #if DEBUG
            Console.ReadLine();
            #endif
        }
    }
}
