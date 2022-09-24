﻿#pragma warning disable IDE1006 // 명명 스타일
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SuperComicLib.Collections
{
    [StructLayout(LayoutKind.Sequential), CodeContracts.ParamRef]
    public unsafe partial struct _index_linked_vector<T> : IDisposable
        where T : unmanaged
    {
        private const int NULL_PTR = -1;

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

        #region common clear items method
        [MethodImpl(MethodImplOptions.AggressiveInlining), CodeContracts.NoExcept]
        public void Clear()
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
            }

            _head = _ptr;
            _free = default;
            _size = default;
        }
#endregion
    }
}