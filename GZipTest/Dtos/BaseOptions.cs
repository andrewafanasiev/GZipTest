using System;
using System.Collections.Generic;
using System.Text;
using CommandLine;

namespace GZipTest.Dtos
{
    public class BaseOtions
    {
        [Option('i', Required = true, HelpText = "Input file name")]
        public string InFile { get; set; }

        [Option('o', Required = true, HelpText = "Output file name")]
        public string OutFile { get; set; }
    }

    [Verb("compress")]
    public class Compress : BaseOtions {}

    [Verb("decompress")]
    public class Decompress : BaseOtions {}
}
