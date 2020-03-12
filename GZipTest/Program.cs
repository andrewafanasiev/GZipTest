using System;
using System.Collections.Generic;
using System.Diagnostics;
using GZipTest.Exceptions;
using GZipTest.Factories;
using GZipTest.Interfaces;
using GZipTest.IO;
using GZipTest.Validation;
using NLog;

namespace GZipTest
{
    class Program
    {
        static int Main(string[] args)
        {
            ArgsValidator argsValidator = new ArgsValidator();
            Logger logger = LogManager.GetCurrentClassLogger();

            try
            {
                if (argsValidator.IsArgsValid(args, out string validationMessage))
                {
                    string actionType = args[0], inFile = args[1], outFile = args[2];
                    int chunkSize = Environment.SystemPageSize * 1024;
                    GZipManager gzipManager = new GZipManager(inFile, new FileChunkReader(inFile), new FileChunkWriter(outFile),
                        new FileSplitterFactory(), new CompressorFactory(), new TaskFactory(), new ErrorLogs());
                    Stopwatch stopWatch = new Stopwatch();

                    stopWatch.Start();
                    bool isOpSuccess = gzipManager.Execute(actionType, Environment.ProcessorCount, chunkSize, out List<Exception> exceptions);
                    stopWatch.Stop();

                    if (isOpSuccess)
                    {
                        IOManager.OpSuccess(stopWatch.Elapsed.TotalSeconds);
                        return 0;
                    }

                    foreach (Exception ex in exceptions) logger.Error(ex);
                    IOManager.OpError(exceptions);

                    return 1;
                }

                IOManager.ValidationError(validationMessage);
                return 1;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                IOManager.OpError(ex);

                return 1;
            }
        }
    }
}