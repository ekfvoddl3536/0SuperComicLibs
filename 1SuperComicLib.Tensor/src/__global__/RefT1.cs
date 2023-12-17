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

namespace SuperComicLib;

[StructLayout(LayoutKind.Sequential)]
public ref struct RefT1<T>
{
    public ref T v1;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public RefT1(ref T v1) => this.v1 = ref v1;
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public RefT1(Span<T> span) : this(ref MemoryMarshal.GetReference(span)) { }
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public RefT1(ReadOnlySpan<T> span) : this(ref MemoryMarshal.GetReference(span)) { }
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public RefT1(T[] array) : this(ref MemoryMarshal.GetArrayDataReference(array)) { }
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public RefT1(T[] array, int startIndex) : this(ref Unsafe.Add(ref MemoryMarshal.GetArrayDataReference(array), startIndex)) { }

    public readonly ref T this[long index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get => ref Unsafe.Add(ref Unsafe.AsRef(in v1), (nint)index);
    }

    public readonly bool isNull
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get => Unsafe.IsNullRef(in v1);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public readonly RefT1<U> Cast<U>() where U : unmanaged => new RefT1<U>(ref Unsafe.As<T, U>(ref Unsafe.AsRef(in v1)));

    public readonly override int GetHashCode() => 0;
    public readonly override bool Equals(object? obj) => false;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator T(RefT1<T> v) => v.v1;
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator RefT1<T>(in T v) => new RefT1<T>(ref Unsafe.AsRef(in v));
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator RefT1<T>(T[] v) => new RefT1<T>(v);
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator RefT1<T>(Span<T> v) => new RefT1<T>(v);
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator RefT1<T>(ReadOnlySpan<T> v) => new RefT1<T>(v);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static bool operator ==(RefT1<T> left, RefT1<T> right) => Unsafe.AreSame(in left.v1, in right.v1);
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static bool operator !=(RefT1<T> left, RefT1<T> right) => !(left == right);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static bool operator <(RefT1<T> left, RefT1<T> right) => Unsafe.IsAddressLessThan(in left.v1, in right.v1);
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static bool operator >(RefT1<T> left, RefT1<T> right) => Unsafe.IsAddressGreaterThan(in left.v1, in right.v1);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static bool operator <=(RefT1<T> left, RefT1<T> right) => !(left > right);
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static bool operator >=(RefT1<T> left, RefT1<T> right) => !(left < right);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static RefT1<T> operator +(RefT1<T> left, int offset) => left + (long)offset;
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static RefT1<T> operator -(RefT1<T> left, int offset) => left - (long)offset;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static RefT1<T> operator +(RefT1<T> left, uint offset) => left + (long)offset;
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static RefT1<T> operator -(RefT1<T> left, uint offset) => left - (long)offset;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static RefT1<T> operator +(RefT1<T> left, long offset) => new RefT1<T>(ref Unsafe.Add(ref left.v1, (nint)offset));
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static RefT1<T> operator -(RefT1<T> left, long offset) => new RefT1<T>(ref Unsafe.Subtract(ref left.v1, (nint)offset));

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static RefT1<T> operator +(RefT1<T> left, ulong offset) => new RefT1<T>(ref Unsafe.Add(ref left.v1, (nuint)offset));
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static RefT1<T> operator -(RefT1<T> left, ulong offset) => new RefT1<T>(ref Unsafe.Subtract(ref left.v1, (nuint)offset));

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static RefT1<T> operator ++(RefT1<T> left) => left + 1L;
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static RefT1<T> operator --(RefT1<T> left) => left - 1L;
}