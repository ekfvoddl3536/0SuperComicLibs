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

namespace SuperComicLib.LowLevel
{
    public readonly unsafe struct NativeInstance<T> : IDisposable
        where T : class
    {
        #region static
        internal static readonly NativeClass<T> inst;
        internal static readonly IntPtr typehnd;
        internal static int _size() => ((int*)typehnd)[1];

        static NativeInstance()
        {
            inst = NativeClass<T>.Instance;
            typehnd = typeof(T).TypeHandle.Value;
        }
        #endregion

        public readonly byte* _Ptr;

        private NativeInstance(byte* ptr) => _Ptr = ptr;

        #region methods & properties
        public T Value => inst.Cast(_Ptr + sizeof(void*));

        public byte* this[int byte_offset] => _Ptr + (sizeof(void*) << 1) + byte_offset;
        #endregion

        #region override
        public void Dispose()
        {
            if (_Ptr != null)
            {
                if (Value is IDisposable d)
                    d.Dispose();

                Marshal.FreeHGlobal((IntPtr)_Ptr);
            }
        }

        public override bool Equals(object obj) => false;

        public override int GetHashCode() => 
            _Ptr == null 
            ? -1
            : Value.GetHashCode();
        #endregion

        #region static methods
        public static bool operator ==(NativeInstance<T> left, NativeInstance<T> right) => left._Ptr == right._Ptr;
        public static bool operator !=(NativeInstance<T> left, NativeInstance<T> right) => left._Ptr != right._Ptr;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NativeInstance<T> Clone(T obj)
        {
            if (obj == null)
                return default;

            byte* ptr = NativeClass.Internal_Alloc(_size(), false);
            *(IntPtr*)(ptr + sizeof(void*)) = typehnd;

            TypedReference tr = __makeref(obj);

            ulong sizeInBytes = (uint)(_size() - sizeof(void*));
            Buffer.MemoryCopy(**(byte***)&tr + sizeof(void*), ptr + (sizeof(void*) << 1), sizeInBytes, sizeInBytes);

            return new NativeInstance<T>(ptr);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NativeInstance<T> Alloc(bool zeromem = true)
        {
            byte* ptr = NativeClass.Internal_Alloc(_size(), zeromem);
            *(IntPtr*)(ptr + sizeof(void*)) = typehnd;

            return new NativeInstance<T>(ptr);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NativeInstance<T> Alloc(int additional_size, bool zeromem = true)
        {
            if (additional_size < 0)
                throw new InvalidOperationException();

            byte* ptr = NativeClass.Internal_Alloc(_size() + additional_size, zeromem);
            *(IntPtr*)(ptr + sizeof(void*)) = typehnd;

            return new NativeInstance<T>(ptr);
        }
        #endregion
    }
}
