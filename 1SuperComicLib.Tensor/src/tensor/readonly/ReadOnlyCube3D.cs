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

[DebuggerDisplay("ro-cub({width}, {height}, {depth})")]
[StructLayout(LayoutKind.Sequential, Pack = 0x10)]
public readonly unsafe struct ReadOnlyCube3D<T> : IEquatable<ReadOnlyCube3D<T>>
    where T : unmanaged
{
    internal readonly Cube3D<T> v;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlyCube3D(Cube3D<T> v) => this.v = v;

    public ReadOnlyVector1D<T> AsVector() => v.AsVector();

    public ReadOnlyMatrix2D<T> this[long width_index] => v[width_index];

    public ReadOnlyVector1D<T> this[long width_index, long height_index] => v[width_index, height_index];

    public ref readonly T this[long width_index, long height_index, long depth_index] => ref v[width_index, height_index, depth_index];

    public ulong size => v.size;

    public uint width => v.width;

    public uint height => v.height;

    public uint depth => v.depth;

    public Shape shape => v.shape;

    /// <summary>
    /// Simple pointer address and shape equality comparison.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(ReadOnlyCube3D<T> other) => v.Equals(other.v);

    /// <summary>
    /// Deep equality comparison based on raw values. 
    /// (Performs a comparison of shape and all elements for equality, without performing reference comparison)
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool SequenceEqual(ReadOnlyCube3D<T> other) => SequenceEqual(other, null);

    /// <summary>
    /// Deep equality comparison based on raw values. 
    /// (Performs a comparison of shape and all elements for equality, without performing reference comparison)
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool SequenceEqual(ReadOnlyCube3D<T> other, IEqualityComparer<T>? comparer) =>
        v.SequenceEqual(other.v, comparer);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlyCube3D<T> Slice(Shape startIndex) => v.Slice(startIndex);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlyCube3D<T> Slice(Shape startIndex, Shape length) => v.Slice(startIndex, length);

    #region override
    public override int GetHashCode() => v.GetHashCode();
    public override bool Equals(object? obj) => obj is ReadOnlyCube3D<T> other && Equals(other);
    #endregion

    #region opreator
    /// <summary>
    /// Fast and simple pointer address equality comparison.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(ReadOnlyCube3D<T> left, ReadOnlyCube3D<T> right) => left.v == right.v;
    /// <summary>
    /// Fast and simple pointer address in-equality comparison.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(ReadOnlyCube3D<T> left, ReadOnlyCube3D<T> right) => left.v != right.v;
    #endregion

    #region cast
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator ReadOnlyCube3D<T>(Cube3D<T> v) => new ReadOnlyCube3D<T>(v);
    #endregion
}