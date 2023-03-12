// MIT License
//
// Copyright (c) 2023. SuperComic (ekfvoddl3535@naver.com)
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

#pragma warning disable IDE0060
#pragma warning disable CS1591
using System;
using System.Runtime.CompilerServices;

namespace SuperComicLib.RuntimeMemoryMarshals
{
    /// <summary>
    /// Contains generic, low-level functionality for manipulating managed and unmanaged pointers,
    /// that assist '<see href="System.Runtime.CompilerServices.Unsafe"/>'
    /// </summary>
    public static unsafe class ILUnsafe
    {
        /// <summary>
        /// Convert a pointer address to a managed class.
        /// <br/>
        /// <c>T cls = (T)(void*)value</c>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T AsClass<T>(void* value) where T : class => throw new PlatformNotSupportedException();
        /// <summary>
        /// Convert a reference address to a managed class.
        /// <br/>
        /// <c>T cls = (T)(void*)value</c>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T AsClass<T>(ref byte value) where T : class => throw new PlatformNotSupportedException();

        /// <summary>
        /// Get the instance address of the managed class.
        /// <br/>
        /// <c>void* cls = (void*)value</c>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void* AsPointer<T>(T value) where T : class => throw new PlatformNotSupportedException();
        /// <summary>
        /// Get the instance address of the managed class.
        /// <br/>
        /// <c>void* cls = (byte*)value + byteOffset</c>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void* AsPointer<T>(T value, IntPtr byteOffset) where T : class => throw new PlatformNotSupportedException();

        /// <summary>
        /// Get the instance address of the managed class.
        /// <br/>
        /// <c>ref byte cls = ref (*byte*)value</c>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref byte AsRef<T>(T value) where T : class => throw new PlatformNotSupportedException();
        /// <summary>
        /// Get the instance address of the managed class.
        /// <br/>
        /// <c>ref byte cls = ref *((byte*)value + byteOffset)</c>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref byte AsRef<T>(T value, IntPtr byteOffset) where T : class => throw new PlatformNotSupportedException();

        /// <summary>
        /// Get a reference to the instance data.
        /// <br/>
        /// <c>ref TField cls = ref *(TField*)value.firstFieldData</c>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref TField GetDataRef<TClass, TField>(TClass value) where TClass : class => throw new PlatformNotSupportedException();
        /// <summary>
        /// Get a reference to the instance data.
        /// <br/>
        /// <c>ref TField cls = ref *(TField*)((byte*)value.firstFieldData + byteOffset)</c>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref TField GetDataRef<TClass, TField>(TClass value, IntPtr byteOffset) where TClass : class => throw new PlatformNotSupportedException();

        /// <summary>
        /// Convert type
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref readonly TTo ConstAs<TFrom, TTo>(in TFrom source) => throw new PlatformNotSupportedException();

        /// <summary>
        /// Reference address value compare
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Compare(object left, object right) => throw new PlatformNotSupportedException();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref T AddByteOffset<T>(void* source, int byteOffset) => throw new PlatformNotSupportedException();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref T Add<T>(void* source, int elementOffset, int addByteOffset) => throw new PlatformNotSupportedException();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref readonly T Add<T>(in T source, int elementOffset) => throw new PlatformNotSupportedException();

        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        // public static T* Add<T>(T* source, nint_t index) where T : unmanaged => throw new PlatformNotSupportedException();
        // 
        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        // public static T* Sub<T>(T* source, nint_t index) where T : unmanaged => throw new PlatformNotSupportedException();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ConvI4(bool value) => throw new PlatformNotSupportedException();
    }
}
