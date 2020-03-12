using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using GZipTest.Factories;
using GZipTest.Helpers;
using GZipTest.Interfaces;
using Moq;
using NUnit.Framework;

namespace GZipTest.Tests.Helpers
{
    [TestFixture]
    public class ThreadHelperTests
    {
        [Test]
        public void AbortedConvertsToRunningThreadState()
        {
            ThreadState sts = ThreadState.Aborted.GetSimpleThreadState();

            Assert.AreEqual(ThreadState.Running, sts);
        }

        [Test]
        public void UnstartedThreadStateRemainsUnchanged()
        {
            ThreadState sts = (ThreadState.Background | ThreadState.Unstarted).GetSimpleThreadState();

            Assert.AreEqual(ThreadState.Unstarted, sts);
        }

        [Test]
        public void SuspendedConvertsToRunningThreadState()
        {
            ThreadState sts = ThreadState.Suspended.GetSimpleThreadState();

            Assert.AreEqual(ThreadState.Running, sts);
        }

        [Test]
        public void StoppedThreadStateRemainsUnchanged()
        {
            ThreadState sts = ThreadState.Stopped.GetSimpleThreadState();

            Assert.AreEqual(ThreadState.Stopped, sts);
        }

        [Test]
        public void WaitSleepJoinThreadStateRemainsUnchanged()
        {
            ThreadState sts = ThreadState.WaitSleepJoin.GetSimpleThreadState();

            Assert.AreEqual(ThreadState.WaitSleepJoin, sts);
        }

        [Test]
        public void RunningThreadStateRemainsUnchanged()
        {
            ThreadState sts = ThreadState.Running.GetSimpleThreadState();

            Assert.AreEqual(ThreadState.Running, sts);
        }
    }
}
