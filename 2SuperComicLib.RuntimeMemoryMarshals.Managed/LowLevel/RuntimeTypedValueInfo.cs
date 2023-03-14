
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

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SuperComicLib.RuntimeMemoryMarshals
{
    /// <summary>
    /// Allows access to type <typeparamref name="T"/> of <see cref="NativeInstance{T}"/> that cannot be inferred at compile time.
    /// </summary>
    [StructLayout(LayoutKind.Sequential), MonoRuntimeNotSupported]
    public readonly unsafe ref struct RuntimeTypedValueInfo<T>
    {
        private readonly byte* _Ptr;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal RuntimeTypedValueInfo(byte* _ptr) => _Ptr = _ptr;

        /// <summary>
        /// Get the value.
        /// </summary>
        /// <returns>
        /// If <typeparamref name="T"/> is a class, the return is a reference to <typeparamref name="T"/>.<br/>
        /// otherwise; the return value is a copy of <typeparamref name="T"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T GetValue()
        {
            if (typeof(T).IsClass)
            {
                var t0 = _Ptr + sizeof(void*);
                return Unsafe.As<byte, T>(ref Unsafe.AsRef<byte>(&t0));
            }
            else
                return Unsafe.AsRef<T>(_Ptr + (sizeof(void*) << 1));
        }

        /// <summary>
        /// Get structure value by reference.<para/>
        /// This method may return a null reference.<br/>
        /// Null checking can be done using <see cref="Unsafe.AsPointer{T}(ref T)"/>
        /// </summary>
        /// <returns>If <typeparamref name="T"/> is not a struct, the return is a null reference.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T GetValueDirect() =>
            ref (typeof(T).IsValueType == false) 
            ? ref Unsafe.AsRef<T>(null) 
            : ref Unsafe.AsRef<T>(_Ptr + (sizeof(void*) << 1));
    }
}
