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

using System.Diagnostics;

namespace SuperComicLib.Tensor;

[DebuggerDisplay("mat({width}, {height})")]
[StructLayout(LayoutKind.Sequential, Pack = 0x10)]
public readonly unsafe struct Matrix2D<T> : IEquatable<Matrix2D<T>>
    where T : unmanaged
{
    public readonly T* ptr;
    public readonly uint width;
    public readonly uint height;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Matrix2D(void* ptr, uint width, uint height)
    {
        this.ptr = (T*)ptr;
        this.width = width;
        this.height = height;
    }

    /// <param name="memory">
    /// This is a reference to a pinned managed array.<br/>
    /// The reference must be maintained until the use of this object is completed to prevent the memory from being collected and avoid invalid memory access.
    /// </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Matrix2D(uint width, uint height, out object? memory)
    {
        var size = width * (ulong)height;
        if (size >> 31 != 0)
            throw new ArgumentException(SR.MATRIX2D_OVERFLOW);

        var vs = GC.AllocateArray<T>((int)size, true);
        memory = vs;

        ptr = (T*)Unsafe.AsPointer(ref MemoryMarshal.GetArrayDataReference(vs));

        this.width = width;
        this.height = height;
    }

    public Vector1D<T> AsVector() => new Vector1D<T>(ptr, (long)size);

    public Vector1D<T> this[long width_index] => new Vector1D<T>(ptr + height * width_index, height);

    public ref T this[long width_index, long height_index] => ref ptr[height * width_index + height_index];

    public ulong size => width * (ulong)height;

    public Shape shape => (width, height);

    #region equals
    /// <summary>
    /// Simple pointer address and shape equality comparison.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(Matrix2D<T> other) =>
        ptr == other.ptr &
        Unsafe.As<uint, long>(ref Unsafe.AsRef(in width)) == Unsafe.As<uint, long>(ref Unsafe.AsRef(in other.width));

    /// <summary>
    /// Deep equality comparison based on raw values. 
    /// (Performs a comparison of shape and all elements for equality, without performing reference comparison)
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool SequenceEqual(Matrix2D<T> other) => SequenceEqual(other, null);

    /// <summary>
    /// Deep equality comparison based on raw values. 
    /// (Performs a comparison of shape and all elements for equality, without performing reference comparison)
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool SequenceEqual(Matrix2D<T> other, IEqualityComparer<T>? comparer) =>
        Unsafe.As<uint, long>(ref Unsafe.AsRef(in width)) == Unsafe.As<uint, long>(ref Unsafe.AsRef(in other.width)) &&
        AsVector().SequenceEqual((IEnumerable<T>)other.AsVector(), comparer);
    #endregion

    #region slice
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Matrix2D<T> Slice(Shape startIndex) => 
        Slice(startIndex, (width - startIndex.dim1, height - startIndex.dim2));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Matrix2D<T> Slice(Shape startIndex, Shape length)
    {
        Debug.Assert((startIndex.rank | length.rank) == 2);
        return new Matrix2D<T>(
            ptr + height * startIndex.dim1 + startIndex.dim2,
            length.dim1,
            length.dim2);
    }
    #endregion

    #region override
    public override int GetHashCode() => ((ulong)ptr).GetHashCode();
    public override bool Equals(object? obj) => obj is Matrix2D<T> other && Equals(other);
    #endregion

    #region opreator
    /// <summary>
    /// Fast and simple pointer address equality comparison.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(Matrix2D<T> left, Matrix2D<T> right) => left.ptr == right.ptr;
    /// <summary>
    /// Fast and simple pointer address in-equality comparison.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(Matrix2D<T> left, Matrix2D<T> right) => left.ptr != right.ptr;
    #endregion

    #region static method
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix2D<T> Alloc(uint width, uint height) =>
        new Matrix2D<T>(NativeMemory.Alloc((nuint)width * height, (nuint)sizeof(T)), width, height);
    #endregion
}
