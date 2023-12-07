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
[DebuggerDisplay("ro-vec({length})")]
[StructLayout(LayoutKind.Sequential, Pack = 0x10)]
public readonly unsafe struct ReadOnlyVector1D<T> : IEquatable<ReadOnlyVector1D<T>>, IEnumerable<T>
    where T : unmanaged
{
    internal readonly Vector1D<T> v;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlyVector1D(Vector1D<T> v) => this.v = v;

    public ref readonly T this[long index] => ref v.ptr[index];

    public long length => v.length;

    /// <summary>
    /// Simple pointer address and length equality comparison.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(ReadOnlyVector1D<T> other) => v.Equals(other.v);

    /// <summary>
    /// Deep equality comparison based on raw values. 
    /// (Performs a comparison of shape and all elements for equality, without performing reference comparison)
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool SequenceEqual(ReadOnlyVector1D<T> other) => SequenceEqual(other, null);

    /// <summary>
    /// Deep equality comparison based on raw values. 
    /// (Performs a comparison of shape and all elements for equality, without performing reference comparison)
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool SequenceEqual(ReadOnlyVector1D<T> other, IEqualityComparer<T>? comparer) =>
        v.SequenceEqual(other.v, comparer);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlyVector1D<T> Slice(long startIndex) => v.Slice(startIndex);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlyVector1D<T> Slice(long startIndex, long length) => v.Slice(startIndex, length);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<T> AsSpan() => new ReadOnlySpan<T>(v.ptr, (int)v.length);

    public IEnumerator<T> GetEnumerator() => v.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public override int GetHashCode() => v.GetHashCode();
    public override bool Equals(object? obj) => obj is ReadOnlyVector1D<T> other && Equals(other);

    /// <summary>
    /// Fast and simple pointer address equality comparison.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(in ReadOnlyVector1D<T> left, in ReadOnlyVector1D<T> right) => left.v == right.v;
    /// <summary>
    /// Fast and simple pointer address in-equality comparison.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(in ReadOnlyVector1D<T> left, in ReadOnlyVector1D<T> right) => left.v != right.v;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator ReadOnlyVector1D<T>(Vector1D<T> v) => new ReadOnlyVector1D<T>(v);
}
