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

[CollectionBuilder(typeof(LibCollectionBuilder), nameof(LibCollectionBuilder.CreateReadOnlyTensor1D))]
[DebuggerDisplay("ro-ten({shape})")]
[StructLayout(LayoutKind.Sequential, Pack = 0x10)]
public readonly unsafe struct ReadOnlyTensorND<T> : IEquatable<ReadOnlyTensorND<T>>, IEnumerable<ItemIndexTuple<T>>, IEnumerable<T>
    where T : unmanaged
{
    internal readonly TensorND<T> v;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlyTensorND(TensorND<T> v) => this.v = v;

    #region property
    public ShapeRef shape => v.shape;

    public int rank => v.rank;

    /// <summary>
    /// Gets a copy of <see langword="this"/> <see cref="ReadOnlyTensorND{T}"/>.
    /// </summary>
    /// <returns>[<b><see langword="get"/></b>] A copy of the <see langword="this"/> <see cref="ReadOnlyTensorND{T}"/>.</returns>
    public ReadOnlyTensorND<T> me => v.me;
    #endregion

    #region indexer
    public ReadOnlyTensorND<T> this[uint __ArgIndex__] => v[__ArgIndex__];

    public ReadOnlyTensorND<T> this[Shape __ArgIndexes__] => v[__ArgIndexes__];

    public ReadOnlyTensorND<T> this[ShapeRef __ArgIndexes__] => v[__ArgIndexes__];

    public ReadOnlyTensorND<T> this[params uint[] __ArgIndexes__] => v[__ArgIndexes__];

    public ReadOnlyTensorND<T> this[ReadOnlySpan<uint> __ArgIndexes__] => v[__ArgIndexes__];

    public ReadOnlyTensorND<T> this[params Slice[] __ArgSlices__] => v[__ArgSlices__];

    public ReadOnlyTensorND<T> this[ReadOnlySpan<Slice> __ArgSlices__] => v[__ArgSlices__];
    #endregion

    #region methods
    #region reshape
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlyTensorND<T> reshape(Shape shape) => reshape(shape.AsSpan());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlyTensorND<T> reshape(ShapeRef shape) => reshape(shape.Dims);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlyTensorND<T> reshape(params uint[] shape) => reshape(shape.AsReadOnlySpan());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlyTensorND<T> reshape(ReadOnlySpan<uint> shape) => v.reshape(shape);
    #endregion

    #region slice
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlyTensorND<T> slice(params Slice[] slices) => slice(slices.AsReadOnlySpan());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlyTensorND<T> slice(ReadOnlySpan<Slice> slices) => v.slice(slices);
    #endregion

    #region conversion
    public ref readonly T toScalar() => ref v.toScalar();

    public ReadOnlyVector1D<T> toVector() => v.toVector();

    public ReadOnlyMatrix2D<T> toMatrix() => v.toMatrix();

    public ReadOnlyCube3D<T> toCube() => v.toCube();

    public ReadOnlyVector1D<T> toVector(out object? memory) => v.toVector(out memory);

    public ReadOnlyMatrix2D<T> toMatrix(out object? memory) => v.toMatrix(out memory);

    public ReadOnlyCube3D<T> toCube(out object? memory) => v.toCube(out memory);
    #endregion

    #region asvector, asspan
    /// <summary>
    /// Returns a tensor as a raw vector.<para/>
    /// This operation includes data in all dimensions represented by the shape of this tensor.
    /// </summary>
    public ReadOnlyVector1D<T> AsVector() => v.AsVector();

    public ReadOnlyVector1D<T> AsVector(out object? memory) => v.AsVector(out memory);

    public ReadOnlySpan<T> AsSpan() => v.AsSpan();
    #endregion

    #region trimExcess
    /// <summary>
    /// Compresses and rearranges the tensor to ensure that elements across all dimensions are contiguous in memory.
    /// </summary>
    /// <param name="alwaysCreateNewTensor">Determines whether a new tensor should always be created.</param>
    public ReadOnlyTensorND<T> trimExcess(bool alwaysCreateNewTensor = true) => v.trimExcess(alwaysCreateNewTensor);

    /// <summary>
    /// Compresses and rearranges the tensor to ensure that elements across all dimensions are contiguous in memory.<para/>
    /// This process does not allocate new memory, but utilizes the provided memory space.
    /// </summary>
    /// <param name="nativeMemory">
    /// Represents the provided memory space, which must be a fixed memory address not subject to relocation by the Garbage Collector (GC).
    /// </param>
    public ReadOnlyTensorND<T> trimExcess(Vector1D<T> nativeMemory) => v.trimExcess(nativeMemory);
    #endregion

    #region utils
    #region copy
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool tryCopyTo(TensorND<T> destination) => v.tryCopyTo(destination);
    #endregion

    #region equal
    /// <summary>
    /// Deep equality comparison based on raw values. 
    /// (Performs a comparison of shape and all elements for equality, without performing reference comparison)
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool SequenceEqual(ReadOnlyTensorND<T> other) => SequenceEqual(other, null);

    /// <summary>
    /// Deep equality comparison based on raw values. 
    /// (Performs a comparison of shape and all elements for equality, without performing reference comparison)
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool SequenceEqual(ReadOnlyTensorND<T> other, IEqualityComparer<T>? comparer) =>
        v.SequenceEqual(other.v, comparer);
    #endregion

    #endregion

    #region forEach
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void forEach(Action<T> action) => v.forEach(action);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void forEach(delegate*<T, void> ptr_action) => v.forEach(ptr_action);
    #endregion

    #region forEach +index
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void forEach(WithIndexAction<T> action) => v.forEach(action);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void forEach(delegate*<in T, ShapeRef, void> ptr_action) => v.forEach(ptr_action);
    #endregion

    #region getEnumerator
    /// <summary>
    /// Enumerates elements in contiguous order in memory.
    /// </summary>
    /// <remarks>
    /// This enumerator is <see langword="not"/> the <see langword="default"/> enumerator.<br/>
    /// When up-cast to an <see cref="IEnumerable"/>, this enumerator is <see langword="not"/> used by <see langword="default"/>.
    /// </remarks>
    public IEnumerator<T> GetEnumerator() => v.GetEnumerator();

    /// <summary>
    /// Enumerates element and dimension indices in contiguous order in memory.
    /// </summary>
    /// <remarks>
    /// This enumerator is the <see langword="default"/> enumerator.<br/>
    /// When up-cast to an <see cref="IEnumerable"/>, this enumerator is used by <see langword="default"/>.
    /// </remarks>
    public IEnumerator<ItemIndexTuple<T>> GetWithIndexEnumerator() => v.GetWithIndexEnumerator();
    #endregion
    #endregion

    #region interface implements
    /// <summary>
    /// Reference and shape comparison. (Perform simple shape-matching object equality check)
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(ReadOnlyTensorND<T> other) => v.Equals(other.v);

    IEnumerator<ItemIndexTuple<T>> IEnumerable<ItemIndexTuple<T>>.GetEnumerator() => GetWithIndexEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetWithIndexEnumerator();
    #endregion

    #region override
    public override int GetHashCode() => v.GetHashCode();
    public override bool Equals(object? obj) => obj is ReadOnlyTensorND<T> other && Equals(other);
    #endregion

    #region cast
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator ReadOnlyTensorND<T>(TensorND<T> v) => new ReadOnlyTensorND<T>(v);

    public static implicit operator ReadOnlyTensorND<T>(T v) => (TensorND<T>)v;
    public static implicit operator ReadOnlyTensorND<T>(T* v) => (TensorND<T>)v;
    public static implicit operator ReadOnlyTensorND<T>(ReadOnlyVector1D<T> v) => (TensorND<T>)v.v;
    public static implicit operator ReadOnlyTensorND<T>(ReadOnlyMatrix2D<T> v) => (TensorND<T>)v.v;
    public static implicit operator ReadOnlyTensorND<T>(ReadOnlyCube3D<T> v) => (TensorND<T>)v.v;

    public static implicit operator ReadOnlyTensorND<T>(ReadOnlySpan<T> vs) => (TensorND<T>)vs;
    #endregion

    #region opreator
    /// <summary>
    /// Reference comparison. (Performs a fast and simple equality check)
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(ReadOnlyTensorND<T> left, ReadOnlyTensorND<T> right) => left.v == right.v;
    /// <summary>
    /// Reference comparison. (Performs a fast and simple inequality check)
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(ReadOnlyTensorND<T> left, ReadOnlyTensorND<T> right) => left.v != right.v;
    #endregion
}
