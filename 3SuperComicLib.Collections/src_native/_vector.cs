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

#pragma warning disable IDE1006 // 명명 스타일
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SuperComicLib.Collections
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe partial struct _vector<T> : IRawList<T>, IReadOnlyRawContainer<T>, IDisposable
        where T : unmanaged
    {
        private T* m_Ptr;
        private T* m_Last;
        private T* m_End;

        #region common constructor
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public _vector(in _vector<T> source) : this(source.cbegin(), source.cend())
        {
        }
        #endregion

        #region iterator
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public _iterator<T> begin() => new _iterator<T>(m_Ptr);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public _iterator<T> end() => new _iterator<T>(m_Last);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public reverse_iterator<T> rbegin() => new reverse_iterator<T>(m_Last - 1);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public reverse_iterator<T> rend() => new reverse_iterator<T>(m_Ptr - 1);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public const_iterator<T> cbegin() => begin();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public const_iterator<T> cend() => end();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public const_reverse_iterator<T> crbegin() => rbegin();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public const_reverse_iterator<T> crend() => rend();
        #endregion

        #region common collection control method
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T pop_back() =>
            m_Ptr == m_Last
            ? throw new InvalidOperationException("empty collection")
            : *m_Last--;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T pop_back_unsafe() => *m_Last--;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void fast_clear() => m_Last = m_Ptr;
        #endregion

        #region common
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeSpan<T> AsSpan() => new NativeSpan<T>(begin(), end());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose() => Marshal.FreeHGlobal((IntPtr)m_Ptr);
        #endregion
    }
}
