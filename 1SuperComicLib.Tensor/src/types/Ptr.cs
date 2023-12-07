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

[StructLayout(LayoutKind.Sequential)]
public readonly unsafe struct Ptr<T> : IComparable<Ptr<T>>, IEquatable<Ptr<T>>
    where T : unmanaged
{
    public readonly T* _Addr;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Ptr(T* pAddr) => _Addr = pAddr;

    public ref T Value
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ref *_Addr;
    }

    #region methods
    public bool Equals(Ptr<T> other) => other == this;
    public int CompareTo(Ptr<T> other) => (int)CMath.Normal((long)((ulong)_Addr - (ulong)other._Addr));

    public override int GetHashCode() => ((ulong)_Addr).GetHashCode();
    public override bool Equals([NotNullWhen(true)] object? obj) => obj is Ptr<T> other && Equals(other);
    public override string ToString() => ((ulong)_Addr).ToString("X16");
    #endregion

    #region equal, compare operators
    #region Ptr<>, Ptr<>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(Ptr<T> left, Ptr<T> right) => left._Addr == right._Addr;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(Ptr<T> left, Ptr<T> right) => left._Addr != right._Addr;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator <(Ptr<T> left, Ptr<T> right) => left._Addr < right._Addr;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator >(Ptr<T> left, Ptr<T> right) => left._Addr > right._Addr;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator <=(Ptr<T> left, Ptr<T> right) => left._Addr <= right._Addr;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator >=(Ptr<T> left, Ptr<T> right) => left._Addr >= right._Addr;
    #endregion

    #region Ptr<>, T*
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(Ptr<T> left, T* right) => left._Addr == right;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(Ptr<T> left, T* right) => left._Addr != right;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator <(Ptr<T> left, T* right) => left._Addr < right;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator >(Ptr<T> left, T* right) => left._Addr > right;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator <=(Ptr<T> left, T* right) => left._Addr <= right;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator >=(Ptr<T> left, T* right) => left._Addr >= right;
    #endregion

    #region T*, Ptr<>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(T* left, Ptr<T> right) => left == right._Addr;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(T* left, Ptr<T> right) => left != right._Addr;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator <(T* left, Ptr<T> right) => left < right._Addr;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator >(T* left, Ptr<T> right) => left > right._Addr;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator <=(T* left, Ptr<T> right) => left <= right._Addr;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator >=(T* left, Ptr<T> right) => left >= right._Addr;
    #endregion
    #endregion

    #region cast
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Ptr<T>(T* ptr) => new Ptr<T>(ptr);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator T*(Ptr<T> ptr) => ptr._Addr;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator T(Ptr<T> ptr) => *ptr._Addr;
    #endregion
}
