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

public static unsafe class TensorsExtension
{
    #region as-readonly
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlyVector1D<T> AsReadOnly<T>(this Vector1D<T> v) where T : unmanaged => v;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlyMatrix2D<T> AsReadOnly<T>(this Matrix2D<T> v) where T : unmanaged => v;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlyCube3D<T> AsReadOnly<T>(this Cube3D<T> v) where T : unmanaged => v;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlyTensorND<T> AsReadOnly<T>(this TensorND<T> v) where T : unmanaged => v;
    #endregion

    #region cast
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector1D<TTo> Cast<TFrom, TTo>(this Vector1D<TFrom> v)
        where TFrom : unmanaged
        where TTo : unmanaged
    {
        var len = (ulong)v.length * (uint)sizeof(TFrom) / (uint)sizeof(TTo);

        return new Vector1D<TTo>(v.ptr, (long)len);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix2D<TTo> Cast<TFrom, TTo>(this Matrix2D<TFrom> v)
        where TFrom : unmanaged
        where TTo : unmanaged
    {
        var w = (ulong)v.width * (uint)sizeof(TFrom) / (uint)sizeof(TTo);
        var h = (ulong)v.height * (uint)sizeof(TFrom) / (uint)sizeof(TTo);

        return new Matrix2D<TTo>(v.ptr, (uint)w, (uint)h);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Cube3D<TTo> Cast<TFrom, TTo>(this Cube3D<TFrom> v)
        where TFrom : unmanaged
        where TTo : unmanaged
    {
        var w = (ulong)v.width * (uint)sizeof(TFrom) / (uint)sizeof(TTo);
        var h = (ulong)v.height * (uint)sizeof(TFrom) / (uint)sizeof(TTo);
        var d = (ulong)v.depth * (uint)sizeof(TFrom) / (uint)sizeof(TTo);

        return new Cube3D<TTo>(v.ptr, (uint)w, (uint)h, (uint)d);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TensorND<TTo> Cast<TFrom, TTo>(this TensorND<TFrom> v)
        where TFrom : unmanaged
        where TTo : unmanaged
    {
        var c = v.shape.Dims.ToArray();

        for (int i = 0; i < c.Length; ++i)
            c[i] = (uint)((ulong)c[i] * (uint)sizeof(TFrom) / (uint)sizeof(TTo));

        return new TensorND<TTo>((TTo*)v.ptr, new DimsHandle(c.AsReadOnlySpan()), v.memory);
    }
    #endregion

    #region cast (readonly group)
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlyVector1D<TTo> Cast<TFrom, TTo>(this ReadOnlyVector1D<TFrom> v)
        where TFrom : unmanaged
        where TTo : unmanaged =>
        v.v.Cast<TFrom, TTo>();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlyMatrix2D<TTo> Cast<TFrom, TTo>(this ReadOnlyMatrix2D<TFrom> v)
        where TFrom : unmanaged
        where TTo : unmanaged =>
        v.v.Cast<TFrom, TTo>();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlyCube3D<TTo> Cast<TFrom, TTo>(this ReadOnlyCube3D<TFrom> v)
        where TFrom : unmanaged
        where TTo : unmanaged =>
        v.v.Cast<TFrom, TTo>();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlyTensorND<TTo> Cast<TFrom, TTo>(this ReadOnlyTensorND<TFrom> v)
        where TFrom : unmanaged
        where TTo : unmanaged =>
        v.v.Cast<TFrom, TTo>();
    #endregion

    #region clear & fill (vector)
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Clear<T>(this Vector1D<T> v) where T : unmanaged
    {
        var t = v;

        for (; t.length > int.MaxValue; t = t.Slice(int.MaxValue))
            t.Slice(0, int.MaxValue).AsSpan().Clear();

        t.AsSpan().Clear();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Fill<T>(this Vector1D<T> v, T value) where T : unmanaged
    {
        var t = v;

        for (; t.length > int.MaxValue; t = t.Slice(int.MaxValue))
            t.Slice(0, int.MaxValue).AsSpan().Fill(value);

        t.AsSpan().Fill(value);
    }
    #endregion

    #region clear
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Clear<T>(this Matrix2D<T> v) where T : unmanaged => 
        v.AsVector().Clear();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Clear<T>(this Cube3D<T> v) where T : unmanaged => 
        v.AsVector().Clear();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Clear<T>(this TensorND<T> v) where T : unmanaged => 
        v.AsVector().Clear();
    #endregion

    #region fill
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Fill<T>(this Matrix2D<T> v, T value) where T : unmanaged => 
        v.AsVector().Fill(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Fill<T>(this Cube3D<T> v, T value) where T : unmanaged => 
        v.AsVector().Fill(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Fill<T>(this TensorND<T> v, T value) where T : unmanaged => 
        v.AsVector().Fill(value);
    #endregion
}