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

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using SuperComicLib.CodeContracts;

namespace SuperComicLib.RuntimeMemoryMarshals
{
    /// <summary>
    /// Features related to the memory of managed objects.
    /// </summary>
    [SuppressUnmanagedCodeSecurity]
    public static unsafe class NativeClass
    {
        /// <summary>
        /// Get a reference to the MethodTable. dotnet (CoreCLR) only.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining), AssumeInputsValid, MonoRuntimeNotSupported]
        public static ref readonly PubMethodTable GetMethodTable([DisallowNull] Type type) => ref *(PubMethodTable*)type.TypeHandle.Value;

        /// <summary>
        /// Get a instance size of <typeparamref name="T"/>. dotnet (CoreCLR) only.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining), NoOverhead, MonoRuntimeNotSupported]
        public static int InstanceSizeOf<T>() => *((int*)typeof(T).TypeHandle.Value + 1);

        /// <summary>
        /// Get a instance size of <paramref name="type"/>. dotnet (CoreCLR) only.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining), AssumeInputsValid, NoOverhead, MonoRuntimeNotSupported]
        public static int InstanceSizeOf([DisallowNull] Type type) => *((int*)type.TypeHandle.Value + 1);

        /// <summary>
        /// Get a instance size of object from <paramref name="typeHandle"/>. dotnet (CoreCLR) only.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining), AssumeInputsValid, NoOverhead, MonoRuntimeNotSupported]
        public static int InstanceSizeOf([DisallowNull, ValidRange] IntPtr typeHandle) => *((int*)typeHandle + 1);

        /// <summary>
        /// Allocates in unmanaged memory, using the memory layout of managed array.
        /// <br/>
        /// The current runtime is auto-detected. Avoid using this method if know fixed runtime information.
        /// </summary>
        /// <param name="length">It is not recommended to use this method if the size is 0 (zero).</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ArrayRef<T> GetUninitializedUnmanagedArray<T>(int length) =>
            JITPlatformEnvironment.IsRunningOnMono
            ? GetUninitializedUnmanagedArray_mono<T>(length)
            : GetUninitializedUnmanagedArray_dotnet<T>(length);

        /// <summary>
        /// Allocates in unmanaged memory, using the memory layout of dotnet (CoreCLR) managed array.
        /// </summary>
        /// <param name="length">It is not recommended to use this method if the size is 0 (zero).</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining), MonoRuntimeNotSupported]
        public static ArrayRef<T> GetUninitializedUnmanagedArray_dotnet<T>(int length)
        {
            var sz = (nint_t)length * Unsafe.SizeOf<T>() + sizeof(void*) * 3;

            var ptr = (IntPtr*)Marshal.AllocHGlobal((IntPtr)sz);

            ptr[0] = IntPtr.Zero;
            ptr[1] = typeof(T[]).TypeHandle.Value;
            ptr[2] = (IntPtr)(uint)length;

            return new ArrayRef<T>(ptr + 1, ptr + 2);
        }

        /// <summary>
        /// Allocates in unmanaged memory, using the memory layout of mono (MonoRuntime) managed array.
        /// </summary>
        /// <param name="length">It is not recommended to use this method if the size is 0 (zero).</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ArrayRef<T> GetUninitializedUnmanagedArray_mono<T>(int length)
        {
            var sz = (nint_t)length * Unsafe.SizeOf<T>() + sizeof(void*) * 4;

            var ptr = (IntPtr*)Marshal.AllocHGlobal((IntPtr)sz);

            ptr[0] = *(IntPtr*)ILUnsafe.AsPointer(Array.Empty<T>());
            ptr[1] = IntPtr.Zero;
            ptr[2] = IntPtr.Zero;
            ptr[3] = (IntPtr)(uint)length;

            return new ArrayRef<T>(ptr, ptr + 3);
        }

        /// <summary>
        /// Creates a managed object in unmanaged memory.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining), MonoRuntimeNotSupported]
        public static NativeInstance<T> Alloc<T>()
        {
            var type = typeof(T);
            if (type.IsArray)
                return default;

            var tHnd = type.TypeHandle.Value;

            var ptr = (IntPtr*)Marshal.AllocHGlobal(InstanceSizeOf(tHnd));

            ptr[0] = IntPtr.Zero;
            ptr[1] = tHnd;

            return new NativeInstance<T>(ptr);
        }

        /// <summary>
        /// Creates an initialized managed object in unmanaged memory.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining), MonoRuntimeNotSupported]
        public static NativeInstance<T> AllocZeroed<T>()
        {
            var type = typeof(T);
            if (type.IsArray)
                return default;

            var tHnd = type.TypeHandle.Value;

            var tSz = InstanceSizeOf(tHnd);

            var ptr = (IntPtr*)Marshal.AllocHGlobal(tSz);

            ptr[0] = IntPtr.Zero;
            ptr[1] = tHnd;

            Unsafe.InitBlockUnaligned(ptr + 2, 0, (uint)tSz);

            return new NativeInstance<T>(ptr);
        }

        /// <summary>
        /// Creates a uninitialized string in unmanaged memory.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining), AssumeInputsValid, MonoRuntimeNotSupported]
        public static StringRef GetUninitializedString([ValidRange] int length)
        {
            var tSz = (length << 1) + sizeof(void*) * 3 + sizeof(char);

            // align
            tSz = (tSz + (sizeof(void*) - 1)) & -sizeof(void*);

            var ptr = (IntPtr*)Marshal.AllocHGlobal(tSz);

            ptr[0] = IntPtr.Zero;
            ptr[1] = typeof(string).TypeHandle.Value;
            *(int*)(ptr + 2) = length;

            return new StringRef(ptr);
        }
    }
}
