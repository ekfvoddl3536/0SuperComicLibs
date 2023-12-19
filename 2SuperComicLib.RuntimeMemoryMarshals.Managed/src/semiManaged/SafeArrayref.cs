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
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using SuperComicLib.CodeContracts;

namespace SuperComicLib.RuntimeMemoryMarshals
{
    /// <summary>
    /// An <see cref="arrayref{T}"/> that can be automatically collected by the <see cref="GC"/>.<para/>
    /// For safe usage, some of the features and extended features provided by <see cref="arrayref{T}"/> are not available.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [DebuggerTypeProxy(typeof(SemiManagedArrayElementDebugView<>))]
    [DebuggerDisplay("{Length}")]
    public sealed unsafe class SafeArrayref<T> : ICloneable, IList<T>, IReadOnlyList<T>, IStructuralEquatable, IStructuralComparable
        where T : unmanaged
    {
        private static long _tempLock;
        private static SafeArrayref<T> _EmptyArray;

        public static SafeArrayref<T> Empty
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if (_EmptyArray == null)
                {
                    for (SpinWait w = default; Interlocked.CompareExchange(ref _tempLock, 1, 0) != 0;)
                        w.SpinOnce();

                    try
                    {
                        if (_EmptyArray == null)
                            _EmptyArray = new SafeArrayref<T>();
                    }
                    finally
                    {
                        Interlocked.Exchange(ref _tempLock, 0);
                    }
                }

                return _EmptyArray;
            }
        }

        internal readonly arrayref<T> _arr;

        #region constructors
        private SafeArrayref() => _arr = arrayref<T>.newf(0);

        public SafeArrayref(int length) : this(length, false)
        {
        }

        public SafeArrayref(int length, bool skipZeroInitialize)
        {
            // .NET 6 -> Array.MaxLength
            if ((uint)length > Arrayrefs.MaxLength)
                throw new ArgumentOutOfRangeException(nameof(length));

            _arr =
                length == 0
                ? Empty._arr
                : arrayref<T>.newf(length);

            if (!skipZeroInitialize)
                Array.Clear(_arr.AsManaged(), 0, length);
        }

        public SafeArrayref(IEnumerable<T> collection)
        {
            if (collection == null) 
                throw new ArgumentNullException(nameof(collection));

            int length = collection.Count();
            if (length == 0)
            {
                _arr = Empty._arr;
                return;
            }

            var temp = arrayref<T>.newf(length);

            ref var dest = ref temp[0];
            foreach (var item in collection)
            {
                dest = item;
                dest = ref ILUnsafe.Add(ref dest, 1);
            }

            _arr = temp;
        }

        /// <summary>
        /// Initializes the <see cref="SafeArrayref{T}"/> using the provided source <see cref="arrayref{T}"/>.
        /// </summary>
        /// <param name="source">The source <see cref="arrayref{T}"/>.</param>
        /// <param name="keepAlive">If set to <see langword="true"/>, it creates a new array and copies the contents of the <paramref name="source"/>.<br/>
        /// If set to <see langword="false"/>, the <paramref name="source"/> will be disposed of when the current <see cref="SafeArrayref{T}"/> is destroyed.
        /// </param>
        public SafeArrayref(in arrayref<T> source, bool keepAlive = true)
        {
            if (source.IsNull)
                throw new ArgumentNullException(nameof(source));

            if (keepAlive)
            {
                var temp = arrayref<T>.newf(source.Length);
                Array.Copy(source.AsManaged(), temp.AsManaged(), temp.Length);
                _arr = temp;
            }
            else
                _arr = source;
        }

        public SafeArrayref(T[] source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (source.Length == 0)
                _arr = Empty._arr;
            else
            {
                var temp = arrayref<T>.newf(source.Length);
                Array.Copy(source, temp.AsManaged(), source.Length);
                _arr = temp;
            }
        }

        /// <summary>
        /// iterators to the initial and final positions in a range.
        /// <para/>
        /// The range used is <c>[<paramref name="first"/>, <paramref name="last"/>)</c>, which includes all the elements between <paramref name="first"/> and <paramref name="last"/>.<br/>
        /// Including the element pointed by <paramref name="first"/> but not the element pointed by <paramref name="last"/>.
        /// </summary>
        /// <param name="first">The starting address of the range, including the pointed element.</param>
        /// <param name="last">The ending address of the range, <b>excluding</b> the pointed element.</param>
        public SafeArrayref(arrayref<T>.const_iterator first, arrayref<T>.const_iterator last)
        {
            long length = last - first;
            if (length > Arrayrefs.MaxLength)
                throw new ArgumentOutOfRangeException($"{nameof(first)} and {nameof(last)}");

            if (length == 0)
            {
                _arr = Empty._arr;
                return;
            }

            var temp = arrayref<T>.newf((int)length);

            var iter = temp.begin();
            for (var end = iter + length; iter != end; ++iter, ++first)
                iter.value = first.value;
        }
        #endregion

        #region properties
        public bool IsCLRArray
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _arr.IsCLRArray;
        }

        public int Length
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _arr.Length;
        }

        public long LongLength
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _arr.LongLength;
        }
        #endregion

        #region indexer
        /// <summary>
        /// References the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to reference</param>
        /// <returns>A reference to the element at the specified index.</returns>
        /// <exception cref="IndexOutOfRangeException">
        /// <paramref name="index"/> is less than 0.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="index"/> is equal to or greater than <see cref="Length"/>
        /// </exception>
        public ref T this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if ((uint)index >= (uint)_arr.Length)
                    throw new IndexOutOfRangeException(nameof(index));

                return ref _arr[index];
            }
        }

        /// <summary>
        /// References the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to reference</param>
        /// <returns>A reference to the element at the specified index.</returns>
        /// <exception cref="IndexOutOfRangeException">
        /// <paramref name="index"/> is less than 0.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="index"/> is equal to or greater than <see cref="LongLength"/>
        /// </exception>
        public ref T this[long index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if ((ulong)index >= (ulong)_arr.LongLength)
                    throw new IndexOutOfRangeException(nameof(index));

                return ref _arr[index];
            }
        }

        /// <summary>
        /// References the element at the specified index.
        /// <para/>
        /// This method does not perform range checks on the provided <paramref name="index"/> parameter.
        /// </summary>
        /// <param name="index">The zero-based index of the element to reference</param>
        /// <returns>A reference to the element at the specified index.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining), AssumeInputsValid, AssumeOperationValid]
        public ref T at_fast([ValidRange] int index) => ref _arr[index];

        /// <summary>
        /// References the element at the specified index.
        /// <para/>
        /// This method does not perform range checks on the provided <paramref name="index"/> parameter.
        /// </summary>
        /// <param name="index">The zero-based index of the element to reference</param>
        /// <returns>A reference to the element at the specified index.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining), AssumeInputsValid, AssumeOperationValid]
        public ref T at_fast([ValidRange] long index) => ref _arr[index];
        #endregion

        #region convert
        /// <summary>
        /// Creates a new managed array and copies the data.
        /// <para/>
        /// This method <see cref="Array.Copy(Array, Array, int)"><b>creates a copy</b></see> and does not pass a reference to the original array.
        /// </summary>
        public T[] ToArray()
        {
            var src = _arr.AsManaged();
            if (src.Length == 0)
                return Array.Empty<T>();

            var res = new T[src.Length];
            Array.Copy(src, res, src.Length);

            return res;
        }

        /// <summary>
        /// Casts using the reference of the semi-managed array to a managed array.
        /// <para/>
        /// This method <see cref="object"><b>does not create a copy</b></see> and passes a reference to the original array.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T[] AsManaged() => _arr.AsManaged();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Memory<T> AsMemory() => _arr.AsMemory();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Memory<T> Slice(int startIndex) => _arr.AsMemory().Slice(startIndex);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Memory<T> Slice(int startIndex, int count) => _arr.AsMemory().Slice(startIndex, count);
        #endregion

        #region array methods
        public bool TrueForAll(Predicate<T> match)
        {
            var vs = _arr.AsManaged();
            for (int i = 0; i < vs.Length; ++i)
                if (!match.Invoke(vs[i]))
                    return false;

            return true;
        }

        public bool Exist(Predicate<T> match)
        {
            var vs = _arr.AsManaged();
            for (int i = 0; i < vs.Length; ++i)
                if (match.Invoke(vs[i]))
                    return true;

            return false;
        }

        public int LastIndexOf(T item)
        {
            var comparer = EqualityComparer<T>.Default;

            ref readonly var arr = ref _arr;
            for (int i = arr.Length; --i >= 0;)
                if (comparer.Equals(arr[i], item))
                    return i;

            return -1;
        }

        public int IndexOf(T item)
        {
            var comparer = EqualityComparer<T>.Default;

            var vs = _arr.AsManaged();
            for (int i = 0; i < vs.Length; ++i)
                if (comparer.Equals(vs[i], item))
                    return i;

            return -1;
        }
        
        public bool Contains(T item) => IndexOf(item) >= 0;

        public void CopyTo(T[] array, int arrayIndex)
        {
            var source = _arr.AsManaged();
            Array.Copy(source, 0, array, arrayIndex, source.Length);
        }
        #endregion

        #region explicit implements
        T IList<T>.this[int index]
        {
            get => this[index];
            set => this[index] = value;
        }
        T IReadOnlyList<T>.this[int index] => this[index];
        int IReadOnlyCollection<T>.Count => _arr.Length;
        int ICollection<T>.Count => _arr.Length;
        bool ICollection<T>.IsReadOnly => true;

        void IList<T>.Insert(int index, T item) => throw new NotSupportedException(Arrayrefs.ERROR_FIXEDSIZECOLLECTION);
        void IList<T>.RemoveAt(int index) => throw new NotSupportedException(Arrayrefs.ERROR_FIXEDSIZECOLLECTION);
        void ICollection<T>.Add(T item) => throw new NotSupportedException(Arrayrefs.ERROR_FIXEDSIZECOLLECTION);
        bool ICollection<T>.Remove(T item) => throw new NotSupportedException(Arrayrefs.ERROR_FIXEDSIZECOLLECTION);
        void ICollection<T>.Clear() => throw new NotSupportedException(Arrayrefs.ERROR_FIXEDSIZECOLLECTION);
        #endregion

        #region IStructuralComparable, IClone
        int IStructuralComparable.CompareTo(object other, IComparer comparer)
        {
            if (other == null)
                return 1;

            if (!(other is SafeArrayref<T> temp))
                throw new ArgumentException("TypeError", nameof(other));

            ref readonly var left = ref _arr;
            ref readonly var right = ref temp._arr;
            if (left.Length != right.Length)
                throw new ArgumentException("UnequalLength", nameof(other));

            ref var l_iter = ref left[0];
            ref var l_end = ref left[left.Length];

            ref var r_iter = ref right[0];
            while (!ILUnsafe.AreSame(l_iter, l_end))
            {
                int c = comparer.Compare(l_iter, l_end);
                if (c != 0)
                    return c;

                l_iter = ref ILUnsafe.Increment(ref l_iter);
                r_iter = ref ILUnsafe.Increment(ref r_iter);
            }

            return 0;
        }

        bool IStructuralEquatable.Equals(object other, IEqualityComparer comparer)
        {
            if (this == other)
                return true;

            if (!(other is SafeArrayref<T> temp))
                return false;

            ref readonly var left = ref _arr;
            ref readonly var right = ref temp._arr;
            if (left.Length != right.Length)
                return false;

            ref var l_iter = ref left[0];
            ref var l_end = ref left[left.Length];

            ref var r_iter = ref right[0];
            while (!ILUnsafe.AreSame(l_iter, l_end))
            {
                if (!comparer.Equals(l_iter, r_iter))
                    return false;

                l_iter = ref ILUnsafe.Increment(ref l_iter);
                r_iter = ref ILUnsafe.Increment(ref r_iter);
            }

            return true;
        }

        int IStructuralEquatable.GetHashCode(IEqualityComparer comparer)
        {
            var vs = _arr.AsManaged();

            int ret = 0;

            for (int i = CMath.Max(vs.Length - 8, 0); i < vs.Length; ++i)
                ret = CombineHashCodes(ret, comparer.GetHashCode(vs[i]));

            return ret;
        }

        // from https://referencesource.microsoft.com/#mscorlib/system/array.cs,87d117c8cc772cca,references
        private static int CombineHashCodes(int h1, int h2) => ((h1 << 5) + h1) ^ h2;

        /// <summary>
        /// Creates a shallow copy of the <see cref="SafeArrayref{T}"/>.
        /// </summary>
        /// <returns>A shallow copy of the <see cref="SafeArrayref{T}"/>.</returns>
        public object Clone()
        {
            var src = _arr.AsManaged();
            var res = new SafeArrayref<T>(src.Length, true);

            Array.Copy(src, res.AsManaged(), src.Length);

            return res;
        }
        #endregion

        #region enumerator
        public IEnumerator<T> GetEnumerator() => (IEnumerator<T>)_arr.AsManaged().GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        #endregion

        #region iterator
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public arrayref<T>.iterator begin() => _arr.begin();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public arrayref<T>.iterator end() => _arr.end();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public arrayref<T>.reverse_iterator rbegin() => _arr.rbegin();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public arrayref<T>.reverse_iterator rend() => _arr.rend();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public arrayref<T>.const_iterator cbegin() => begin();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public arrayref<T>.const_iterator cend() => end();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public arrayref<T>.const_reverse_iterator crbegin() => rbegin();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public arrayref<T>.const_reverse_iterator crend() => rend();
        #endregion

        #region override
        public override int GetHashCode() => ((long)_arr._pClass).GetHashCode();
        #endregion

        #region finalize
        ~SafeArrayref()
        {
            ref readonly var arr = ref _arr;
            if (arr.IsNull)
                return;

            if (_EmptyArray == null || _EmptyArray._arr._pClass != arr._pClass)
                arr.Dispose();
        }
        #endregion

        #region operator
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator T[](SafeArrayref<T> array) => array._arr.AsManaged();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator SafeArrayref<T>(T[] array) => new SafeArrayref<T>(array);
        #endregion
    }
}
