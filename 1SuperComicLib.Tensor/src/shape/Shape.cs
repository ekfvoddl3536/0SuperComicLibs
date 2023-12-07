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

using System.Diagnostics.CodeAnalysis;

namespace SuperComicLib.Tensor;

/// <summary>
/// A lightweight shape object that can represent up to 7-dimensions.
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 0x10)]
public readonly unsafe ref struct Shape
{
    public readonly uint dim1, dim2, dim3, dim4;
    public readonly uint dim5, dim6, dim7, rank;

    #region constructors
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public Shape(uint dim1) :
        this(dim1, 0, 0, 0, 0, 0, 0, 1)
    {
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public Shape(uint dim1, uint dim2) :
        this(dim1, dim2, 0, 0, 0, 0, 0, 2)
    {
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public Shape(uint dim1, uint dim2, uint dim3) :
        this(dim1, dim2, dim3, 0, 0, 0, 0, 3)
    {
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public Shape(uint dim1, uint dim2, uint dim3, uint dim4) :
        this(dim1, dim2, dim3, dim4, 0, 0, 0, 4)
    {
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public Shape(uint dim1, uint dim2, uint dim3, uint dim4, uint dim5) :
        this(dim1, dim2, dim3, dim4, dim5, 0, 0, 5)
    {
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public Shape(uint dim1, uint dim2, uint dim3, uint dim4, uint dim5, uint dim6) :
        this(dim1, dim2, dim3, dim4, dim5, dim6, 0, 6)
    {
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public Shape(uint dim1, uint dim2, uint dim3, uint dim4, uint dim5, uint dim6, uint dim7) :
        this(dim1, dim2, dim3, dim4, dim5, dim6, dim7, 7)
    {
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private Shape(uint dim1, uint dim2, uint dim3, uint dim4, uint dim5, uint dim6, uint dim7, uint rank)
    {
        this.dim1 = dim1;
        this.dim2 = dim2;
        this.dim3 = dim3;
        this.dim4 = dim4;
        this.dim5 = dim5;
        this.dim6 = dim6;
        this.dim7 = dim7;
        this.rank = rank;
    }
    #endregion

    #region property
    public ulong size => rank switch
    {
        0 => 0,
        1 => dim1,
        2 => dim2 * (ulong)dim1,
        3 => dim3 * (ulong)dim2 * dim1,
        4 => dim4 * (ulong)dim3 * dim2 * dim1,
        5 => dim5 * (ulong)dim4 * dim3 * dim2 * dim1,
        6 => dim6 * (ulong)dim5 * dim4 * dim3 * dim2 * dim1,
        _ => dim7 * (ulong)dim6 * dim5 * dim4 * dim3 * dim2 * dim1,
    };
    #endregion

    #region methods
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<uint> AsSpan() => new ReadOnlySpan<uint>(Unsafe.AsPointer(ref Unsafe.AsRef(in dim1)), (int)rank);
    #endregion

    #region override
    public override int GetHashCode() => 0;
    public override bool Equals([NotNullWhen(true)] object? obj) => false;
    public override string ToString() => $"shape: [{ToString(AsSpan())}]";
    #endregion

    #region static helper method
    [SkipLocalsInit]
    internal static string ToString(in ReadOnlySpan<uint> dims)
    {
        if (dims.Length == 0)
            return string.Empty;

        var pChar = stackalloc char[dims.Length << 4];
        var pLast = pChar;

        var written = 0L;
        dims[0].TryFormat(new Span<char>(pChar, 16), out Unsafe.As<long, int>(ref written));
        pLast += written;

        for (int i = 1; i < dims.Length; ++i)
        {
            // ", "
            *(int*)pLast = 0x0020_002C;
            dims[i].TryFormat(new Span<char>(pLast + 2, 16), out Unsafe.As<long, int>(ref written));
            pLast += written + 2;
        }

        return new string(new ReadOnlySpan<char>(pChar, (int)(pLast - pChar)));
    }
    #endregion

    #region cast
    public static implicit operator Shape(int dim1) => new Shape((uint)dim1);
    public static implicit operator Shape((int, int) dims) => new Shape((uint)dims.Item1, (uint)dims.Item2);
    public static implicit operator Shape((int, int, int) dims) => new Shape((uint)dims.Item1, (uint)dims.Item2, (uint)dims.Item3);
    public static implicit operator Shape((int, int, int, int) dims) => new Shape((uint)dims.Item1, (uint)dims.Item2, (uint)dims.Item3, (uint)dims.Item4);
    public static implicit operator Shape((int, int, int, int, int) dims) => new Shape((uint)dims.Item1, (uint)dims.Item2, (uint)dims.Item3, (uint)dims.Item4, (uint)dims.Item5);
    public static implicit operator Shape((int, int, int, int, int, int) dims) => new Shape((uint)dims.Item1, (uint)dims.Item2, (uint)dims.Item3, (uint)dims.Item4, (uint)dims.Item5, (uint)dims.Item6);
    public static implicit operator Shape((int, int, int, int, int, int, int) dims) => new Shape((uint)dims.Item1, (uint)dims.Item2, (uint)dims.Item3, (uint)dims.Item4, (uint)dims.Item5, (uint)dims.Item6, (uint)dims.Item7);

    public static implicit operator Shape(uint dim1) => new Shape(dim1);
    public static implicit operator Shape((uint, uint) dims) => new Shape(dims.Item1, dims.Item2);
    public static implicit operator Shape((uint, uint, uint) dims) => new Shape(dims.Item1, dims.Item2, dims.Item3);
    public static implicit operator Shape((uint, uint, uint, uint) dims) => new Shape(dims.Item1, dims.Item2, dims.Item3, dims.Item4);
    public static implicit operator Shape((uint, uint, uint, uint, uint) dims) => new Shape(dims.Item1, dims.Item2, dims.Item3, dims.Item4, dims.Item5);
    public static implicit operator Shape((uint, uint, uint, uint, uint, uint) dims) => new Shape(dims.Item1, dims.Item2, dims.Item3, dims.Item4, dims.Item5, dims.Item6);
    public static implicit operator Shape((uint, uint, uint, uint, uint, uint, uint) dims) => new Shape(dims.Item1, dims.Item2, dims.Item3, dims.Item4, dims.Item5, dims.Item6, dims.Item7);
    #endregion

    #region operator
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static bool operator ==(Shape left, Shape right) => left.AsSpan().SequenceEqual(right.AsSpan());
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static bool operator !=(Shape left, Shape right) => left.AsSpan().SequenceEqual(right.AsSpan());
    #endregion
}
