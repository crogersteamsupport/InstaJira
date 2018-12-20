using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;

namespace TeamSupport.UnitTest
{
    [TestClass]
    public class TickTest
    {
        void UseThreadTime(ref long sum, int i)
        {
            for (int j = 0; j < i; ++j)
                sum += j;
            System.Threading.Thread.Sleep(3);   // ignored!

            //for (int j = 0; j < i; ++j)
            //    sum -= j;
        }

        [TestMethod]
        public void TickCountTest()
        {
            long outer = Ticks.TickCount;
            long sum = 0;
            for (int i = 0; i < 10000; ++i)
            {
                using (ScopedElapsedTime.Trace)
                    UseThreadTime(ref sum, i);
            }

            double ms = Ticks.Milliseconds(Ticks.Elapsed(outer));
            ScopedElapsedTime.DebugWrite();
        }
    }
}
