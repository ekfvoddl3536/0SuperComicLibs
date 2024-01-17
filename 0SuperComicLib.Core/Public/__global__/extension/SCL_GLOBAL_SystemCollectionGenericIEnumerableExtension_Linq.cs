// MIT License
//
// Copyright (c) 2019-2024. SuperComic (ekfvoddl3535@naver.com)
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

#pragma warning disable CS1591
using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using SuperComicLib.CodeContracts;

namespace SuperComicLib
{
    public static class SCL_GLOBAL_SystemCollectionGenericIEnumerableExtension_Linq
    {
        /// <summary>
        /// Use 'Slice(int, int).ToArray()' instead.
        /// </summary>
        [Obsolete("Use 'Slice(int, int).ToArray()' instead.", false)]
        public static T[] SubArray<T>(this T[] _arr, int startIdx, int count) => _arr.Slice(startIdx, count).ToArray();

        /// <summary>
        /// Use 'Slice(int, int).TryCopyTo(<see cref="Memory{T}"/>)' instead.
        /// </summary>
        [Obsolete("Use 'Slice(int, int).TryCopyTo(SuperComicLib.Memory<T>)' instead.", false)]
        public static bool TrySubArray<T>(this T[] _arr, int startIdx, int count, out T[] result)
        {
            var memory = _arr.Slice(startIdx, count);
            
            if (!memory.IsValid)
            {
                result = Array.Empty<T>();
                return false;
            }

            result = memory.ToArray();
            return true;
        }

        /// <summary>
        /// Use 'Replace(T, <see cref="Predicate{T}"/>)' instead.
        /// </summary>
        [Obsolete("Use 'Replace(T, Predicate<T>)' instead.", false)]
        public static T[] ReplaceAll<T>(this T[] _arr, T old, T replace) where T : IEquatable<T> =>
            _arr.Replace(replace, x => x.Equals(old)).ToArray();

        /// <summary>
        /// Use 'Replace(<see cref="IEnumerable{T}"/>, <see cref="Predicate{T}"/>)' instead.
        /// </summary>
        [Obsolete("Use 'Replace(IEnumerable<T>, Predicate<T>)' instead.", false)]
        public static T[] ReplaceAll<T>(this T[] _arr, T old, IEnumerable<T> replace) where T : IEquatable<T> =>
            _arr.Replace(replace, x => x.Equals(old)).ToArray();

        /// <summary>
        /// Use 'Replace(<see cref="IEnumerable{T}"/>, <see cref="Predicate{T}"/>)' instead.
        /// </summary>
        [Obsolete("Use 'Replace(IEnumerable<T>, Predicate<T>)' instead.", false)]
        public static T[] ReplaceAll<T>(this T[] _arr, T[] old, T[] replace) where T : IEquatable<T> =>
            _arr.Replace(replace, x => old.Contains(x)).ToArray();

        /// <summary>
        /// Use 'Replace(T, <see cref="Predicate{T}"/>)' instead.
        /// </summary>
        [Obsolete("Use 'Replace(IEnumerable<T>, Predicate<T>)' instead.", false)]
        public static T[] ReplaceAll<T>(this T[] _arr, T old, T replace, out int[] indices) where T : IEquatable<T>
        {
            var indexList = new List<int>(4);

            var result = _arr.Replace(replace, (item, index) =>
            {
                if (old.Equals(item))
                {
                    indexList.Add((int)index);
                    return true;
                }
                else
                    return false;
            }).ToArray();

            indices = indexList.ToArray();

            return result;
        }

        /// <summary>
        /// Use 'Replace(<see cref="IEnumerable{T}"/>, <see cref="Predicate{T}"/>)' instead.
        /// </summary>
        [Obsolete("Use 'Replace(IEnumerable<T>, Predicate<T>)' instead.", false)]
        public static T[] ReplaceAll<T>(this T[] _arr, T old, IEnumerable<T> replace, out int[] indices) where T : IEquatable<T>
        {
            var indexList = new List<int>(4);

            var result = _arr.Replace(replace, (item, index) =>
            {
                if (old.Equals(item))
                {
                    indexList.Add((int)index);
                    return true;
                }
                else
                    return false;
            }).ToArray();

            indices = indexList.ToArray();

            return result;
        }

        /// <summary>
        /// Use 'Replace(<see cref="IEnumerable{T}"/>, <see cref="Predicate{T}"/>)' instead.
        /// </summary>
        [Obsolete("Use 'Replace(IEnumerable<T>, Predicate<T>)' instead.", false)]
        public static T[] ReplaceAll<T>(this T[] _arr, T[] old, T[] replace, out int[] indices) where T : IEquatable<T>
        {
            var indexList = new List<int>(4);

            var result = _arr.Replace(replace, (item, index) =>
            {
                if (old.Contains(item))
                {
                    indexList.Add((int)index);
                    return true;
                }
                else
                    return false;
            }).ToArray();

            indices = indexList.ToArray();

            return result;
        }

        /// <summary>
        /// Use System.Linq and 'Zip' instead.
        /// </summary>
        [Obsolete("Use System.Linq and 'Zip' instead.", true)]
        public static void ForEachTuple<T1, T2>(this T1[] _arr1, T2[] _arr2, object action)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Construct tuples by assigning an index to the order in which they are enumerated.
        /// </summary>
        public static IEnumerable<(T, long)> IndexWith<T>(this IEnumerable<T> source) =>
            new IndexWithEnumerable<T>(source);

        /// <summary>
        /// Replaces elements that meet conditions with other values.
        /// </summary>
        public static IEnumerable<T> Replace<T>(this IEnumerable<T> source, [DisallowNull] IEnumerable<T> replace, [DisallowNull] Predicate<T> predicate) =>
            new ReplaceManyEnumerable<T>(source, replace, predicate);

        /// <summary>
        /// Replaces elements that meet conditions with other values.
        /// </summary>
        public static IEnumerable<T> Replace<T>(this IEnumerable<T> source, T replace, [DisallowNull] Predicate<T> predicate) =>
            new ReplaceEnumerable<T>(source, replace, predicate);

        /// <summary>
        /// Replaces elements that meet conditions with other values.
        /// </summary>
        public static IEnumerable<T> Replace<T>(this IEnumerable<T> source, [DisallowNull] IEnumerable<T> replace, [DisallowNull] Func<T, long, bool> predicateWithIndex) =>
            new ReplaceManyWithIndexEnumerable<T>(source, replace, predicateWithIndex);

        /// <summary>
        /// Replaces elements that meet conditions with other values.
        /// </summary>
        public static IEnumerable<T> Replace<T>(this IEnumerable<T> source, T replace, [DisallowNull] Func<T, long, bool> predicateWithIndex) =>
            new ReplaceWithIndexEnumerable<T>(source, replace, predicateWithIndex);

        /// <summary>
        /// Specifies an action that is performed only the first time the enumeration begins.
        /// </summary>
        public static IEnumerable<T> Prepare<T>(this IEnumerable<T> source, Action body) =>
            new PrepareActionEnumerable<T>(source, body);

        /// <summary>
        /// Peek at the enumerated value, without consuming it.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<T> Peek<T>(this IEnumerable<T> collection, [DisallowNull] Action<T> body) =>
            new PeekEnumerable<T>(collection, body);

        /// <summary>
        /// Peek at the enumerated value, without consuming it.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<T> Peek<T>(this IEnumerable<T> collection, [DisallowNull] Action<T, long> bodyWithIndex) =>
            new PeekWithIndexEnumerable<T>(collection, bodyWithIndex);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<TSource> Peek<TSource, TLocal>(this IEnumerable<TSource> collection, TLocal localinit, Func<TSource, TLocal, TLocal> body) =>
            new PeekWithLocalEnumerable<TSource, TLocal>(collection, new ValueInit<TLocal>(localinit).Get, body);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<TSource> Peek<TSource, TLocal>(this IEnumerable<TSource> collection, Func<TLocal> localinit, Func<TSource, TLocal, TLocal> body) =>
            new PeekWithLocalEnumerable<TSource, TLocal>(collection, localinit, body);

        /// <summary>
        /// Splits the elements of a sequence into batches of a specified size, returning each batch as an array.
        /// </summary>
        /// <remarks>
        /// This method divides an <see cref="IEnumerable{T}"/> into several smaller arrays, 
        /// each containing elements of the specified batch size. If the number of elements in the source sequence 
        /// is not a multiple of the batch size, the last batch will contain fewer elements than the specified batch size.
        /// </remarks>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> where <typeparamref name="T"/> is an array of the source sequence type, 
        /// each representing a batch of elements.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">Batch size is too small.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<T[]> Batch<T>(this IEnumerable<T> collection, int maxBatchSize)
        {
            if (maxBatchSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(maxBatchSize));

            return new BatchedEnumerable<T>(collection, maxBatchSize);
        }

        /// <summary>
        /// Enumerates the values and finally consumes the result.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ForEach<T>(this IEnumerable<T> collection, [DisallowNull] Action<T> body)
        {
            var enumerator = collection.GetEnumerator();
            
            while (enumerator.MoveNext())
                body.Invoke(enumerator.Current);
        }

        /// <summary>
        /// Enumerates the values and finally consumes the result.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ForEach<T>(this IEnumerable<T> collection, [DisallowNull] Action<T, long> bodyWithIndex)
        {
            var enumerator = collection.GetEnumerator();

            for (long index = 0; enumerator.MoveNext(); ++index)
                bodyWithIndex.Invoke(enumerator.Current, index);
        }
    }
}
