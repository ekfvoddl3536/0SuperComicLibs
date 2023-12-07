// MIT License
//
// Copyright (c) 2023. SuperComic (ekfvoddl3535@naver.com)
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

using System.Collections;
using System.Diagnostics;

namespace SuperComicLib.Tensor;

[DebuggerTypeProxy(typeof(Debugging.InternalVectorDebugView<>))]
[DebuggerDisplay("vec({length})")]
[StructLayout(LayoutKind.Sequential, Pack = 0x10)]
public readonly unsafe struct Vector1D<T> : IEquatable<Vector1D<T>>, IEnumerable<T>
    where T : unmanaged
{
    public readonly T* ptr;
    public readonly long length;

    #region constructor
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector1D(void* ptr, long length)
    {
        this.ptr = (T*)ptr;
        this.length = length;
    }

    /// <param name="memory">
    /// This is a reference to a pinned managed array.<br/>
    /// The reference must be maintained until the use of this object is completed to prevent the memory from being collected and avoid invalid memory access.
    /// </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector1D(int length, out object? memory)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(length);

        var vs = GC.AllocateArray<T>(length, true);
        memory = vs;

        ptr = (T*)Unsafe.AsPointer(ref MemoryMarshal.GetArrayDataReference(vs));

        this.length = (uint)length;
    }
    #endregion

    #region indexer
    public ref T this[long index] => ref ptr[index];
    #endregion

    #region methods
    #region equals
    /// <summary>
    /// Simple pointer address and length equality comparison.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(Vector1D<T> other) => ptr == other.ptr & length == other.length;

    /// <summary>
    /// Deep equality comparison based on raw values. 
    /// (Performs a comparison of shape and all elements for equality, without performing reference comparison)
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool SequenceEqual(Vector1D<T> other) => SequenceEqual(other, null);

    /// <summary>
    /// Deep equality comparison based on raw values. 
    /// (Performs a comparison of shape and all elements for equality, without performing reference comparison)
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool SequenceEqual(Vector1D<T> other, IEqualityComparer<T>? comparer) =>
        length == other.length && this.SequenceEqual((IEnumerable<T>)other, comparer);
    #endregion

    #region slice
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector1D<T> Slice(long startIndex) => Slice(startIndex, length - startIndex);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector1D<T> Slice(long startIndex, long length) => new Vector1D<T>(ptr + startIndex, length);
    #endregion

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<T> AsSpan() => new Span<T>(ptr, (int)length);

    public IEnumerator<T> GetEnumerator() => new Enumerator(this);
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public PointerEnumerable<T> AsPointerEnumerable() => new PointerEnumerable<T>(ptr, ptr + length);
    #endregion

    #region override
    public override int GetHashCode() => ((ulong)ptr).GetHashCode();
    public override bool Equals(object? obj) => obj is Vector1D<T> other && Equals(other);
    #endregion

    #region operator
    /// <summary>
    /// Fast and simple pointer address equality comparison.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(in Vector1D<T> left, in Vector1D<T> right) => left.ptr == right.ptr;
    /// <summary>
    /// Fast and simple pointer address in-equality comparison.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(in Vector1D<T> left, in Vector1D<T> right) => left.ptr != right.ptr;
    #endregion

    #region static method
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector1D<T> Alloc(long length) => 
        new Vector1D<T>(NativeMemory.Alloc((nuint)length, (nuint)sizeof(T)), length);
    #endregion

    #region nested struct
    [StructLayout(LayoutKind.Sequential, Pack = 0x10)]
    private struct Enumerator : IEnumerator<T>
    {
        private T* _last;
        private readonly T* _end;
        private readonly T* _first;

        public Enumerator(in Vector1D<T> vector)
        {
            _last = vector.ptr - 1;
            _end = vector.ptr + vector.length;
            _first = vector.ptr;
        }

        public readonly T Current => *_last;
        readonly object IEnumerator.Current => *_last;
        public bool MoveNext() => ++_last != _end;
        public void Reset() => _last = _first - 1;
        public readonly void Dispose() { }
    }
    #endregion
}
