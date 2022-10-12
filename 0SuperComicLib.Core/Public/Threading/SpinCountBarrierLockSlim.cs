// MIT License
//
// Copyright (c) 2019-2022 SuperComic (ekfvoddl3535@naver.com)
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
    using static SpinBlockLockSlim;
    public struct SpinCountBarrierLockSlim : ISpinLockBase
    {
        private volatile int m_state;
        private volatile int m_count;

        public bool IsLocked => m_state == STATE_LOCK;

        public bool TryEnter(int millisecondsTimeout)
        {
            if (millisecondsTimeout < Timeout.Infinite)
                throw new ArgumentOutOfRangeException(nameof(millisecondsTimeout), millisecondsTimeout, SCL_CORE_THREADING_EXCEPTIONS.INVALID_TIMEOUT_VALUE);

            int state = m_state;
            if (state != STATE_UNLOCK) // 잠금이 아닐 경우 spin wait
            {
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

                    state = m_state;
                    if (state == STATE_UNLOCK) // 잠금이 풀린 경우 break
                        break;
                    else if (sw.NextSpinWillYield &&
                        (uint)millisecondsTimeout <= (uint)Environment.TickCount - startTime)
                        return false;
                } while (true);
            }

            Interlocked.Increment(ref m_count);
            return true;
        }

        public void Enter() => TryEnter(Timeout.Infinite);

        public void Exit() => Interlocked.Decrement(ref m_count);

        public void Blocking()
        {
            SpinWait sw = default;

            loop:
            if (Interlocked.CompareExchange(ref m_state, STATE_LOCK, STATE_UNLOCK) == STATE_UNLOCK)
                do
                {
                    int count = m_count;
                    if (count == 0)
                        break;

                    sw.SpinOnce();
                }
                while (true);
            else
            {
                sw.SpinOnce();
                goto loop;
            }
        }

        public void Release() =>
            Interlocked.Exchange(ref m_state, STATE_UNLOCK);

        public void ReleaseAndEnter()
        {
            Interlocked.Increment(ref m_count);
            Interlocked.Exchange(ref m_state, STATE_UNLOCK);
        }
    }
}
