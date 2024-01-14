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
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace SuperComicLib.Threading
{
    [StructLayout(LayoutKind.Sequential)]
    public struct SpinCountdownLockSlim : ISpinLockBase
    {
        private volatile int m_count;
        private readonly int m_initCount;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SpinCountdownLockSlim(int initCount)
        {
            m_count = initCount;
            m_initCount = initCount;
        }

        public bool IsLocked => m_count == 0;

        public bool TryEnter(int millisecondsTimeout)
        {
            SpinBlockLockSlim.ENSURE_TIMEOUT_ARG(millisecondsTimeout);

            if (TryEnterOnce())
                return true;

            if (millisecondsTimeout == 0)
                return false;

            if (millisecondsTimeout == Timeout.Infinite)
            {
                EnterCoreInf();
                return true;
            }
            else
                return EnterCore(millisecondsTimeout * TimeSpan.TicksPerMillisecond);
        }

        private void EnterCoreInf()
        {
            for (SpinWait sw = default; ;)
            {
                sw.SpinOnce();

                if (TryEnterOnce())
                    break;
            }
        }

        private bool EnterCore(long endTicks)
        {
            long elapsedTicks = 0;
            for (SpinWait sw = default; ;)
            {
                var startT = Environment.TickCount;

                sw.SpinOnce();

                if (TryEnterOnce())
                    return true;

                elapsedTicks += (uint)(Environment.TickCount - startT);
                if ((ulong)elapsedTicks >= (ulong)endTicks)
                    return false;
            }
        }

        public void Enter() => TryEnter(Timeout.Infinite);

        public void Exit()
        {
            EnsureExitState_DEBUG();

            Interlocked.Increment(ref m_count);
        }

        public void Reset() => Interlocked.Exchange(ref m_count, m_initCount);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool TryEnterOnce()
        {
            int count = m_count;
            return count > 0 && Interlocked.CompareExchange(ref m_count, count - 1, count) == count;
        }

        [Conditional("DEBUG")]
        private void EnsureExitState_DEBUG()
        {
            int count = m_count;
            if ((uint)count >= (uint)m_initCount)
                throw new BadThreadEnterExitFlowException(nameof(SpinBlockLockSlim));
        }
    }
}
