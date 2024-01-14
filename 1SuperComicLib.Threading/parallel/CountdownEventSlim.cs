// MIT License
//
// Copyright (c) 2019-2024. SuperComic (ekfvoddl3535@naver.com)
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

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
