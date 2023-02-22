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

#pragma warning disable IDE1006 // 명명 스타일
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SuperComicLib.Collections
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe partial struct _index_linked_vector<T> : IDisposable
        where T : unmanaged
    {
        private const int NULL_PTR = -1;

        #region common constructor
        public _index_linked_vector(in _index_linked_vector<T> source) : this(source.size())
        {
            var sz = source.size();
            for (var node = source._head; --sz >= 0;)
            {
                p_insert_before(_head, *v_value(node));
                node = _get(source._head, *v_next(node));
            }
        }

        public _index_linked_vector(const_iterator<T> first, const_iterator<T> last) :
#if X86
            this((int)(last._ptr - first._ptr))
#else
            this(last._ptr - first._ptr)
#endif
        {
            var head = _head;
            for (var iter = first._ptr; iter != last._ptr; iter++)
                p_insert_before(head, *iter);
        }
#endregion

#region common get item method
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public _index_node<T> first() =>
            _size != 0
            ? new _index_node<T>(_head)
            : default;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public _index_node<T> last() =>
            _size != 0
            ? new _index_node<T>(_get(_ptr, *v_prev(_head)))
            : default;
#endregion

#region common add item method
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public _index_node<T> add_last(in T value) => new _index_node<T>(p_insert_before(_head, value));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public _index_node<T> add_first(in T value) => new _index_node<T>(_head = p_insert_before(_head, value));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public _index_node<T> add_after(_index_node<T> node, in T value)
        {
            validate_node(node);
            return add_after_unsafe(node, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public _index_node<T> add_before(_index_node<T> node, in T value)
        {
            validate_node(node);
            return add_before_unsafe(node, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public _index_node<T> add_after_unsafe(_index_node<T> node, in T value) => 
            new _index_node<T>(p_insert_before(_get(_ptr, node.next), value));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public _index_node<T> add_before_unsafe(_index_node<T> node, in T value)
        {
            var result = p_insert_before(node._ptr, value);

            if (node._ptr == _head) // head 위치 이전에 삽입 - add_first와 같음
                _head = result;

            return new _index_node<T>(result);
        }
        #endregion

        #region common managed/Array method
#if !AnyCPU
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T[] ToArray()
        {
            var sz = _size;
            if (sz == 0)
                return Array.Empty<T>();

            T[] newArr = new T[sz];

            fixed (T* p = &newArr[0])
                _internalCopyTo(new NativeSpan<T>(p, sz), sz);

            return newArr;
        }
#endif
        #endregion

        #region common clear items method
        [MethodImpl(MethodImplOptions.AggressiveInlining), CodeContracts.NoExcept]
        public void clear()
        {
            var list_ = _ptr;

            var next = _head;
#if AnyCPU
            for (var sz = _size; ptr_math.isNeg(--sz) == false;)
#else
            for (var sz = _size; --sz >= 0;)
#endif
            {
                var curr = next;
                next = _get(list_, *v_next(curr));

                // reset
                *v_next(curr) = default;
                *v_prev(curr) = default;
                *v_value(curr) = default;
            }

            _head = _ptr;
            _free = NULL_PTR;
            _size = default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining), CodeContracts.NoExcept]
        public void fast_clear()
        {
            _head = _ptr;
            
            *v_next(_head) = default;
            *v_prev(_head) = default;

            _free = NULL_PTR;
            _size = default;
        }
        #endregion
    }
}
