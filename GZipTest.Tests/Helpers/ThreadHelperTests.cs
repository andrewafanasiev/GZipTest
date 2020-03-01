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
            var sts = ThreadState.Aborted.GetSimpleThreadState();

            Assert.AreEqual(ThreadState.Running, sts);
        }

        [Test]
        public void UnstartedThreadStateRemainsUnchanged()
        {
            var sts = (ThreadState.Background | ThreadState.Unstarted).GetSimpleThreadState();

            Assert.AreEqual(ThreadState.Unstarted, sts);
        }

        [Test]
        public void SuspendedConvertsToRunningThreadState()
        {
            var sts = ThreadState.Suspended.GetSimpleThreadState();

            Assert.AreEqual(ThreadState.Running, sts);
        }

        [Test]
        public void StoppedThreadStateRemainsUnchanged()
        {
            var sts = ThreadState.Stopped.GetSimpleThreadState();

            Assert.AreEqual(ThreadState.Stopped, sts);
        }

        [Test]
        public void WaitSleepJoinThreadStateRemainsUnchanged()
        {
            var sts = ThreadState.WaitSleepJoin.GetSimpleThreadState();

            Assert.AreEqual(ThreadState.WaitSleepJoin, sts);
        }

        [Test]
        public void RunningThreadStateRemainsUnchanged()
        {
            var sts = ThreadState.Running.GetSimpleThreadState();

            Assert.AreEqual(ThreadState.Running, sts);
        }
    }
}
