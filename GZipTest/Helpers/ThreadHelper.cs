using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace GZipTest.Helpers
{
    public static class ThreadHelper
    {
        /// <summary>
        /// Thread state conversion to most useful values
        /// </summary>
        /// <returns>Returns one of the four possible values: Unstarted, Running, WaitSleepJoin, and Stopped</returns>
        public static ThreadState GetSimpleThreadState(this ThreadState threadState)
        {
            return threadState & (ThreadState.Unstarted | ThreadState.WaitSleepJoin | ThreadState.Stopped);
        }
    }
}
