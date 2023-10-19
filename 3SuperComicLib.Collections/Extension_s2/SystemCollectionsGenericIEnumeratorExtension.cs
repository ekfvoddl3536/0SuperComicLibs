// MIT License
//
// Copyright (c) 2019-2023. SuperComic (ekfvoddl3535@naver.com)
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

using System.Collections.Generic;

namespace SuperComicLib.Collections
{
    public static unsafe class SystemCollectionsGenericIEnumeratorExtension
    {
        #region basic method
        /// <returns>The number of elements copied to the buffer.</returns>
        public static int EnumerateToEnd<T>(this IEnumerator<T> @this, IList<T> destination)
        {
            int begin = destination.Count;

            while (@this.MoveNext())
                destination.Add(@this.Current);

            return destination.Count - begin;
        }

        /// <returns>The number of elements copied to the buffer. It is safe to call until this value is 0</returns>
        public static int EnumerateToEnd<T>(this IEnumerator<T> @this, Memory<T> buffer)
        {
            var dst = buffer._source;
            var begin = buffer._start;
            var si = begin;

            while (si < buffer.Length && @this.MoveNext())
                dst[si++] = @this.Current;

            return si - begin;
        }

        /// <returns>The number of elements copied to the buffer. It is safe to call until this value is 0</returns>
        public static long EnumerateToEnd<T>(this IEnumerator<T> @this, NativeSpan<T> buffer) where T : unmanaged
        {
            var vsi = buffer.Source;
            var vdi = buffer.Source + buffer.Length;

            while (vsi != vdi && @this.MoveNext())
                *vsi++ = @this.Current;

            return vsi - buffer.Source;
        }

        /// <returns>The number of elements copied to the buffer. It is safe to call until this value is 0</returns>
        public static long EnumerateToEnd_Iterator<T>(this IEnumerator<T> @this, T* begin, T* end) where T : unmanaged
        {
            var vsi = begin;
            while (vsi < end && @this.MoveNext())
                *vsi++ = @this.Current;

            return vsi - begin;
        }

        /// <returns>The number of elements copied to the buffer. It is safe to call until this value is 0</returns>
        private static long EnumerateToEnd_ReverseIterator<T>(IEnumerator<T> @this, T* begin, T* end) where T : unmanaged
        {
            var vsi = begin;
            while (vsi > end && @this.MoveNext())
                *vsi-- = @this.Current;

            return vsi - begin;
        }
        #endregion

        #region T[] ([start],[count])
        /// <returns>The number of elements copied to the buffer. It is safe to call until this value is 0</returns>
        public static int EnumerateToEnd<T>(this IEnumerator<T> @this, T[] buffer, int start, int count) =>
            EnumerateToEnd(@this, new Memory<T>(buffer, start, count));
        /// <returns>The number of elements copied to the buffer. It is safe to call until this value is 0</returns>
        public static int EnumerateToEnd<T>(this IEnumerator<T> @this, T[] buffer, int start) =>
            EnumerateToEnd(@this, new Memory<T>(buffer, start, buffer.Length - start));
        /// <returns>The number of elements copied to the buffer. It is safe to call until this value is 0</returns>
        public static int EnumerateToEnd<T>(this IEnumerator<T> @this, T[] buffer) =>
            EnumerateToEnd(@this, new Memory<T>(buffer, 0, buffer.Length));
        #endregion

        #region T* ([count])
        /// <returns>The number of elements copied to the buffer. It is safe to call until this value is 0</returns>
        public static long EnumerateToEnd<T>(this IEnumerator<T> @this, T* buffer, int count) where T : unmanaged =>
            EnumerateToEnd(@this, new NativeSpan<T>(buffer, count));

        /// <returns>The number of elements copied to the buffer. It is safe to call until this value is 0</returns>
        public static long EnumerateToEndUnsafe<T>(this IEnumerator<T> @this, T* buffer) where T : unmanaged
        {
            var vsi = buffer;

            while (@this.MoveNext())
                *vsi++ = @this.Current;

            return vsi - buffer;
        }
        #endregion

        #region -> toArray & list
        public static Memory<T> EnumerateToEnd<T>(this IEnumerator<T> @this, int bufferSize)
        {
            T[] result = new T[bufferSize];
            int read = EnumerateToEnd(@this, new Memory<T>(result, 0, result.Length));
            return new Memory<T>(result, 0, read);
        }

        public static List<T> EnumerateToEnd<T>(this IEnumerator<T> @this)
        {
            List<T> result = new List<T>();
            EnumerateToEnd(@this, result);
            return result;
        }

        public static T[] ToArray<T>(this IEnumerator<T> @this) => EnumerateToEnd(@this).ToArray();
        #endregion

        #region iterator & reverse_iterator
        /// <returns>The number of elements copied to the buffer. It is safe to call until this value is 0</returns>
        public static long EnumerateToEnd<T>(this IEnumerator<T> @this, _iterator<T> begin, _iterator<T> end) where T : unmanaged =>
            EnumerateToEnd_Iterator(@this, begin._ptr, end._ptr);

        /// <returns>The number of elements copied to the buffer. It is safe to call until this value is 0</returns>
        public static long EnumerateToEnd<T>(this IEnumerator<T> @this, _iterator<T> begin, const_iterator<T> cend) where T : unmanaged =>
            EnumerateToEnd_Iterator(@this, begin._ptr, cend._ptr);

        /// <returns>The number of elements copied to the buffer. It is safe to call until this value is 0</returns>
        public static long EnumerateToEnd<T>(this IEnumerator<T> @this, reverse_iterator<T> begin, reverse_iterator<T> end) where T : unmanaged =>
            EnumerateToEnd_ReverseIterator(@this, begin._ptr, end._ptr);

        /// <returns>The number of elements copied to the buffer. It is safe to call until this value is 0</returns>
        public static long EnumerateToEnd<T>(this IEnumerator<T> @this, reverse_iterator<T> begin, const_reverse_iterator<T> cend) where T : unmanaged =>
            EnumerateToEnd_ReverseIterator(@this, begin._ptr, cend._ptr);
        #endregion
    }
}
