// MIT License
//
// Copyright (c) 2019-2022 SuperComic (ekfvoddl3535@naver.com)
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
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SuperComicLib.IO
{
    public readonly unsafe ref struct NativeCharStream
    {
        public readonly char* _source;

        #region constructors
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeCharStream(int length) => 
            _source = (char*)Marshal.AllocHGlobal(sizeof(char) * length);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeCharStream(char* source) => 
            _source = source;
        #endregion

        #region indexer
        public ref char this[int index] => ref _source[index];
        #endregion

        #region methods
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int SkipWhiteSpaces(int array_index, int array_length)
        {
            char* src = _source + array_index;
            char* end = _source + array_length;

            while (src != end && char.IsWhiteSpace(*src))
                src++;

            return (int)(src - _source);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int IndexOf(int array_index, int array_length, char value)
        {
            char* src = _source + array_index;
            char* end = _source + array_length;

            while (src != end && *src == value)
                src++;

            return (int)(src - _source);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int IndexOf(int array_index, int array_length, Predicate<char> match)
        {
            char* src = _source + array_index;
            char* end = _source + array_length;

            while (src != end && !match.Invoke(*src))
                src++;

            return (int)(src - _source);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CopyToUnsafe(int array_index, char[] dst, int dst_index, int count)
        {
            fixed (char* pdst = dst)
                CopyToUnsafe(array_index, pdst + dst_index, count);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CopyToUnsafe(int array_index, char* dest, int count)
        {
            char* src = _source + array_index;
            char* end = src + count;

            ulong sz = (uint)count * sizeof(char);
            Buffer.MemoryCopy(src, dest, sz, sz);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string ToString(int array_index, int array_length, int count)
        {
            if ((uint)array_index >= (uint)array_length)
                return null;

            if (count <= 0)
                return string.Empty;

            return new string(_source + array_index, 0, count);
        }

        public void Free() => Marshal.FreeHGlobal((IntPtr)_source);
        #endregion

        #region convert method
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeSpan<char> AsSpan(int array_index, int array_length) =>
            new NativeSpan<char>(_source + array_index, array_length - array_index);
        #endregion

        #region override
        public override bool Equals(object obj) => false;
        public override int GetHashCode() => 0;
        #endregion

        #region equals operator
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(NativeCharStream left, NativeCharStream right) => left._source == right._source;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(NativeCharStream left, NativeCharStream right) => left._source != right._source;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(NativeCharStream left, void* right) => left._source == right;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(NativeCharStream left, void* right) => left._source != right;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(void* left, NativeCharStream right) => left == right._source;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(void* left, NativeCharStream right) => left != right._source;
        #endregion

        #region math operator
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static char* operator +(NativeCharStream left, int value) => left._source + value;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static char* operator -(NativeCharStream left, int value) => left._source - value;
        #endregion
    }
}
