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
    public struct SpinCountBarrierLockSlim : ISpinLockBase
    {
        private SpinBlockLockSlim m_lock;
        private int m_count;

        public bool IsLocked => m_lock.IsLocked;

        public bool TryEnter(int millisecondsTimeout)
        {
            SpinBlockLockSlim.ENSURE_TIMEOUT_ARG(millisecondsTimeout);

            if (m_lock.IsLocked)
            {
                if (millisecondsTimeout == 0)
                    return false;

                if (millisecondsTimeout == Timeout.Infinite)
                    m_lock.EnterCoreInf();
                else if (!m_lock.EnterCore(millisecondsTimeout * TimeSpan.TicksPerMillisecond))
                    return false;
            }

            Interlocked.Increment(ref m_count);
            return true;
        }

        public void Enter() => TryEnter(Timeout.Infinite);

        public void Exit()
        {
            EnsureExitState_DEBUG();

            Interlocked.Decrement(ref m_count);
        }

        public void Blocking()
        {
            SpinWait sw = default;
            while (!m_lock.TryEnterOnce())
                sw.SpinOnce();

            sw.Reset();
            while (m_count != 0)
                sw.SpinOnce();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Release() => m_lock.Exit();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ReleaseAndEnter()
        {
            Interlocked.Increment(ref m_count);
            m_lock.Exit();
        }

        [Conditional("DEBUG")]
        private void EnsureExitState_DEBUG()
        {
            int count = m_count;
            if (count <= 0)
                throw new BadThreadEnterExitFlowException(nameof(SpinCountBarrierLockSlim));
        }
    }
}