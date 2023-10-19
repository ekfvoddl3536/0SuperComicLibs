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

#pragma warning disable IDE1006 // 명명 스타일
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using SuperComicLib.CodeContracts;

namespace SuperComicLib.Collections
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct _index_linked_vector<T> : IDisposable
        where T : unmanaged
    {
        private const int NULL_PTR = -1;

        private byte* _ptr; // first (4 -or- 8) bytes = length
        private byte* _head;
        private long _free;
        private long _size;

        #region constructor
        public _index_linked_vector(in _index_linked_vector<T> source) : this(source.size())
        {
            var sz = source.size();
            for (var node = source._head; --sz >= 0;)
            {
                p_insert_before(_head, *v_value(node));
                node = _get(source._head, *v_next(node));
            }
        }

        public _index_linked_vector(const_iterator<T> first, const_iterator<T> last) : this(last._ptr - first._ptr)
        {
            var head = _head;
            for (var iter = first._ptr; iter != last._ptr; iter++)
                p_insert_before(head, *iter);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public _index_linked_vector(long initCapacity)
        {
            _ptr = null;
            _head = null;
            _free = NULL_PTR;
            _size = 0;

            increaseCapacity(initCapacity);
        }
        #endregion

        #region indexer
        public _index_node<T> this[long raw_index] => new _index_node<T>(_get(_ptr, raw_index));

        /// <exception cref="ArgumentOutOfRangeException">out of range</exception>
        /// <exception cref="NullReferenceException">tried to access a deleted element</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public _index_node<T> at(long raw_index)
        {
            ArgValidateHelper.ThrowIfIndexOutOfRange(raw_index, capacity());

            var v = this[raw_index];
            if (v.next < 0 || // deleted node
                (v.next | v.prev) == 0 && (ulong)raw_index >= (ulong)_size)
                throw new NullReferenceException(nameof(raw_index));

            return v;
        }
        #endregion

        #region insert
        private byte* p_insert_before(byte* baseNode, in T value)
        {
            long tidx_;
            if (_free >= 0)
            {
                tidx_ = _free;
                _free = *(long*)_get_vp(_ptr, tidx_);
            }
            else if ((tidx_ = _size) >= capacity()) // no space
            {
                var baseIndex = _index(baseNode, _ptr);

                increaseCapacity(_size + 1);
                baseNode = _get(_ptr, baseIndex);
            }

            var list_ = _ptr;

            var newNode_ = _get(list_, tidx_);
            *v_value(newNode_) = value;

            *v_next(newNode_) = _index(baseNode, list_);
            *v_prev(newNode_) = *v_prev(baseNode);

            *(long*)_get(list_, *v_prev(baseNode)) = tidx_;
            *v_prev(baseNode) = tidx_;

            _size++;

            return newNode_;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long index_of(_index_node<T> node) => _index(node._ptr, _ptr);
        #endregion

        #region erase
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void erase_unsafe(_index_node<T> node)
        {
            var list_ = _ptr;

            var pnode = node._ptr;

            *(long*)_get_vp(list_, *v_next(pnode)) = *v_prev(pnode);
            *(long*)_get(list_, *v_prev(pnode)) = *v_next(pnode);

            if (_head == pnode)
                _head = _get(list_, *v_next(pnode));

            if (--_size == 0) // empty list
            {
                _head = _ptr;
                _free = NULL_PTR;
            }
            else
            {
                *v_prev(pnode) = _free;
                _free = _index(pnode, list_);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void erase(_index_node<T> node)
        {
            validate_node(node);
            erase_unsafe(node);
        }
        #endregion

        #region copyTo, toArray
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));

            if ((uint)arrayIndex >= (uint)array.Length ||
                (long)arrayIndex + array.Length < _size)
                throw new ArgumentOutOfRangeException(nameof(arrayIndex));

            fixed (T* p = &array[arrayIndex])
                _internalCopyTo(new NativeSpan<T>(p, array.Length - arrayIndex), _size);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CopyTo(in NativeSpan<T> dest) => CopyTo(dest, _size);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CopyTo(in NativeSpan<T> dest, long count)
        {
            if (dest.Source == null)
                throw new ArgumentNullException(nameof(dest));

            if ((ulong)dest.Length < (ulong)count ||
                count > _size)
                throw new ArgumentOutOfRangeException(nameof(count));

            _internalCopyTo(dest, count);
        }
        #endregion

        #region size & capacity
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long capacity() => *(long*)(_ptr - sizeof(long));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long size() => _size;
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
        [MethodImpl(MethodImplOptions.AggressiveInlining), X64LossOfLength]
        public T[] ToArray()
        {
            var sz = (int)_size;
            if (sz <= 0)
                return Array.Empty<T>();

            T[] newArr = new T[sz];

            fixed (T* p = &newArr[0])
                _internalCopyTo(new NativeSpan<T>(p, sz), sz);

            return newArr;
        }
        #endregion

        #region common clear items method
        [MethodImpl(MethodImplOptions.AggressiveInlining), NoExcept]
        public void clear()
        {
            var list_ = _ptr;

            var next = _head;
            for (var sz = _size; --sz >= 0;)
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

        [MethodImpl(MethodImplOptions.AggressiveInlining), NoExcept]
        public void fast_clear()
        {
            _head = _ptr;

            *v_next(_head) = default;
            *v_prev(_head) = default;

            _free = NULL_PTR;
            _size = default;
        }
        #endregion

        #region dispose
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose()
        {
            if (_ptr != null)
                Marshal.FreeHGlobal((IntPtr)(_ptr - sizeof(long)));
        }
        #endregion

        #region private method
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void _internalCopyTo(in NativeSpan<T> dest, long count)
        {
            T* pdst = dest.Source;

            byte* bp = _ptr;
            for (byte* p = _head; count-- > 0;)
            {
                *pdst++ = *v_value(p);
                p = _get(bp, *v_next(p));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void validate_node(_index_node<T> node)
        {
            long index = _index(node._ptr, _ptr);
            ArgValidateHelper.ThrowIfIndexOutOfRange(index, capacity());

            if (node.next < 0 ||
                (node.next | node.prev) == 0 && (ulong)index >= (ulong)_size)
                throw new InvalidOperationException($"Dereference to {nameof(NULL_PTR)}");
        }

        private void increaseCapacity(long req_size)
        {
            var sz = (long)CMathi.Max((ulong)req_size, 4u);

            var new_sizeInBytes = _sizeInBytes(Arrays.GetNextSize((int)sz));
            if (new_sizeInBytes <= sz)
                throw new OutOfMemoryException(nameof(_index_linked_vector<T>));

            var np = (byte*)Marshal.AllocHGlobal((IntPtr)(new_sizeInBytes + sizeof(long)));

            *(long*)np = sz;

            var vs_np = np + sizeof(long);

            var copysize = (ulong)_sizeInBytes(_size);
            MemoryBlock.Memmove<T>(_ptr, vs_np, copysize);

            MemoryBlock.Clear(_get(vs_np, _size), (ulong)new_sizeInBytes - copysize);

            if (_ptr != null)
                Marshal.FreeHGlobal((IntPtr)(_ptr - sizeof(long)));

            _head = vs_np + _index(_head, _ptr);
            _ptr = vs_np;
        }
        #endregion

        #region helper methods
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long _sizeInBytes(long size) => (sizeof(long) + sizeof(long) + sizeof(T)) * size;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long _index(byte* p, byte* b) => (p - b) / (sizeof(long) + sizeof(long) + sizeof(T));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static byte* _get(byte* p, long idx) => p + (sizeof(long) + sizeof(long) + sizeof(T)) * idx;
        // get, value prev (pivot)
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static byte* _get_vp(byte* p, long idx) => _get(p, idx) + sizeof(long);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long* v_next(byte* p) => (long*)p;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long* v_prev(byte* p) => (long*)(p + sizeof(long));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static T* v_value(byte* p) => (T*)(p + sizeof(long) + sizeof(void**));
        #endregion
    }
}
