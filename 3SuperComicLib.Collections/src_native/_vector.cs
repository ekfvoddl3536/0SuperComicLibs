#pragma warning disable IDE1006 // 명명 스타일
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SuperComicLib.Collections
{
    [StructLayout(LayoutKind.Sequential), CodeContracts.ParamRef]
    public unsafe partial struct _vector<T> : IRawList<T>
        where T : unmanaged
    {
        private T* m_Ptr;
        private T* m_Last;
        private T* m_End;

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

        #region common
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeSpan<T> AsSpan() => new NativeSpan<T>(begin(), end());
        #endregion
    }
}
