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

[CollectionBuilder(typeof(LibCollectionBuilder), nameof(LibCollectionBuilder.CreateTensor1D))]
[DebuggerDisplay("ten({shape})")]
[StructLayout(LayoutKind.Sequential, Pack = 0x10)]
public readonly unsafe partial struct TensorND<T> : IEquatable<TensorND<T>>, IEnumerable<ItemIndexTuple<T>>, IEnumerable<T>
    where T : unmanaged
{
    /// <summary>
    /// This value is null by default.<br/>
    /// Used to store a reference when pinning a managed array of scalar values.
    /// <para/>
    /// This value must always be passed along with `<see cref="ptr"/>` when it is not <see langword="null"/>, 
    /// and this reference should be maintained until the usage of `<see cref="ptr"/>` is concluded.
    /// </summary>
    public readonly object? memory;
    public readonly T* ptr;

    private readonly DimsHandle dimsHnd;

    #region constructors
    #region scalar
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TensorND(T scalar)
    {
        var vs = GC.AllocateArray<T>(1, true);
        memory = vs;

        ptr = (T*)Unsafe.AsPointer(ref MemoryMarshal.GetArrayDataReference(vs));
        *ptr = scalar;

        dimsHnd = DimsHandle.Scalar;
    }
    #endregion

    #region pinned allocate
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TensorND(Shape shape) : this(shape.AsSpan())
    {
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TensorND(ShapeRef shape) : this(shape.Dims)
    {
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TensorND(params uint[] shape) : this(shape.AsReadOnlySpan())
    {
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TensorND(ReadOnlySpan<uint> shape)
    {
        dimsHnd = new DimsHandle(shape);

        // x > int.MaxValue
        if ((dimsHnd.totalSize >>> 31) != 0)
            throw new ArgumentOutOfRangeException(nameof(shape), "Length too large to allocate a managed array.");

        var vs = GC.AllocateUninitializedArray<T>((int)dimsHnd.totalSize, true);
        memory = vs;

        var ptr = (T*)Unsafe.AsPointer(ref MemoryMarshal.GetArrayDataReference(vs));
        this.ptr = ptr;
    }
    #endregion

    #region from array init
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TensorND(T[] array, Shape shape) : this(array.AsReadOnlySpan(), shape.AsSpan())
    {
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TensorND(T[] array, ShapeRef shape) : this(array.AsReadOnlySpan(), shape.Dims)
    {
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TensorND(T[] array, params uint[] shape) : this(array.AsReadOnlySpan(), shape.AsReadOnlySpan())
    {
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TensorND(T[] array, ReadOnlySpan<uint> shape) : this(array.AsReadOnlySpan(), shape)
    {
    }
    #endregion

    #region from span init
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TensorND(ReadOnlySpan<T> values, Shape shape) : this(values, shape.AsSpan())
    {
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TensorND(ReadOnlySpan<T> values, ShapeRef shape) : this(values, shape.Dims)
    {
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TensorND(ReadOnlySpan<T> values, params uint[] shape) : this(values, shape.AsReadOnlySpan())
    {
    }

    public TensorND(ReadOnlySpan<T> values, ReadOnlySpan<uint> shape) : this(shape)
    {
        var vs = (T[])memory!;

        var len = Math.Min(values.Length, (int)dimsHnd.totalSize);
        values.Slice(0, len).CopyTo(vs);

        // 남은 공간
        vs.AsSpan(len).Clear();
    }
    #endregion

    #region from enumerable init
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TensorND(IEnumerable<T> collection, Shape shape) : this(collection, shape.AsSpan())
    {
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TensorND(IEnumerable<T> collection, ShapeRef shape) : this(collection, shape.Dims)
    {
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TensorND(IEnumerable<T> collection, params uint[] shape) : this(collection, shape.AsReadOnlySpan())
    {
    }

    public TensorND(IEnumerable<T> collection, ReadOnlySpan<uint> shape) : this(shape)
    {
        var vs = (T[])memory!;
        var ptr = this.ptr;

        var count = 0L;

        foreach (var item in collection)
        {
            // FULL!!
            if ((int)count == vs.Length)
                return;

            ptr[count++] = item;
        }

        // 남은 공간
        vs.AsSpan((int)count).Clear();
    }
    #endregion

    #region pointer assign init
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TensorND(void* ptr, Shape shape) : this(ptr, shape.AsSpan())
    {
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TensorND(void* ptr, ShapeRef shape) : this(ptr, shape.Dims)
    {
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TensorND(void* ptr, params uint[] shape) : this(ptr, shape.AsReadOnlySpan())
    {
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TensorND(void* ptr, ReadOnlySpan<uint> shape)
    {
        this.ptr = (T*)ptr;
        dimsHnd = new DimsHandle(shape);
        memory = null;
    }
    #endregion

    #region quick assign
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal TensorND(T* ptr, DimsHandle dimsHnd, object? memory)
    {
        this.ptr = ptr;
        this.dimsHnd = dimsHnd;
        this.memory = memory;
    }
    #endregion
    #endregion

    #region property
    public ShapeRef shape => new ShapeRef(dimsHnd._shape(0));

    public int rank => dimsHnd._rank(0);

    /// <summary>
    /// Gets a copy of <see langword="this"/> <see cref="TensorND{T}"/>, or sets the <see langword="values"/> of its elements <see langword="directly"/>.
    /// </summary>
    /// <returns>[<b><see langword="get"/></b>] A copy of the <see langword="this"/> <see cref="TensorND{T}"/>.</returns>
    /// <value>[<b><see langword="set"/></b>] The <see langword="value"/> of the <see cref="TensorND{T}"/> to apply to elements in all dimensions of <see langword="this"/> <see cref="TensorND{T}"/>.</value>
    public TensorND<T> me
    {
        get => trimExcess(true);
        set => _setItem([], value);
    }
    #endregion

    #region indexer
    public TensorND<T> this[uint __ArgIndex__]
    {
        get => _getItem(new ReadOnlySpan<uint>(ref __ArgIndex__));
        set => _setItem(new ReadOnlySpan<uint>(ref __ArgIndex__), value);
    }

    public TensorND<T> this[Shape __ArgIndexes__]
    {
        get => _getItem(__ArgIndexes__.AsSpan());
        set => _setItem(__ArgIndexes__.AsSpan(), value);
    }

    public TensorND<T> this[ShapeRef __ArgIndexes__]
    {
        get => _getItem(__ArgIndexes__.Dims);
        set => _setItem(__ArgIndexes__.Dims, value);
    }

    public TensorND<T> this[params uint[] __ArgIndexes__]
    {
        get => _getItem(__ArgIndexes__.AsReadOnlySpan());
        set => _setItem(__ArgIndexes__.AsReadOnlySpan(), value);
    }

    public TensorND<T> this[ReadOnlySpan<uint> __ArgIndexes__]
    {
        get => _getItem(__ArgIndexes__);
        set => _setItem(__ArgIndexes__, value);
    }

    public TensorND<T> this[params Slice[] __ArgSlices__]
    {
        get => slice(__ArgSlices__.AsReadOnlySpan());
        set => slice(__ArgSlices__.AsReadOnlySpan())._setItem([], value);
    }

    public TensorND<T> this[ReadOnlySpan<Slice> __ArgSlices__]
    {
        get => slice(__ArgSlices__);
        set => slice(__ArgSlices__)._setItem([], value);
    }
    #endregion

    #region methods
    #region reshape
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TensorND<T> reshape(Shape shape) => reshape(shape.AsSpan());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TensorND<T> reshape(ShapeRef shape) => reshape(shape.Dims);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TensorND<T> reshape(params uint[] shape) => reshape(shape.AsReadOnlySpan());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TensorND<T> reshape(ReadOnlySpan<uint> shape)
    {
        var new_shape = DimsHandle.TryReshape(dimsHnd, shape) ?? throw new ArgumentException("invalid shape.");
        return new TensorND<T>(ptr, new_shape, memory);
    }
    #endregion

    #region slice
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TensorND<T> slice(params Slice[] slices) => slice(slices.AsReadOnlySpan());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TensorND<T> slice(ReadOnlySpan<Slice> slices)
    {
        if (slices.Length == 0)
            return this;

        ref readonly var hnd = ref dimsHnd;

        var ellipsis = slices.IndexOf(Slice.Ellipsis);
        if (ellipsis >= 0)
            slices = SliceHelper._internal_build(slices, ellipsis, hnd._shape(0));
        
        if (hnd._rank(0) != slices.Length)
            throw new ArgumentOutOfRangeException(nameof(slices));

        var clone = hnd._slice(slices, out var result);

        return new TensorND<T>(ptr + result.start, new DimsHandle(clone, clone.Length >> 1, 0, result.end), memory);
    }
    #endregion

    #region conversion
    public ref T toScalar()
    {
        if (rank != 0)
            throw new InvalidOperationException("non-scalar");

        return ref *ptr;
    }

    public Vector1D<T> toVector()
    {
        if (rank != 1)
            throw new InvalidOperationException("non-vector1D");

        ref readonly var dh = ref dimsHnd;

        _validation_sequential_tensor(in dh);

        return new Vector1D<T>(ptr, dh._dims());
    }

    public Matrix2D<T> toMatrix()
    {
        if (rank != 2)
            throw new InvalidOperationException("non-matrix2D");

        ref readonly var dh = ref dimsHnd;

        _validation_sequential_tensor(in dh);

        ref var sz = ref dh._dims();
        return new Matrix2D<T>(ptr, Unsafe.Add(ref sz, 1), sz);
    }

    public Cube3D<T> toCube()
    {
        if (rank != 3)
            throw new InvalidOperationException("non-cube3D");

        ref readonly var dh = ref dimsHnd;
        
        _validation_sequential_tensor(in dh);

        ref var sz = ref dh._dims();
        return new Cube3D<T>(ptr, Unsafe.Add(ref sz, 2), Unsafe.Add(ref sz, 1), sz);
    }

    public Vector1D<T> toVector(out object? reference)
    {
        reference = memory;
        return toVector();
    }

    public Matrix2D<T> toMatrix(out object? reference)
    {
        reference = memory;
        return toMatrix();
    }

    public Cube3D<T> toCube(out object? reference)
    {
        reference = memory;
        return toCube();
    }
    #endregion

    #region asvector, asspan
    /// <summary>
    /// Returns a tensor as a raw vector.<para/>
    /// This operation includes data in all dimensions represented by the shape of this tensor.
    /// </summary>
    public Vector1D<T> AsVector() => new Vector1D<T>(ptr, dimsHnd.totalSize);

    public Vector1D<T> AsVector(out object? reference)
    {
        reference = memory;
        return AsVector();
    }

    public Span<T> AsSpan()
    {
        if (memory != null)
        {
            var vs = (T[])memory;

            var offset = ptr - (T*)Unsafe.AsPointer(ref MemoryMarshal.GetArrayDataReference(vs));

            return new Span<T>(vs, (int)offset, (int)dimsHnd.totalSize);
        }
        else
            return new Span<T>(ptr, (int)dimsHnd.totalSize);
    }
    #endregion

    #region trimExcess
    /// <summary>
    /// Compresses and rearranges the tensor to ensure that elements across all dimensions are contiguous in memory.
    /// </summary>
    /// <param name="alwaysCreateNewTensor">Determines whether a new tensor should always be created.</param>
    public TensorND<T> trimExcess(bool alwaysCreateNewTensor = true)
    {
        ref readonly var dh = ref dimsHnd;
        if (!alwaysCreateNewTensor)
        {
            var stream = dh._stream_size();
            if (stream.start < 0 || stream.end == dh.totalSize)
                return this;
        }

        var dst = new TensorND<T>(dh._shape(0));

        _copyTo(dst.ptr, dst.dimsHnd);

        return dst;
    }

    /// <summary>
    /// Compresses and rearranges the tensor to ensure that elements across all dimensions are contiguous in memory.<para/>
    /// This process does not allocate new memory, but utilizes the provided memory space.
    /// </summary>
    /// <param name="nativeMemory">
    /// Represents the provided memory space, which must be a fixed memory address not subject to relocation by the Garbage Collector (GC).
    /// </param>
    public TensorND<T> trimExcess(Vector1D<T> nativeMemory)
    {
        var dh = new DimsHandle(dimsHnd._shape(0));
        if (nativeMemory.length < dh.totalSize)
            throw new ArgumentOutOfRangeException(nameof(nativeMemory));

        var dst = new TensorND<T>(nativeMemory.ptr, dh, null);

        _copyTo(dst.ptr, dh);

        return dst;
    }
    #endregion

    #region utils
    #region set, copy
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void set(TensorND<T> value) => _setItem([], value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool tryCopyTo(TensorND<T> destination)
    {
        if (shape != destination.shape)
            return false;

        _copyTo(destination.ptr, destination.dimsHnd);

        return true;
    }
    #endregion

    #region equal
    /// <summary>
    /// Deep equality comparison based on raw values. 
    /// (Performs a comparison of shape and all elements for equality, without performing reference comparison)
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool SequenceEqual(TensorND<T> other) => SequenceEqual(other, null);

    /// <summary>
    /// Deep equality comparison based on raw values. 
    /// (Performs a comparison of shape and all elements for equality, without performing reference comparison)
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool SequenceEqual(TensorND<T> other, IEqualityComparer<T>? comparer) =>
        shape == other.shape && ((IEnumerable<T>)this).SequenceEqual(other, comparer);
    #endregion

    #region update
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void update(Func<T, T> updater)
    {
        ArgumentNullException.ThrowIfNull(updater);

        var iter = GetEnumerator();
        while (iter.MoveNext())
            iter.Current = updater.Invoke(iter.Current);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void update(delegate*<T, T> ptr_updater)
    {
        var iter = GetEnumerator();
        while (iter.MoveNext())
            iter.Current = ptr_updater(iter.Current);
    }
    #endregion

    #region update +index
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void update(WithIndexFunc<T> updater)
    {
        ArgumentNullException.ThrowIfNull(updater);

        var iter = GetWithIndexEnumerator();
        while (iter.MoveNext())
        {
            var tuple = iter.Current;
            tuple.item = updater.Invoke(tuple.item, tuple.index);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void update(delegate*<in T, ShapeRef, T> ptr_updater)
    {
        var iter = GetWithIndexEnumerator();
        while (iter.MoveNext())
        {
            var tuple = iter.Current;
            tuple.item = ptr_updater(tuple.item, tuple.index);
        }
    }
    #endregion

    #region forEach
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void forEach(Action<T> action)
    {
        ArgumentNullException.ThrowIfNull(action);

        var iter = GetEnumerator();
        while (iter.MoveNext())
            action.Invoke(iter.Current);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void forEach(delegate*<T, void> ptr_action)
    {
        var iter = GetEnumerator();
        while (iter.MoveNext())
            ptr_action(iter.Current);
    }
    #endregion

    #region forEach +index
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void forEach(WithIndexAction<T> action)
    {
        ArgumentNullException.ThrowIfNull(action);

        var iter = GetWithIndexEnumerator();
        while (iter.MoveNext())
        {
            var tuple = iter.Current;
            action.Invoke(tuple.item, tuple.index);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void forEach(delegate*<in T, ShapeRef, void> ptr_action)
    {
        var iter = GetWithIndexEnumerator();
        while (iter.MoveNext())
        {
            var tuple = iter.Current;
            ptr_action(tuple.item, tuple.index);
        }
    }
    #endregion
    #endregion

    #region getEnumerator
    /// <summary>
    /// Enumerates elements in contiguous order in memory.
    /// </summary>
    /// <remarks>
    /// This enumerator is <see langword="not"/> the <see langword="default"/> enumerator.<br/>
    /// When up-cast to an <see cref="IEnumerable"/>, this enumerator is <see langword="not"/> used by <see langword="default"/>.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TensorND<T>.Enumerator GetEnumerator() => new Enumerator(this);

    /// <summary>
    /// Enumerates element and dimension indices in contiguous order in memory.
    /// </summary>
    /// <remarks>
    /// This enumerator is the <see langword="default"/> enumerator.<br/>
    /// When up-cast to an <see cref="IEnumerable"/>, this enumerator is used by <see langword="default"/>.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TensorND<T>.WithIndexEnumerator GetWithIndexEnumerator() => new WithIndexEnumerator(this);
    #endregion
    #endregion

    #region interface implements
    /// <summary>
    /// Reference and shape comparison. (Perform simple shape-matching object equality check)
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(TensorND<T> other) =>
        // Since the pointer addresses are identical, we can know that the same data exists without performing element comparison.
        this == other && shape == other.shape;

    IEnumerator<ItemIndexTuple<T>> IEnumerable<ItemIndexTuple<T>>.GetEnumerator() => GetWithIndexEnumerator();
    IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetWithIndexEnumerator();
    #endregion

    #region helper methods
    private static void _validation_sequential_tensor(ref readonly DimsHandle dh)
    {
        var stream = dh._stream_size();
        if (stream.end != dh.totalSize) // non-sequential
            throw new InvalidOperationException(SR.TENSOR_NONSEQUENTIAL);
    }

    private TensorND<T> _getItem(ReadOnlySpan<uint> dims)
    {
        if (dims.Length == 0)
            return this;

        var start_index = dimsHnd.index(dims);
        if (start_index < 0)
            throw new ArgumentOutOfRangeException(nameof(dims));

        var dhnd = new DimsHandle(dimsHnd, dims.Length);

        return new TensorND<T>(ptr + start_index, dhnd, memory);
    }

    private void _setItem(ReadOnlySpan<uint> dims, TensorND<T> value)
    {
        ref readonly var sd = ref dimsHnd;

        if (!sd._equals(dims.Length, value.dimsHnd))
            throw new ArgumentException(SR.MISMATCH_SHAPE, nameof(value));

        if (this == value)
            return;

        var startIndex = sd.index(dims);
        if (startIndex < 0)
            throw new ArgumentOutOfRangeException(nameof(dims));

        value._copyTo(ptr + startIndex, new DimsHandle(sd.lpDims, sd.dims_Length, sd.dims_Offset + dims.Length, sd.totalSize));
    }

    #region copy (destination seq)
    private void _copyTo(T* pdst, DimsHandle dstDimsHnd)
    {
        ref readonly var dh = ref dimsHnd;

        var src_stream = dh._stream_size();
        if (src_stream.start < 0) // scalar
        {
            *pdst = *ptr;
            return;
        }

        _copyTo_uni(pdst, dstDimsHnd, src_stream);
    }

    private void _copyTo_uni(T* pdst, DimsHandle dstDimsHnd, Range64 src_stream)
    {
        // requires:
        //       src.shape == dst.shape
        Debug.Assert(shape == dstDimsHnd._shape(0));

        var dst_stream = dstDimsHnd._stream_size();

        var dr = dstDimsHnd._rank_refs(dst_stream.start, out var ds);
        var sr = dimsHnd._rank_refs(src_stream.start, out var ss);

        long total = 
            dst_stream.end < src_stream.end
            ? TensorNDHelper._getTotal(dr)
            : TensorNDHelper._getTotal(sr);

        var psrc = ptr;

        var siter = psrc;
        var diter = pdst;

        var sEnd = src_stream.end;
        var dEnd = dst_stream.end;

        for (var seq = Math.Min(sEnd, dEnd); ;)
        {
            new ReadOnlySpan<T>(siter, (int)seq).CopyTo(new Span<T>(diter, (int)seq));
            
            if (--total == 0)
                break;

            if ((sEnd -= seq) == 0)
            {
                sEnd = src_stream.end;
                siter = psrc += ss;
            }
            else
                siter += seq;

            if ((dEnd -= seq) == 0)
            {
                dEnd = dst_stream.end;
                diter = pdst += ds;
            }
            else
                diter += seq;
        }
    }
    #endregion
    #endregion

    #region override
    public override int GetHashCode() => ((long)ptr).GetHashCode();
    public override bool Equals(object? obj) => obj is TensorND<T> other && Equals(other);
    #endregion

    #region cast
    public static implicit operator TensorND<T>(T v) => new TensorND<T>(v);
    public static implicit operator TensorND<T>(T* v) => new TensorND<T>(v, DimsHandle.Scalar, null);
    public static implicit operator TensorND<T>(Vector1D<T> v) => new TensorND<T>(v.ptr, cm.rospan([(uint)v.length]));
    public static implicit operator TensorND<T>(Matrix2D<T> v) => new TensorND<T>(v.ptr, cm.rospan([v.width, v.height]));
    public static implicit operator TensorND<T>(Cube3D<T> v) => new TensorND<T>(v.ptr, cm.rospan([v.width, v.height, v.depth]));

    public static implicit operator TensorND<T>(ReadOnlySpan<T> vs) => new TensorND<T>(vs, cm.rospan([(uint)vs.Length]));
    #endregion

    #region opreator
    /// <summary>
    /// Reference comparison. (Performs a fast and simple equality check)
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(TensorND<T> left, TensorND<T> right) => left.ptr == right.ptr;
    /// <summary>
    /// Reference comparison. (Performs a fast and simple inequality check)
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(TensorND<T> left, TensorND<T> right) => left.ptr != right.ptr;
    #endregion

    #region static methods
    public static TensorND<T> Alloc(Shape shape) => Alloc(shape.AsSpan());

    public static TensorND<T> Alloc(ShapeRef shape) => Alloc(shape.Dims);

    public static TensorND<T> Alloc(params uint[] shape) => Alloc(shape.AsReadOnlySpan());

    public static TensorND<T> Alloc(ReadOnlySpan<uint> shape)
    {
        var dims = new DimsHandle(shape);

        var ptr = (T*)NativeMemory.Alloc((nuint)dims.totalSize, (nuint)sizeof(T));

        return new TensorND<T>(ptr, dims, null);
    }

    public static TensorND<T> Constant(T value, Shape shape) => Constant(value, shape.AsSpan());

    public static TensorND<T> Constant(T value, ShapeRef shape) => Constant(value, shape.Dims);

    public static TensorND<T> Constant(T value, params uint[] shape) => Constant(value, shape.AsReadOnlySpan());

    public static TensorND<T> Constant(T value, ReadOnlySpan<uint> shape)
    {
        var tensor = new TensorND<T>(shape);

        /* remove extension dependencies */
        tensor.AsVector().AsSpan().Fill(value);

        return tensor;
    }
    #endregion
}
