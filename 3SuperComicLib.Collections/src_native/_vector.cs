#pragma warning disable IDE1006 // 명명 스타일
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SuperComicLib.Collections
{
    [StructLayout(LayoutKind.Sequential), CodeContracts.ParamRef]
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
