using System;
using CommandLine;
using GZipTest.Interfaces;

namespace GZipTest
{
    class Program
    {
        static void Main(string[] args)
        {
            IArgsValidator argsValidator = new ArgsValidator();

            try
            {
                if (argsValidator.IsArgsValid(args, out string errorMessage))
                {
                    string actionType = args[0], inFile = args[1], outFile = args[2];
                    var gzipManager = new GZipManager(inFile, outFile);

                    gzipManager.Execute(actionType, Environment.ProcessorCount);
                }
                else
                {
                    Console.WriteLine(errorMessage);
                }
            }
            catch (Exception ex)
            {
                //todo: handle exception
            }

            #if DEBUG
            Console.ReadLine();
            #endif
        }
    }
}