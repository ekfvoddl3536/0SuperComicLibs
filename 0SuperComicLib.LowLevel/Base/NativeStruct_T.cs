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
using System.Security;

namespace SuperComicLib.LowLevel
{
    [SuppressUnmanagedCodeSecurity]
    public unsafe sealed class NativeStruct<T> : PointerMethods<T> 
        where T : struct
    {
        #region instance members
        private readonly UnsafeReadPointerStruct<T> read;

        internal NativeStruct() => read = NativeClass.CreateReadPointer<T>(typeof(NativeStruct<T>));

        public T Read(void* ptr) => read.Invoke(ptr);

        public override TOut Get<TOut>(ref T inst, int offset)
        {
            TypedReference tr = __makeref(inst);
            return *(TOut*)(*(byte**)&tr + offset);
        }

        public override void Set<TIn>(ref T inst, TIn value, int offset)
        {
            TypedReference tr = __makeref(inst);
            *(TIn*)(*(byte**)&tr + offset) = value;
        }

        public override T Default(void* ptr) => Read(ptr);

        public override void RefMemory(ref T obj, UnsafePointerAction cb)
        {
            TypedReference tr = __makeref(obj);
            cb.Invoke(*(byte**)&tr);
        }
        #endregion

        #region static members
        private static NativeStruct<T> m_instance;

        public static NativeStruct<T> Instance => m_instance ?? (m_instance = new NativeStruct<T>());
        #endregion
    }
}