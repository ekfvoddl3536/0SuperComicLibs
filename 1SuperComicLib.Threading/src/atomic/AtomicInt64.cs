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
    public struct AtomicInt64
    {
        private long m_value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public AtomicInt64(long value) => m_value = value;

        public long Value
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)] // don't cache
            get => Volatile.Read(ref m_value);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => Interlocked.Exchange(ref m_value, value);
        }

        /// <summary>
        /// see: <see cref="Interlocked.Add(ref long, long)"/>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long Add(long value) => Interlocked.Add(ref m_value, value);

        /// <summary>
        /// see: <see cref="Interlocked.CompareExchange(ref long, long, long)"/>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long CompareExchange(long newValue)
        {
            var observedValue = Value;
            return Interlocked.CompareExchange(ref m_value, newValue, observedValue);
        }

        /// <summary>
        /// Exchange value
        /// </summary>
        /// <param name="predicate"><c>bool(long oldValue, long newValue)</c>. If it returns true, it will attempt to update with the new value.</param>
        /// <returns>Old value</returns>
        public long ExchangeIf(long newValue, Func<long, long, bool> predicate)
        {
            for (SpinWait sw = default; ;)
            {
                var observedValue = Value;
                if (predicate.Invoke(observedValue, newValue) &&
                    Interlocked.CompareExchange(ref m_value, newValue, observedValue) != observedValue)
                    sw.SpinOnce();
                else
                    return observedValue;
            }
        }
    }
}
