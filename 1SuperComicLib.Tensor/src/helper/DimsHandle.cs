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

internal readonly unsafe struct DimsHandle
{
    // 0            ->  dims_Length     :   DIMS
    // dims_Length  ->  dims_Length * 2 :   ACC DIMS
    public readonly uint[] lpDims;

    public readonly long dims_Length;

    public readonly long dims_Offset;

    public readonly long totalSize;

    public DimsHandle(ReadOnlySpan<uint> shape)
    {
        if (shape.Length == 0)
        {
            lpDims = [];
            dims_Length = dims_Offset = totalSize = 0;
            return;
        }

        lpDims = new uint[shape.Length << 1];
        dims_Length = (uint)shape.Length;
        dims_Offset = 0;

        ref var ptr_shape = ref MemoryMarshal.GetArrayDataReference(lpDims);
        ref var ptr_sizes = ref Unsafe.Add(ref ptr_shape, lpDims.Length - 1);

        ref var shape_rev = ref Unsafe.Add(ref MemoryMarshal.GetReference(shape), shape.Length - 1);

        var acc = 1ul;
        for (int i = 0; i < shape.Length; ++i)
        {
            Unsafe.Add(ref ptr_shape, i) = shape[i];

            Unsafe.Add(ref ptr_sizes, -i) = (uint)acc;

            acc *= Unsafe.Add(ref shape_rev, -i);
        }

        totalSize = (long)acc;

        if (acc >> 32 != 0)
            throw new OverflowException(SR.TENSOR_OVERFLOW);
    }

    public DimsHandle(in DimsHandle source, long nLength)
    {
        Debug.Assert(source.dims_Length + nLength > 0);

        lpDims = source.lpDims;
        dims_Length = source.dims_Length;
        dims_Offset = nLength;
        totalSize = Unsafe.Add(ref MemoryMarshal.GetArrayDataReference(lpDims), (nint)(source.dims_Length + nLength - 1));
    }

    public DimsHandle(uint[] lpDims, long dims_Length, long dims_Offset, long totalSize)
    {
        this.lpDims = lpDims;
        this.dims_Length = dims_Length;
        this.dims_Offset = dims_Offset;
        this.totalSize = totalSize;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public RefT2<uint> _rank_refs(long offset, out long stride)
    {
        ref var first = ref MemoryMarshal.GetArrayDataReference(lpDims);

        ref var end = ref Unsafe.Add(ref first, (nint)(dims_Offset - 1));
        stride = 
            dims_Length == dims_Offset + offset
            ? totalSize
            : Unsafe.Add(ref end, (nint)((dims_Length << 1) - offset));

        return new RefT2<uint>(
            ref end,
            ref Unsafe.Add(ref first, (nint)(dims_Length - offset - 1))
            /* rend <--- rbegin */
        );
    }

    public Range64 _stream_size()
    {
        if ((lpDims.Length | _rank(0)) == 0)
            // 차원 없음(스칼라)
            return new Range64(-1, 0);

        ref var dims_end = ref Unsafe.Subtract(ref _dims(), 1);

        ref var dims = ref Unsafe.Add(ref MemoryMarshal.GetArrayDataReference(lpDims), (nint)dims_Length - 1);
        ref var sizes = ref Unsafe.Add(ref dims, (nint)dims_Length);

        var seq_rank = 0;
        var acc = 1u;
        for (; !Unsafe.AreSame(ref dims_end, ref dims); ++seq_rank)
        {
            if (acc != sizes)
                // 불연속 발견
                return new Range64(seq_rank, (int)acc);

            acc *= dims;

            dims = ref Unsafe.Subtract(ref dims, 1);
            sizes = ref Unsafe.Subtract(ref sizes, 1);
        }

        // 전체 연속
        return new Range64((uint)seq_rank, acc);
    }

    public uint[] _slice(ReadOnlySpan<Slice> ss, out Range64 result)
    {
        if ((lpDims.Length | _rank(0)) == 0)
        {
            result = default;
            return [];
        }

        var d = 0;
        for (int i = 0; i < ss.Length; ++i) 
            if (ss[i].index < 0) 
                ++d;

        var clone = new uint[d << 1];

        ref var src_shape = ref _dims();
        ref var src_sizes = ref Unsafe.Add(ref src_shape, (nint)dims_Length);

        ref var dst_shape = ref MemoryMarshal.GetArrayDataReference(clone);
        ref var dst_sizes = ref Unsafe.Add(ref dst_shape, d);

        var tsz = 1u;
        var index = 0u;
        for (int i = 0; i < ss.Length; ++i)
            switch (ss[i].index)
            {
                case Slice.V_ALL:
                    tsz *= Unsafe.Add(ref src_shape, i);

                    dst_shape = Unsafe.Add(ref src_shape, i);
                    dst_sizes = Unsafe.Add(ref src_sizes, i);

                    dst_shape = ref Unsafe.Add(ref dst_shape, 1);
                    dst_sizes = ref Unsafe.Add(ref dst_sizes, 1);
                    break;

                case Slice.V_NONE:
                    tsz = dst_shape = dst_sizes = 0;

                    dst_shape = ref Unsafe.Add(ref dst_shape, 1);
                    dst_sizes = ref Unsafe.Add(ref dst_sizes, 1);
                    break;

                default:
                    var idx = (uint)ss[i].index;
                    if (idx >= Unsafe.Add(ref src_shape, i))
                        throw new ArgumentOutOfRangeException(nameof(ss));

                    index += Unsafe.Add(ref src_sizes, i) * idx;
                    break;
            }

        result = new Range64(index, tsz);
        return clone;
    }

    /// <returns>returns < 0: index out of range</returns>
    public long index(ReadOnlySpan<uint> idxs)
    {
        if ((uint)_rank(0) < (uint)idxs.Length)
            return -1L;

        ref var pShape = ref _dims();
        ref var pSizes = ref Unsafe.Add(ref pShape, (nint)dims_Length);

        var index = 0L;
        for (int i = 0; i < idxs.Length; ++i)
        {
            if (idxs[i] >= Unsafe.Add(ref pShape, i))
                return -1L;

            index += Unsafe.Add(ref pSizes, i) * idxs[i];
        }

        return index;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int _rank(int offset)
    {
        Debug.Assert(offset >= 0);
        return (int)(dims_Length - dims_Offset) - offset;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public ReadOnlySpan<uint> _shape(int offset)
    {
        Debug.Assert(offset >= 0);
        return new ReadOnlySpan<uint>(lpDims, (int)dims_Offset + offset, _rank(offset));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public uint _size(int offset)
    {
        Debug.Assert(offset >= 0);
        return
            (uint)(--offset) < (uint)_rank(0)
            ? Unsafe.Add(ref _sizes(), offset)
            : (uint)totalSize;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool _equals(int indexes_Length, in DimsHandle other) =>
        (uint)indexes_Length < (uint)_rank(0) && _shape(indexes_Length).SequenceEqual(other._shape(0));

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public ref uint _dims() =>
        ref Unsafe.Add(ref MemoryMarshal.GetArrayDataReference(lpDims), (nint)dims_Offset);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public ref uint _sizes() =>
        ref Unsafe.Add(ref _dims(), (nint)dims_Length);

    /// <returns><see langword="null"/> = fail</returns>
    public static DimsHandle? TryReshape(in DimsHandle source, ReadOnlySpan<uint> shape)
    {
        // length mismatch
        if (source._rank(0) != shape.Length)
            return null;

        var reshape = new DimsHandle(shape);
        // totalSize mismatch
        if (source.totalSize != reshape.totalSize)
        {
            Unsafe.AsRef(in reshape.lpDims) = null!;
            return null;
        }

        return reshape;
    }

    public static DimsHandle Scalar => new DimsHandle([], 0, 0, 0);
}
