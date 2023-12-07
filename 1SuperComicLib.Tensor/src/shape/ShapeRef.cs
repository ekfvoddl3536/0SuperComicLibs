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

namespace SuperComicLib.Tensor;

/// <summary>
/// An object that can represent more dimensions than a <see cref="Shape"/>.<para/>
/// Dimensional information cannot be stored directly; it references other memory.
/// </summary>
[CollectionBuilder(typeof(LibCollectionBuilder), nameof(LibCollectionBuilder.CreateShapeRef))]
public readonly unsafe ref struct ShapeRef
{
    public readonly ReadOnlySpan<uint> Dims;

    #region constructors
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public ShapeRef(Shape shape) : this(shape.AsSpan())
    {
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public ShapeRef(ReadOnlySpan<int> dims) : this(MemoryMarshal.Cast<int, uint>(dims))
    {
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public ShapeRef(params uint[] dims) : this(dims.AsSpan())
    {
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public ShapeRef(ReadOnlySpan<uint> dims) => Dims = dims;
    #endregion

    #region property
    public int Rank
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get => Dims.Length;
    }
    #endregion

    #region methods
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Shape ToShape(int startIndex)
    {
        Shape m = default;

        ref var dst = ref Unsafe.AsRef(in m.dim1);

        Unsafe.AsRef(in m.rank) = Math.Min(7u, (uint)Dims.Length);

        ref var src = ref MemoryMarshal.GetReference(Dims);

        for (uint i = (uint)startIndex; i != (uint)Dims.Length; ++i)
            Unsafe.Add(ref dst, (nint)i) = Unsafe.Add(ref src, (nint)i);

        return m;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint[] ToArray() => Dims.ToArray();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<uint>.Enumerator GetEnumerator() => Dims.GetEnumerator();
    #endregion

    #region override
    public override int GetHashCode() => 0;
    public override bool Equals(object? obj) => false;
    public override string ToString() => $"shape: [{Shape.ToString(Dims)}]";
    #endregion

    #region cast
    public static implicit operator ShapeRef(Shape s) => new ShapeRef(s);
    public static implicit operator ShapeRef(ReadOnlySpan<uint> s) => new ShapeRef(s);
    public static implicit operator ShapeRef(uint[] s) => new ShapeRef(s.AsSpan());
    #endregion

    #region operator
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static bool operator ==(ShapeRef left, ShapeRef right) => left.Dims.SequenceEqual(right.Dims);
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static bool operator !=(ShapeRef left, ShapeRef right) => !(left == right);
    #endregion
}
