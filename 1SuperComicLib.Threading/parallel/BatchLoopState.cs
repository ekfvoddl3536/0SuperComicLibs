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
using System.Runtime.CompilerServices;
using System.Threading;

namespace SuperComicLib.Threading
{
    public sealed class BatchLoopState
    {
        public const int STATE_BREAK = 1, STATE_STOP = 2;

        internal volatile int m_state;
        internal AtomicInt32 m_lowestBreak;

        public void Stop()
        {
            if (Interlocked.CompareExchange(ref m_state, STATE_STOP, 0) == STATE_BREAK)
                throw new InvalidOperationException("called 'Break' already.");
        }

        public void Break()
        {
            if (Interlocked.CompareExchange(ref m_state, STATE_BREAK, 0) == STATE_STOP)
                throw new InvalidOperationException("called 'Stop' already.");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal bool NeedBreak(int index) =>
            m_state == STATE_STOP ||
            m_state == STATE_BREAK && index >= m_lowestBreak.Value;
    }
}