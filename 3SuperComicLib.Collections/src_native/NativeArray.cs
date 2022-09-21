using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SuperComicLib.Collections
{
    [StructLayout(LayoutKind.Sequential)]
    public readonly unsafe partial struct NativeArray<T> : IDisposable, IRawContainer<T>, IReadOnlyRawContainer<T>, IEquatable<NativeArray<T>>
        where T : unmanaged
    {
        #region iterator
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public _iterator<T> begin() => new _iterator<T>(Ptr);
        // !== end()는 따로 구현

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public reverse_iterator<T> rend() => new reverse_iterator<T>(Ptr - 1);
        // !== rbegin()은 따로 구현

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public const_iterator<T> cbegin() => begin();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public const_iterator<T> cend() => end();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public const_reverse_iterator<T> crbegin() => rbegin();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public const_reverse_iterator<T> crend() => rend();
        #endregion

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose() => Marshal.FreeHGlobal((IntPtr)Ptr);
    }
}
