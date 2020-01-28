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
                    var gzipManager = new GZipManager(inFile, outFile);
                    var stopWatch = new Stopwatch();

                    stopWatch.Start();
                    gzipManager.Execute(actionType, Environment.ProcessorCount, chunkSize);
                    stopWatch.Stop();

                    //todo: separate abstraction for information output
                    Console.WriteLine("Process completed in {0} seconds", stopWatch.Elapsed.TotalSeconds);
                }
                else
                {
                    Console.WriteLine(errorMessage);
                }
            }
            catch (Exception ex)
            {
                //todo: separate abstraction for information output
                Console.WriteLine($"Something wrong. Exception info: {ex}");
            }

            #if DEBUG
            Console.ReadLine();
            #endif
        }
    }
}