using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SuperComicLib.Collections
{
    [StructLayout(LayoutKind.Sequential)]
    public readonly unsafe struct NativeArray<T> : IDisposable, IRawContainer<T>, IReadOnlyRawContainer<T>, IEquatable<NativeArray<T>>
        where T : unmanaged
    {
        public readonly T* Ptr;
        public readonly int Length;

        #region constructors
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeArray(int length, bool initDefault)
        {
            if (length <= 0)
                throw new ArgumentOutOfRangeException(nameof(length));

            Length = length;
            Ptr = (T*)Marshal.AllocHGlobal(sizeof(T) * length);

            if (initDefault)
                RawContainerUtility.Internal_Clear(Ptr, Ptr + length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeArray(T[] source) : this(source?.Length ?? 0, false)
        {
            fixed (T* psrc = &source[0])
            {
                ulong cb = (ulong)(uint)source.Length * (uint)sizeof(T);
                Buffer.MemoryCopy(psrc, Ptr, cb, cb);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeArray(T* hGlobalAllocated_Ptr, int length)
        {
            Ptr = hGlobalAllocated_Ptr;
            Length = length;
        }
        #endregion

        #region indexer
        public ref T this[int index] => ref Ptr[index];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T at(int index)
        {
            if ((uint)index >= (uint)Length)
                throw new ArgumentOutOfRangeException(nameof(index));

            return ref this[index];
        }
        #endregion

        #region methods
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeSpan<T> AsSpan() => new NativeSpan<T>(Ptr, Length);
        #endregion

        #region explicit implement interfaces
        ref readonly T IReadOnlyRawContainer<T>.this[int index] => ref this[index];
        ref readonly T IReadOnlyRawContainer<T>.at(int index) => ref at(index);

        int IRawContainer.capacity() => Length;
        int IRawContainer.size() => Length;
        #endregion

        #region implement interfaces
        public RawMemory getMemory() => new RawMemory(Ptr, Length);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public _iterator<T> begin() => new _iterator<T>(Ptr);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public _iterator<T> end() => new _iterator<T>(Ptr + Length);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public reverse_iterator<T> rbegin() => new reverse_iterator<T>(Ptr + (Length - 1));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public reverse_iterator<T> rend() => new reverse_iterator<T>(Ptr - 1);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public const_iterator<T> cbegin() => begin();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public const_iterator<T> cend() => end();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public const_reverse_iterator<T> crbegin() => rbegin();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public const_reverse_iterator<T> crend() => rend();

        public void Dispose() => Marshal.FreeHGlobal((IntPtr)Ptr);
        #endregion

        #region explicit impl interface
        bool IEquatable<NativeArray<T>>.Equals(NativeArray<T> other) => this == other;
        #endregion

        #region override
        public override bool Equals(object obj) => obj is NativeArray<T> other && this == other;
        public override int GetHashCode() => ((IntPtr)Ptr).GetHashCode() ^ Length;
        #endregion

        #region static members
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(in NativeArray<T> left, in NativeArray<T> right) => left.Ptr == right.Ptr;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(in NativeArray<T> left, in NativeArray<T> right) => left.Ptr != right.Ptr;
        #endregion
    }
}
