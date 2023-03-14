// MIT License
//
// Copyright (c) 2019-2023. SuperComic (ekfvoddl3535@naver.com)
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
using System.Threading;

namespace SuperComicLib.Threading
{
    public struct SpinBlockLockSlim : ISpinLockBase
    {
        internal const int STATE_UNLOCK = 0;
        internal const int STATE_LOCK = 1;

        private int m_state;

        public bool IsLocked
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => m_state == STATE_LOCK;
        }

        public bool TryEnter(int millisecondsTimeout)
        {
            ENSURE_TIMEOUT_ARG(millisecondsTimeout);

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

        internal void EnterCoreInf()
        {
            for (SpinWait sw = default; ;)
            {
                sw.SpinOnce();

                if (TryEnterOnce())
                    break;
            }
        }

        internal bool EnterCore(long endTicks)
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal bool TryEnterOnce() => Interlocked.CompareExchange(ref m_state, STATE_LOCK, STATE_UNLOCK) == STATE_UNLOCK;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Enter() => TryEnter(Timeout.Infinite);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Exit()
        {
            EnsureExitState_DEBUG();

            Interlocked.Exchange(ref m_state, STATE_UNLOCK);
        }

        [Conditional("DEBUG")]
        private void EnsureExitState_DEBUG()
        {
            if (IsLocked == false)
                throw new BadThreadEnterExitFlowException(nameof(SpinBlockLockSlim));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ENSURE_TIMEOUT_ARG(int millisecondsTimeout)
        {
            if (millisecondsTimeout < Timeout.Infinite)
                throw new ArgumentOutOfRangeException(nameof(millisecondsTimeout), millisecondsTimeout, SCL_CORE_THREADING_EXCEPTIONS.INVALID_TIMEOUT_VALUE);
        }
    }
}
