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
using System.Threading;
using static SuperComicLib.Threading.SpinBlockLockSlim;

namespace SuperComicLib.Threading
{
    /// <summary>
    /// Expresses up to 32 lock states.
    /// </summary>
    public struct SpinMultistateLockSlim : ISpinLockBase
    {
        private volatile int m_state;

        public bool IsLockedAnyState => m_state != STATE_UNLOCK;
        public bool IsLocked(int bitFlag_index) => IsLockedAll(1 << bitFlag_index);
        public bool IsLockedAll(int bitFlagMaskMap) => (m_state & bitFlagMaskMap) == bitFlagMaskMap;
        public bool IsLockedAny(int bitFlagMaskMap) => (m_state & bitFlagMaskMap) != STATE_UNLOCK;

        bool ISpinLockBase.IsLocked => IsLockedAnyState;
        public int LockedState => m_state;

        bool ISpinLockBase.TryEnter(int millisecondsTimeout) => TryEnter(millisecondsTimeout, 0);
        void ISpinLockBase.Enter() => TryEnter(Timeout.Infinite, 0);
        void ISpinLockBase.Exit() => Exit(0);

        /// <param name="bitFlag_index">The index of the bit to mark as lock.</param>
        public void Enter(int bitFlag_index) => TryEnter(Timeout.Infinite, bitFlag_index);

        /// <param name="bitFlag_index">The index of the bit to mark as lock.</param>
        public bool TryEnter(int millisecondsTimeout, int bitFlag_index) => TryEnterAll(millisecondsTimeout, 1 << bitFlag_index);

        /// <summary>
        /// Acquires a lock for all specified states.
        /// </summary>
        /// <param name="bitFlagMaskMap">A bit mask map with the bit of the index you want to acquire a lock set to 1.</param>
        public void EnterAll(int bitFlagMaskMap) => TryEnterAll(Timeout.Infinite, bitFlagMaskMap);

        /// <summary>
        /// Attempts to acquire locks for all specified states.
        /// </summary>
        /// <param name="bitFlagMaskMap">A bit mask map with the bit of the index you want to acquire a lock set to 1.</param>
        public bool TryEnterAll(int millisecondsTimeout, int bitFlagMaskMap)
        {
            ENSURE_TIMEOUT_ARG(millisecondsTimeout);

            if (TryEnterOnce(bitFlagMaskMap))
                return true;

            if (millisecondsTimeout == 0)
                return false;

            if (millisecondsTimeout == Timeout.Infinite)
            {
                EnterAllCoreInf(bitFlagMaskMap);
                return true;
            }
            else
                return EnterAllCore(millisecondsTimeout * TimeSpan.TicksPerMillisecond, bitFlagMaskMap);
        }

        private void EnterAllCoreInf(int bitFlagMaskMap)
        {
            for (SpinWait sw = default; ;)
            {
                sw.SpinOnce();

                if (TryEnterOnce(bitFlagMaskMap))
                    break;
            }
        }

        private bool EnterAllCore(long endTicks, int bitFlagMaskMap)
        {
            long elapsedTicks = 0;
            for (SpinWait sw = default; ;)
            {
                var startT = Environment.TickCount;

                sw.SpinOnce();

                if (TryEnterOnce(bitFlagMaskMap))
                    return true;

                elapsedTicks += (uint)(Environment.TickCount - startT);
                if ((ulong)elapsedTicks >= (ulong)endTicks)
                    return false;
            }
        }

        /// <param name="bitFlag_index">The index of the bit to mark as unlocked.</param>
        public void Exit(int bitFlag_index) => ExitAll(1 << bitFlag_index);

        /// <param name="bitFlagMaskMap">A bitmask map with the bits at the indices to unlock set to 1..</param>
        public void ExitAll(int bitFlagMaskMap)
        {
            EnsureExitALLState_DEBUG(bitFlagMaskMap);

            for (SpinWait sw = default; TryExitOnce(bitFlagMaskMap) == false;)
                sw.SpinOnce();
        }

        /// <param name="bitFlagMaskMap">A bitmask map with the bits at the indices to unlock set to 1.</param>
        /// <returns>A bitmask map with bits set to 1 for successfully unlocked indices.</returns>
        public int ExitAny(int bitFlagMaskMap)
        {
            int result;
            for (SpinWait sw = default; TryExitAnyOnce(bitFlagMaskMap, out result) == false;)
                sw.SpinOnce();

            return result;
        }

        public void WaitUntilExit(int bitFlag_index) => WaitUntilExitAll(1 << bitFlag_index);

        public void WaitUntilExitAll(int bitFlagMaskMap)
        {
            for (SpinWait sw = default; IsLockedAny(bitFlagMaskMap);)
                sw.SpinOnce();
        }

        public int WaitUntilExitAny(int bitFlagMaskMap)
        {
            int state;
            for (SpinWait sw = default; ;)
            {
                state = m_state;
                if ((state & bitFlagMaskMap) != bitFlagMaskMap)
                    break;

                sw.SpinOnce();
            }

            return state & bitFlagMaskMap;
        }

        private bool TryEnterOnce(int bitFlagMaskMap)
        {
            int state = m_state & ~bitFlagMaskMap;
            return Interlocked.CompareExchange(ref m_state, state | bitFlagMaskMap, state) == state;
        }

        private bool TryExitOnce(int bitFlagMaskMap)
        {
            int state = m_state | bitFlagMaskMap;
            return Interlocked.CompareExchange(ref m_state, state & ~bitFlagMaskMap, state) == state;
        }

        private bool TryExitAnyOnce(int bitFlagMaskMap, out int result)
        {
            result = m_state & bitFlagMaskMap;

            int state = m_state | result;
            return Interlocked.CompareExchange(ref m_state, state & ~bitFlagMaskMap, state) == state;
        }

        [Conditional("DEBUG")]
        private void EnsureExitALLState_DEBUG(int bitFlagMaskMap)
        {
            if (IsLockedAll(bitFlagMaskMap) == false)
                throw new InvalidOperationException(nameof(SpinMultistateLockSlim));
        }
    }
}
