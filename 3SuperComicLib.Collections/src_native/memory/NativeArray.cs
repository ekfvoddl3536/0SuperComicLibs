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

        #region constructor
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

        #region explicit implement interfaces
        ref readonly T IReadOnlyRawContainer<T>.this[int index] => ref this[index];
        ref readonly T IReadOnlyRawContainer<T>.at(int index) => ref at(index);

        int IRawContainer.capacity() => Length;
        int IRawContainer.size() => Length;
        #endregion

        #region implement interfaces
        public RawMemory getMemory() => new RawMemory(Ptr, Length);

        public RawIterator<T> begin() => new RawIterator<T>(Ptr);
        public RawIterator<T> end() => new RawIterator<T>(Ptr + Length);

        public RawReverseIterator<T> rbegin() => new RawReverseIterator<T>(Ptr + (Length - 1));
        public RawReverseIterator<T> rend() => new RawReverseIterator<T>(Ptr - 1);

        public RawConstIterator<T> cbegin() => new RawConstIterator<T>(Ptr);
        public RawConstIterator<T> cend() => new RawConstIterator<T>(Ptr + Length);

        public RawConstReverseIterator<T> crbegin() => new RawConstReverseIterator<T>(Ptr + (Length - 1));
        public RawConstReverseIterator<T> crend() => new RawConstReverseIterator<T>(Ptr - 1);

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
        public static bool operator ==(in NativeArray<T> left, in NativeArray<T> right) => left.Ptr == right.Ptr;
        public static bool operator !=(in NativeArray<T> left, in NativeArray<T> right) => left.Ptr != right.Ptr;
        #endregion
    }
}
