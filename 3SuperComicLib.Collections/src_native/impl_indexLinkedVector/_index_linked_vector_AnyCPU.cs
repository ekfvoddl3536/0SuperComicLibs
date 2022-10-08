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

#if AnyCPU
#pragma warning disable IDE1006 // 명명 스타일
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SuperComicLib.Collections
{
    unsafe partial struct _index_linked_vector<T>
    {
        // size_t next
        // size_t prev
        // T value
        private byte* _ptr; // first 4 byte = length
        private byte* _head;
        private size_t _free;
        private size_t _size;

#region constructor
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public _index_linked_vector(size_t initCapacity) 
        {
            _ptr = null;
            _head = null;
            _free = sizeof(void*) == sizeof(int) ? NULL_PTR : (long)NULL_PTR;
            _size = 0;

            increaseCapacity(initCapacity);
        }
#endregion

#region size & capacity
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public size_t capacity() => *(size_t*)(_ptr - sizeof(size_t));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public size_t size() => _size;
#endregion

#region indexer
        public _index_node<T> this[size_t raw_index] => new _index_node<T>(_get(_ptr, raw_index));

        /// <exception cref="ArgumentOutOfRangeException">out of range</exception>
        /// <exception cref="NullReferenceException">tried to access a deleted element</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public _index_node<T> at(size_t raw_index)
        {
            if (raw_index >= capacity())
                throw new ArgumentOutOfRangeException(nameof(raw_index));

            var v = this[raw_index];
            if (v.next < 0 || // deleted node
                ptr_math.or(v.next, v.prev) == 0 && (raw_index != 0 || _size != 1))
                throw new NullReferenceException(nameof(raw_index));

            return v;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public size_t index_of(_index_node<T> node) => _index(node._ptr, _ptr);
#endregion

#region insert
        private byte* p_insert_before(byte* baseNode, in T value)
        {
            size_t tidx_;
            if (!ptr_math.isNeg(_free))
            {
                tidx_ = _free;
                _free = *(size_t*)_get_vp(_ptr, tidx_);
            }
            else if ((tidx_ = _size) >= capacity()) // no space
                increaseCapacity(_size);

            var list_ = _ptr;

            var newNode_ = _get(list_, tidx_);
            *v_value(newNode_) = value;

            *v_next(newNode_) = _index(baseNode, list_);
            *v_prev(newNode_) = *v_prev(baseNode);

            *(size_t*)_get(list_, *v_prev(baseNode)) = tidx_;
            *v_prev(baseNode) = tidx_;

            _size++;

            return newNode_;
        }
#endregion

#region erase
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void erase_unsafe(_index_node<T> node)
        {
            var list_ = _ptr;

            var pnode = node._ptr;

            *(size_t*)_get_vp(list_, *v_next(pnode)) = *v_prev(pnode);
            *(size_t*)_get(list_, *v_prev(pnode)) = *v_next(pnode);

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

#region private method
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void validate_node(_index_node<T> node)
        {
            size_t index = _index(node._ptr, _ptr);
            if (index >= _size)
                throw new ArgumentOutOfRangeException(nameof(node));

            if (node.next < 0 ||
                ptr_math.or(node.next, node.prev) == 0 && index != 0)
                throw new InvalidOperationException($"Dereference to {nameof(NULL_PTR)}");
        }

        private void increaseCapacity(size_t req_size)
        {
            byte* vs_np;
            if (sizeof(void*) == sizeof(int))
            {
                int sz = (int)CMath.Max((uint)req_size.value, 4u);

                int new_sizeInBytes = (int)_sizeInBytes(Arrays.GetNextSize(sz)).value;
                var np = (byte*)Marshal.AllocHGlobal(new_sizeInBytes + sizeof(int));

                *(int*)np = sz;

                vs_np = np + sizeof(int);

                ulong copysize = (uint)_sizeInBytes(_size).value;
                Buffer.MemoryCopy(_ptr, vs_np, copysize, copysize);

                MemoryBlock.Clear32(_get(vs_np, _size), (uint)new_sizeInBytes - (uint)copysize);
            }
            else
            {
                long sz = (long)CMath.Max((ulong)req_size.value, 4u);

                long new_sizeInBytes = (long)_sizeInBytes(Arrays.GetNextSize(sz)).value;
                var np = (byte*)Marshal.AllocHGlobal((IntPtr)(new_sizeInBytes + sizeof(long)));

                *(long*)np = sz;

                vs_np = np + sizeof(long);

                ulong copysize = (ulong)_sizeInBytes(_size).value;
                Buffer.MemoryCopy(_ptr, vs_np, copysize, copysize);

                MemoryBlock.Clear64(_get(vs_np, _size), (ulong)new_sizeInBytes - copysize);
            }

            if (_ptr != null)
                Marshal.FreeHGlobal((IntPtr)(_ptr - sizeof(size_t)));

            _head = ptr_offset<byte>.add(vs_np, _index(_head, _ptr));
            _ptr = vs_np;
        }
#endregion

#region dispose
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose()
        {
            if (_ptr != null)
                Marshal.FreeHGlobal((IntPtr)(_ptr - sizeof(size_t)));
        }
#endregion

#region helper methods
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static size_t _sizeInBytes(size_t size) =>
            sizeof(void*) == sizeof(int)
            ? (8 + sizeof(T)) * (int)size.value
            : (16 + sizeof(T)) * (long)size.value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static size_t _index(byte* p, byte* b) =>
            sizeof(void*) == sizeof(int)
            ? (p - b) / (8 + sizeof(T))
            : (p - b) / (16 + sizeof(T));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static byte* _get(byte* p, size_t idx) =>
            sizeof(void*) == sizeof(int)
            ? p + (8 + sizeof(T)) * (int)idx.value
            : p + (16 + sizeof(T)) * (long)idx.value;

        // get, value prev (pivot)
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static byte* _get_vp(byte* p, size_t idx) => _get(p, idx) + sizeof(size_t);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static size_t* v_next(byte* p) => (size_t*)p;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static size_t* v_prev(byte* p) => (size_t*)(p + sizeof(size_t));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static T* v_value(byte* p) => (T*)(p + sizeof(size_t) + sizeof(size_t));
#endregion
    }
}
#endif