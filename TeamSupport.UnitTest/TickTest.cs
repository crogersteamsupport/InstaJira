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
            //for (int j = 0; j < i; ++j)
            //    sum += j;
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
            using (new ScopedElapsedTime())
                return;
            long ticks = Ticks.TickCount;
            System.Threading.Thread.Sleep(1234);
            long value = 0;
            for(int i = 0; i < 1000000000;++i)
            {
                if (i % 2 == 0)
                    value += i;
                else
                    value -= i;
            }

            ms = Ticks.Milliseconds(Ticks.Elapsed(ticks));
        }
    }
}
