#if X86
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SuperComicLib.Collections
{
    unsafe partial struct NativeArray<T>
    {
        public readonly T* Ptr;
        public readonly int Length;

        #region constructors
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeArray(T* hGlobalAllocated_Ptr, int length)
        {
            Ptr = hGlobalAllocated_Ptr;
            Length = length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeArray(int length, bool initDefault)
        {
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length));

            Length = length;
            Ptr = (T*)Marshal.AllocHGlobal(length * sizeof(T));

            if (initDefault)
                MemoryBlock.Clear32((byte*)Ptr, (uint)length * (uint)sizeof(T));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeArray(T[] source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            Length = source.Length;
            Ptr = (T*)Marshal.AllocHGlobal(source.Length * sizeof(T));

            if (source.Length > 0)
                fixed (T* psrc = &source[0])
                {
                    ulong cb = (uint)source.Length * (uint)sizeof(T);
                    Buffer.MemoryCopy(psrc, Ptr, cb, cb);
                }
        }
        #endregion

        #region indexer & property
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

        int IRawContainer.size() => Length;
        int IRawContainer.capacity() => Length;
        #endregion

        #region implement interfaces
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RawMemory getMemory() => new RawMemory(Ptr, Length);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public _iterator<T> end() => new _iterator<T>(Ptr + Length);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public reverse_iterator<T> rbegin() => new reverse_iterator<T>(Ptr + (Length - 1));
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
        public static bool ReferenceEquals(in NativeArray<T> left, in NativeArray<T> right) => left.Ptr == right.Ptr;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(in NativeArray<T> left, in NativeArray<T> right) =>
            left.Ptr == right.Ptr && left.Length == right.Length;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(in NativeArray<T> left, in NativeArray<T> right) =>
            left.Ptr != right.Ptr || left.Length != right.Length;
        #endregion
    }
}
#endif