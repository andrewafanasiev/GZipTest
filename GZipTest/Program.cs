﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            var argsValidator = new ArgsValidator();
            var logger = LogManager.GetCurrentClassLogger();

            try
            {
                if (argsValidator.IsArgsValid(args, out string validationMessage))
                {
                    string actionType = args[0], inFile = args[1], outFile = args[2];
                    int chunkSize = Environment.SystemPageSize * 1024;
                    var gzipManager = new GZipManager(inFile, new FileChunkReader(inFile), new FileChunkWriter(outFile),
                        new FileSplitterFactory(), new CompressorFactory(), new TaskFactory());
                    var stopWatch = new Stopwatch();

                    stopWatch.Start();
                    bool isOpSuccess = gzipManager.Execute(actionType, Environment.ProcessorCount, chunkSize, out List<Exception> exceptions);
                    stopWatch.Stop();

                    if (isOpSuccess)
                    {
                        IOManager.OpSuccess(stopWatch.Elapsed.TotalSeconds);
                        return 0;
                    }

                    foreach (var ex in exceptions) logger.Error(ex);
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