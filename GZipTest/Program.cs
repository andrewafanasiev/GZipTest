using System;
using System.Diagnostics;
using GZipTest.Interfaces;

namespace GZipTest
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                IArgsValidator argsValidator = new ArgsValidator();

                if (argsValidator.IsArgsValid(args, out string errorMessage))
                {
                    string actionType = args[0], inFile = args[1], outFile = args[2];
                    int chunkSize = Environment.SystemPageSize * 1024;
                    var gzipManager = new GZipManager(inFile, new FileReader(inFile), new FileChunkWriter(outFile),
                        new FileSplitterFactory(), new CompressorFactory(), new TaskFactory());
                    var stopWatch = new Stopwatch();

                    stopWatch.Start();
                    bool isOpSuccess = gzipManager.Execute(actionType, Environment.ProcessorCount, chunkSize);
                    stopWatch.Stop();

                    //todo: separate abstraction for information output
                    if (isOpSuccess)
                    {
                        Console.WriteLine("Process completed successfully in {0} seconds", stopWatch.Elapsed.TotalSeconds);
                    }
                    else
                    {
                        Console.WriteLine("An unexpected error occurred while running the application. See logs for details");
                    }
                }
                else
                {
                    Console.WriteLine($"Validation error: {errorMessage}");
                }
            }
            catch (Exception)
            {
                //todo: separate abstraction for information output
                Console.WriteLine("An unexpected error occurred while running the application. See logs for details");
            }

            #if DEBUG
            Console.ReadLine();
            #endif
        }
    }
}