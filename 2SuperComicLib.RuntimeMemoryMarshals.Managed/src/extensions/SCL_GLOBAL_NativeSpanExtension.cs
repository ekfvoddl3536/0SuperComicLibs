// MIT License
//
// Copyright (c) 2019-2023. SuperComic (ekfvoddl3535@naver.com)
// Copyright (c) .NET Foundation and Contributors
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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security;
using SuperComicLib.CodeContracts;

namespace SuperComicLib
{
    [SuppressUnmanagedCodeSecurity]
    public static unsafe class SCL_GLOBAL_NativeSpanExtension
    {
        [Obsolete("This API will be removed in the next major update. Use 'NativeSpan<T>.Cast<TTo>()' method instead. If 'obj.Cast<int, byte>()' is currently in use, please switch to 'obj.Cast<byte>()'.", false)]
        public static NativeSpan<TTo> Cast<TFrom, TTo>(this in NativeSpan<TFrom> @this)
            where TFrom : unmanaged
            where TTo : unmanaged
        {
            var len_u = (ulong)@this.Length * (uint)sizeof(TFrom) / (uint)sizeof(TTo);
            return new NativeSpan<TTo>((TTo*)@this.Source, (long)len_u);
        }

        [Obsolete("This API will be removed in the next major update. Use 'NativeConstSpan<T>.Cast<TTo>()' method instead. If 'obj.Cast<int, byte>()' is currently in use, please switch to 'obj.Cast<byte>()'.", false)]
        public static NativeConstSpan<TTo> Cast<TFrom, TTo>(this in NativeConstSpan<TFrom> @this)
            where TFrom : unmanaged
            where TTo : unmanaged
        {
            var len_u = (ulong)@this.Length * (uint)sizeof(TFrom) / (uint)sizeof(TTo);
            return new NativeConstSpan<TTo>((TTo*)@this._source, (long)len_u);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining), X64LossOfLength]
        public static void CopyTo<T>(this in NativeSpan<T> source, in Memory<T> destination) where T : unmanaged
        {
            if (source.Source == null)
                throw new ArgumentNullException(nameof(source));

            if (!destination.IsValid ||
                (uint)source.Length > (uint)destination.Length)
                throw new ArgumentOutOfRangeException(nameof(destination));

            CopyTo_unsafe(source, destination);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining), X64LossOfLength, AssumeInputsValid]
        public static void CopyTo_unsafe<T>([DisallowNull, ValidRange] this in NativeSpan<T> source, [DisallowNull, ValidRange] in Memory<T> destination) where T : unmanaged
        {
            ulong sz = (uint)source.Length * (uint)sizeof(T);
            fixed (T* pdst = &destination[0])
                Buffer.MemoryCopy(source.Source, pdst, sz, sz);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining), X64LossOfLength]
        public static void CopyTo<T>(this in NativeConstSpan<T> source, in Memory<T> destination) where T : unmanaged
        {
            if (source._source == null)
                throw new ArgumentNullException(nameof(source));

            if (!destination.IsValid ||
                (uint)source.Length > (uint)destination.Length)
                throw new ArgumentOutOfRangeException(nameof(destination));

            CopyTo_unsafe(source, destination);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining), X64LossOfLength, AssumeInputsValid]
        public static void CopyTo_unsafe<T>([DisallowNull, ValidRange] this in NativeConstSpan<T> source, [DisallowNull, ValidRange] in Memory<T> destination) where T : unmanaged
        {
            ulong sz = (uint)source.Length * (uint)sizeof(T);
            fixed (T* pdst = &destination[0])
                Buffer.MemoryCopy(source._source, pdst, sz, sz);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NativeConstSpan<T> AsReadOnly<T>(this in NativeSpan<T> source) where T : unmanaged => new NativeConstSpan<T>(source.Source, source.Length);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SequenceEqual<T>(this NativeSpan<T> left, NativeConstSpan<T> right) where T : unmanaged, IEquatable<T> =>
            SequenceEqual(left.AsReadOnly(), right);

        public static bool SequenceEqual<T>(this NativeConstSpan<T> left, NativeConstSpan<T> right) where T : unmanaged, IEquatable<T>
        {
            if (left.Length != right.Length)
                return false;

            if (left._source == right._source)
                return true;

            T* pleft = left._source;
            T* pright = right._source;

            long length = left.Length;
            for (long i = 0; i < length; ++i)
                if (!pleft[i].Equals(pright[i]))
                    return false;

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SequenceEqual<T>(this NativeSpan<T> left, NativeConstSpan<T> right, IEqualityComparer<T> comparer) where T : unmanaged =>
            SequenceEqual(left.AsReadOnly(), right, comparer);

        public static bool SequenceEqual<T>(this NativeConstSpan<T> left, NativeConstSpan<T> right, IEqualityComparer<T> comparer) where T : unmanaged
        {
            if (left.Length != right.Length)
                return false;

            if (left._source == right._source)
                return true;

            if (comparer == null)
                comparer = EqualityComparer<T>.Default;

            T* pleft = left._source;
            T* pright = right._source;

            long length = left.Length;
            for (long i = 0; i < length; ++i)
                if (!comparer.Equals(pleft[i], pright[i]))
                    return false;

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SequenceCompareTo<T>(this NativeSpan<T> left, NativeConstSpan<T> right) where T : unmanaged, IComparable<T> =>
            SequenceCompareTo(left.AsReadOnly(), right);

        public static int SequenceCompareTo<T>(NativeConstSpan<T> left, NativeConstSpan<T> right) where T : unmanaged, IComparable<T>
        {
            if (CMath.CXOR((long)left._source, (long)right._source, left.Length, right.Length) == 0)
                return 0;

            T* pleft = left._source;
            T* pright = right._source;

            long length = left.Length;
            for (long i = 0; i < length; ++i)
            {
                int cmp = pright[i].CompareTo(pright[i]);
                if (cmp != 0)
                    return cmp;
            }

            return 0;
        }

        public static void Reverse<T>(this NativeSpan<T> span) where T : unmanaged
        {
            if (span.Length <= 1)
                return;

            T* fp = span.Source;
            T* lp = fp + span.Length - 1;

            long half = span.Length >> 1;
            for (long i = 0; i < half; ++i)
            {
                T tmp = lp[-i];
                lp[-i] = fp[i];
                fp[i] = tmp;
            }
        }
    }
}
