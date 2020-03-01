using System;
using System.Collections.Generic;
using System.Diagnostics;
using GZipTest.Factories;
using GZipTest.Interfaces;
using GZipTest.IO;
using GZipTest.Validation;

namespace GZipTest
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var argsValidator = new ArgsValidator();

                if (argsValidator.IsArgsValid(args, out string validationMessage))
                {
                    string actionType = args[0], inFile = args[1], outFile = args[2];
                    int chunkSize = Environment.SystemPageSize * 1024;
                    var gzipManager = new GZipManager(inFile, new FileChunkReader(inFile), new FileChunkWriter(outFile),
                        new FileSplitterFactory(), new CompressorFactory(), new TaskFactory());
                    var stopWatch = new Stopwatch();

                    stopWatch.Start();
                    bool isOpSuccess = gzipManager.Execute(actionType, Environment.ProcessorCount, chunkSize, out List<Exception> errors);
                    stopWatch.Stop();

                    if (isOpSuccess) IOManager.OpSuccess(stopWatch.Elapsed.TotalSeconds);
                    else IOManager.OpError(errors);
                }
                else
                {
                    IOManager.ValidationError(validationMessage);
                }
            }
            catch (Exception ex)
            {
                IOManager.OpError(ex);
            }

            #if DEBUG
            Console.ReadLine();
            #endif
        }
    }
}