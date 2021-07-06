﻿using System;
using System.Threading;

namespace SuperComicLib.Threading
{
    public struct SpinCountdownLockSlim : ISpinLockBase
    {
        private volatile int m_count;
        private readonly int m_initCount;

        public SpinCountdownLockSlim(int initCount) =>
            m_count = m_initCount = initCount;

        public bool IsLocked
        {
            get
            {
                int count = m_count;
                return count == 0;
            }
        }

        public bool TryEnter(int millisecondsTimeout)
        {
            if (millisecondsTimeout < Timeout.Infinite)
                throw new ArgumentOutOfRangeException(nameof(millisecondsTimeout), millisecondsTimeout, SCL_CORE_THREADING_EXCEPTIONS.INVALID_TIMEOUT_VALUE);

            int count = m_count;
            if (count > 0 && Interlocked.CompareExchange(ref m_count, count - 1, count) == count)
                return true;
            
            if (millisecondsTimeout == 0)
                return false;

            SpinWait sw = default;

            uint startTime =
                millisecondsTimeout != Timeout.Infinite
                ? (uint)Environment.TickCount
                : uint.MaxValue;

            do
            {
                sw.SpinOnce();

                count = m_count;
                if (count > 0 && Interlocked.CompareExchange(ref m_count, count - 1, count) == count)
                    return true;
                else if (sw.NextSpinWillYield &&
                    (uint)millisecondsTimeout <= (uint)Environment.TickCount - startTime)
                    return false;
            } while (true);
        }

        public void Enter() => TryEnter(Timeout.Infinite);

        public void Exit()
        {
            int count = m_count;
            if (count >= m_initCount)
                throw new InvalidOperationException(SCL_CORE_THREADING_EXCEPTIONS.INVALID_COUNT);

            Interlocked.Increment(ref count);
        }

        public void Reset() => Interlocked.Exchange(ref m_count, m_initCount);
    }
}
