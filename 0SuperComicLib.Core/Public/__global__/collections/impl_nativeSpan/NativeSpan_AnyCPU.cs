#if AnyCPU
#pragma warning disable IDE1006 // 명명 스타일
using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace SuperComicLib
{
    unsafe partial struct NativeSpan<T>
    {
        public readonly T* Source;
        private readonly void* _length;

        #region constructors
        [MethodImpl(MethodImplOptions.AggressiveInlining), CodeContracts.X64Only]
        public NativeSpan(T* source, size_t length)
        {
            Source = source;
            _length = length.value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining), CodeContracts.X64Only]
        public NativeSpan(T* start, T* end)
        {
            Source = start;
            _length = (void*)(end - start);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeSpan(_iterator<T> start, size_t length) : this(start._ptr, length)
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining), CodeContracts.X64Only]
        public NativeSpan(_iterator<T> start, _iterator<T> end) : this(start._ptr, end._ptr)
        {
        }
        #endregion

        #region property
        public ref T this[int index] => ref *(Source + index);

        [CodeContracts.X64Only]
        public ref T this[long index] => ref *(Source + index);

        [CodeContracts.X64LossOfLength]
        public int Length => (int)_length;

        [CodeContracts.X64Only]
        public long LongLength => (long)_length;
        #endregion

        #region def methods
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeSpan<T> Slice(int startIndex) => Slice(startIndex, Length - startIndex);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeSpan<T> Slice(int startIndex, int length)
        {
            if ((uint)(startIndex + length) > (uint)Length)
                throw new ArgumentOutOfRangeException(nameof(startIndex));

            return new NativeSpan<T>(Source + startIndex, length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T[] ToArray()
        {
            int len = Length;

            if (len <= 0)
                return Array.Empty<T>();

            T[] res = new T[len];

            fixed (T* pdst = &res[0])
                MemoryBlock.Memmove(Source, pdst, _length, sizeof(T));

            return res;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryCopyTo(NativeSpan<T> dst)
        {
            if ((ulong)LongLength <= (ulong)dst.LongLength)
            {
                MemoryBlock.Memmove(Source, dst.Source, _length, sizeof(T));

                return true;
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CopyTo(NativeSpan<T> dst)
        {
            Contract.Requires<ArgumentOutOfRangeException>((ulong)LongLength <= (ulong)dst.LongLength, $"'{nameof(dst)}'");

            MemoryBlock.Memmove(Source, dst.Source, _length, sizeof(T));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Fill(in T value) => MemoryBlock.Memset(Source, _length, value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear() => MemoryBlock.Clear(Source, _length, sizeof(T));
        #endregion

        #region impl methods
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T at(int index)
        {
            Contract.Requires<ArgumentOutOfRangeException>((uint)index < (uint)Length, $"index: {index} / length: {Length}");
            return ref *(Source + index);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining), CodeContracts.X64Only]
        public ref T at(long index)
        {
            Contract.Requires<ArgumentOutOfRangeException>((ulong)index < (ulong)LongLength, $"index: {index} / length: {Length}");
            return ref *(Source + index);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining), CodeContracts.X64Only]
        public _iterator<T> begin() => new _iterator<T>(Source);
        [MethodImpl(MethodImplOptions.AggressiveInlining), CodeContracts.X64Only]
        public _iterator<T> end() =>
            IntPtr.Size == sizeof(int)
            ? new _iterator<T>(Source + Length)
            : new _iterator<T>(Source + LongLength);

        [MethodImpl(MethodImplOptions.AggressiveInlining), CodeContracts.X64Only]
        public reverse_iterator<T> rbegin() =>
            IntPtr.Size == sizeof(int)
            ? new reverse_iterator<T>(Source + (Length - 1))
            : new reverse_iterator<T>(Source + (LongLength - 1));
        [MethodImpl(MethodImplOptions.AggressiveInlining), CodeContracts.X64Only]
        public reverse_iterator<T> rend() => new reverse_iterator<T>(Source - 1);
        #endregion

        #region static members
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(NativeSpan<T> left, NativeSpan<T> right) =>
            left.Source == right.Source && left._length == right._length;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(NativeSpan<T> left, NativeSpan<T> right) =>
            left.Source != right.Source || left._length != right._length;
        #endregion
    }
}
#endif