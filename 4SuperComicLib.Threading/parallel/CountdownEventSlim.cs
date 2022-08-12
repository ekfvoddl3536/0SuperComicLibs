using System;
using System.Threading;

namespace SuperComicLib.Threading
{
    public sealed class CountdownEventSlim : IDisposable
    {
        private int m_initCount;
        private volatile int m_current;
        private ManualResetEvent m_event;

        public CountdownEventSlim() : this(Environment.ProcessorCount) { }

        public CountdownEventSlim(int initCount)
        {
            m_initCount =
                initCount > 1
                ? initCount
                : 1;

            m_current = m_initCount;
            m_event = new ManualResetEvent(false);
        }

        public int CurrentCount => m_current;

        public int InitialCount => m_initCount;

        public bool IsSet => m_current <= 0;

        public WaitHandle WaitHandle => m_event ?? throw new ObjectDisposedException(nameof(CountdownEventSlim));

        public void Reset()
        {
            m_event.Reset();
            m_current = m_initCount;
        }

        public void Signal()
        {
            if (Interlocked.Decrement(ref m_current) == 0)
            {
                ManualResetEvent e = m_event;
                e.Set();
                e.Reset();
            }
        }

        public void Exit() => Interlocked.Decrement(ref m_initCount);

        public bool Wait(int millisecondsTimeout, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Wait(millisecondsTimeout);
        }

        public bool Wait(int millisecondsTimeout)
        {
            bool retval = m_current <= 0;
            if (!retval)
                retval = m_event.WaitOne(millisecondsTimeout, default);

            return retval;
        }

        public bool Wait() => Wait(Timeout.Infinite);

        public void Dispose()
        {
            if (m_event != null)
            {
                m_initCount = 0;
                m_current = 0;
                m_event = null;
            }
            GC.SuppressFinalize(this);
        }
    }
}
