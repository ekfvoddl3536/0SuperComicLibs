using System;
using System.Threading;

namespace SuperComicLib.Threading
{
    public struct SpinBlockLockSlim : ISpinLockBase
    {
        internal const int STATE_UNLOCK = 0;
        internal const int STATE_LOCK = 1;

        private volatile int m_state;

        public bool IsLocked
        {
            get
            {
                int state = m_state;
                return state == STATE_LOCK;
            }
        }

        public bool TryEnter(int millisecondsTimeout)
        {
            if (millisecondsTimeout < Timeout.Infinite)
                throw new ArgumentOutOfRangeException(nameof(millisecondsTimeout), millisecondsTimeout, SCL_CORE_THREADING_EXCEPTIONS.INVALID_TIMEOUT_VALUE);

            if (Interlocked.CompareExchange(ref m_state, STATE_LOCK, STATE_UNLOCK) == STATE_UNLOCK)
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

                if (Interlocked.CompareExchange(ref m_state, STATE_LOCK, STATE_UNLOCK) == STATE_UNLOCK)
                    return true;
                else if (sw.NextSpinWillYield &&
                    (uint)millisecondsTimeout <= (uint)Environment.TickCount - startTime)
                    return false;
            } while (true);
        }

        public void Enter() => TryEnter(Timeout.Infinite);

        public void Exit() => Interlocked.Exchange(ref m_state, STATE_UNLOCK);
    }
}
