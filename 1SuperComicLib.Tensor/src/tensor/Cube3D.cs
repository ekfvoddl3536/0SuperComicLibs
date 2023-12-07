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

[DebuggerDisplay("cub({width}, {height}, {depth})")]
[StructLayout(LayoutKind.Sequential, Pack = 0x10)]
public readonly unsafe struct Cube3D<T> : IEquatable<Cube3D<T>>
    where T : unmanaged
{
    public readonly T* ptr;
    public readonly uint width;
    public readonly uint height;
    public readonly uint depth;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Cube3D(void* ptr, uint width, uint height, uint depth)
    {
        this.ptr = (T*)ptr;
        this.width = width;
        this.height = height;
        this.depth = depth;
    }

    /// <param name="memory">
    /// This is a reference to a pinned managed array.<br/>
    /// The reference must be maintained until the use of this object is completed to prevent the memory from being collected and avoid invalid memory access.
    /// </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Cube3D(uint width, uint height, uint depth, out object? memory)
    {
        var size = width * (ulong)height;
        if (size >> 31 != 0)
            throw new ArgumentException(SR.MATRIX2D_OVERFLOW);

        size *= depth;
        if (size >> 31 != 0)
            throw new ArgumentOutOfRangeException(nameof(depth));

        var vs = GC.AllocateArray<T>((int)size, true);
        memory = vs;

        ptr = (T*)Unsafe.AsPointer(ref MemoryMarshal.GetArrayDataReference(vs));

        this.width = width;
        this.height = height;
        this.depth = depth;
    }

    public Vector1D<T> AsVector() => new Vector1D<T>(ptr, (long)size);

    public Matrix2D<T> this[long width_index] => 
        new Matrix2D<T>(ptr + height * depth * width_index, height, depth);

    public Vector1D<T> this[long width_index, long height_index] => 
        new Vector1D<T>(ptr + height * depth * width_index + depth * height_index, depth);

    public ref T this[long width_index, long height_index, long depth_index] => 
        ref ptr[height * depth * width_index + depth * height_index + depth_index];

    public ulong size => width * (ulong)height * depth;

    public Shape shape => (width, height, depth);

    #region equals
    /// <summary>
    /// Simple pointer address and shape equality comparison.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(Cube3D<T> other) =>
        ptr == other.ptr &
        Unsafe.As<uint, long>(ref Unsafe.AsRef(in width)) == Unsafe.As<uint, long>(ref Unsafe.AsRef(in other.width)) &
        depth == other.depth;

    /// <summary>
    /// Deep equality comparison based on raw values. 
    /// (Performs a comparison of shape and all elements for equality, without performing reference comparison)
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool SequenceEqual(Cube3D<T> other) => SequenceEqual(other, null);

#pragma warning disable IDE0047 // 불필요한 괄호 제거
    /// <summary>
    /// Deep equality comparison based on raw values. 
    /// (Performs a comparison of shape and all elements for equality, without performing reference comparison)
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool SequenceEqual(Cube3D<T> other, IEqualityComparer<T>? comparer) =>
        (   /* bitAND operation */
            Unsafe.As<uint, long>(ref Unsafe.AsRef(in width)) == Unsafe.As<uint, long>(ref Unsafe.AsRef(in other.width)) &
            depth == other.depth
        ) &&
        AsVector().SequenceEqual((IEnumerable<T>)other.AsVector(), comparer);
#pragma warning restore IDE0047
    #endregion

    #region slice
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Cube3D<T> Slice(Shape startIndex) => 
        Slice(startIndex, (width - startIndex.dim1, height - startIndex.dim2, depth - startIndex.dim3));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Cube3D<T> Slice(Shape startIndex, Shape length)
    {
        Debug.Assert((startIndex.rank | length.rank) == 3);
        return new Cube3D<T>(
            ptr + height * depth * startIndex.dim1 + depth * startIndex.dim2 + startIndex.dim3,
            length.dim1,
            length.dim2,
            length.dim3);
    }
    #endregion

    #region override
    public override int GetHashCode() => ((ulong)ptr).GetHashCode();
    public override bool Equals(object? obj) => obj is Cube3D<T> other && Equals(other);
    #endregion

    #region opreator
    /// <summary>
    /// Fast and simple pointer address equality comparison.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(Cube3D<T> left, Cube3D<T> right) => left.ptr == right.ptr;
    /// <summary>
    /// Fast and simple pointer address in-equality comparison.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(Cube3D<T> left, Cube3D<T> right) => left.ptr != right.ptr;
    #endregion

    #region static method
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Cube3D<T> Alloc(uint width, uint height, uint depth) =>
        new Cube3D<T>(NativeMemory.Alloc(width * (nuint)height * depth, (nuint)sizeof(T)), width, height, depth);
    #endregion
}
