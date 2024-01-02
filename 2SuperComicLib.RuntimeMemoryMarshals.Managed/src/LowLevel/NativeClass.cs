// MIT License
//
// Copyright (c) 2019-2024. SuperComic (ekfvoddl3535@naver.com)
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

using SuperComicLib.CodeContracts;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;

namespace SuperComicLib.RuntimeMemoryMarshals
{
    /// <summary>
    /// Features related to the memory of managed objects.
    /// </summary>
    [SuppressUnmanagedCodeSecurity]
    public static unsafe class NativeClass
    {
        /// <summary>
        /// Get a reference to the MethodTable. CoreCLR (.NET Framework, .NET) only.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining), AssumeInputsValid, MonoRuntimeNotSupported]
        public static ref readonly PubMethodTable GetMethodTable([DisallowNull] Type type) => ref *(PubMethodTable*)type.TypeHandle.Value;

        /// <summary>
        /// Get a instance size of <typeparamref name="T"/>. CoreCLR (.NET Framework, .NET) only.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining), NoOverhead, MonoRuntimeNotSupported]
        public static int InstanceSizeOf<T>() => *((int*)typeof(T).TypeHandle.Value + 1);

        /// <summary>
        /// Get a instance size of <paramref name="type"/>. CoreCLR (.NET Framework, .NET) only.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining), AssumeInputsValid, NoOverhead, MonoRuntimeNotSupported]
        public static int InstanceSizeOf([DisallowNull] Type type) => *((int*)type.TypeHandle.Value + 1);

        /// <summary>
        /// Get a instance size of object from <paramref name="typeHandle"/>. CoreCLR (.NET Framework, .NET) only.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining), AssumeInputsValid, NoOverhead, MonoRuntimeNotSupported]
        public static int InstanceSizeOf([DisallowNull, ValidRange] IntPtr typeHandle) => *((int*)typeHandle + 1);

        /// <summary>
        /// Creates a managed object in unmanaged memory.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining), MonoRuntimeNotSupported]
        public static NativeInstance<T> Alloc<T>()
        {
            var type = typeof(T);
            DEBUG_TYPE_VALIDATION(type);

            var tHnd = type.TypeHandle.Value;

            var ptr = (IntPtr*)Marshal.AllocHGlobal(InstanceSizeOf(tHnd));

            ptr[0] = IntPtr.Zero;
            ptr[1] = tHnd;

            return new NativeInstance<T>(ptr);
        }

        /// <summary>
        /// Creates an initialized managed object in unmanaged memory.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        [MethodImpl(MethodImplOptions.NoInlining), MonoRuntimeNotSupported]
        public static NativeInstance<T> AllocZeroed<T>()
        {
            var type = typeof(T);
            DEBUG_TYPE_VALIDATION(type);

            var tHnd = type.TypeHandle.Value;

            var tSz = InstanceSizeOf(tHnd);

            var ptr = (IntPtr*)Marshal.AllocHGlobal(tSz);

            ptr[0] = IntPtr.Zero;
            ptr[1] = tHnd;

            MemoryBlock.Clear((byte*)(ptr + 2), (uint)(tSz - (sizeof(long) << 1)));

            return new NativeInstance<T>(ptr);
        }

        [Conditional("DEBUG")]
        private static void DEBUG_TYPE_VALIDATION(Type v)
        {
            if (v.IsArray ||
                v.IsPointer || v.IsByRef ||
                v.IsInterface || v.IsEnum ||
                v.IsPrimitive || v == typeof(string))
                throw new InvalidOperationException($"'{v.FullName}' is not allowed.");
        }
    }
}
