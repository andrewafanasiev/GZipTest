using System;
using System.Collections.Generic;
using System.Text;

namespace GZipTest.Interfaces
{
    public interface IFileSplitterFactory
    {
        IFileSplitter Create(string actiontype);
    }
}
