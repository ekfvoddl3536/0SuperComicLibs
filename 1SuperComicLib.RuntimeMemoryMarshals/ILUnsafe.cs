// The MIT License (MIT)
//
// Copyright (c) 2023. SuperComic (ekfvoddl3535@naver.com)
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

#pragma warning disable CS1591
using System;
using System.Runtime.CompilerServices;
using System.Security;

namespace SuperComicLib.RuntimeMemoryMarshals
{
    /// <summary>
    /// Contains generic, low-level functionality for manipulating managed and unmanaged pointers,
    /// that assist '<see href="System.Runtime.CompilerServices.Unsafe"/>'
    /// <para/>
    /// Universal support for <see href="https://github.com/dotnet/runtime">CoreCLR (.NET Framework, .NET)</see> and <see href="https://github.com/mono/mono">Mono</see> runtimes.
    /// <para/>
    /// Due to frequent errors caused by the version of the <c>System.Runtime.CompilerServices.Unsafe</c> package,<br/>
    /// all features of <c>System.Runtime.CompilerServices.Unsafe 6.0.0</c> are incorporated here to completely eliminate dependencies on it.
    /// </summary>
    [SuppressUnmanagedCodeSecurity, SecurityCritical]
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
        /// Get the instance address of the managed class.
        /// <br/>
        /// <c>void* cls = (void*)value</c>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void* AsPointer(object value) => throw new PlatformNotSupportedException();
        /// <summary>
        /// Get the instance address of the managed class.
        /// <br/>
        /// <c>void* cls = (byte*)value + byteOffset</c>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void* AsPointer(object value, long byteOffset) => throw new PlatformNotSupportedException();

        /// <summary>
        /// Get a reference to the instance data.
        /// <br/>
        /// <c>ref TField cls = ref *(TField*)value.firstFieldData</c>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref TField GetDataRef<TField>(object class_reference) => throw new PlatformNotSupportedException();
        /// <summary>
        /// Get a reference to the instance data.
        /// <br/>
        /// <c>ref TField cls = ref *(TField*)((byte*)value.firstFieldData + byteOffset)</c>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref TField GetDataRef<TField>(object class_reference, long byteOffset) => throw new PlatformNotSupportedException();

        /// <summary>
        /// Get a pointer to the instance data.
        /// <br/>
        /// <c>TField* cls = (TField*)value.firstFieldData</c>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TField* GetDataPtr<TField>(object class_reference) where TField : unmanaged => throw new PlatformNotSupportedException();
        /// <summary>
        /// Get a pointer to the instance data.
        /// <br/>
        /// <c>TField* cls = (TField*)((byte*)value.firstFieldData + byteOffset)</c>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TField* GetDataPtr<TField>(object class_reference, long byteOffset) where TField : unmanaged => throw new PlatformNotSupportedException();

        /// <summary>
        /// Convert type
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref readonly TTo ReadOnlyAs<TFrom, TTo>(in TFrom source) => throw new PlatformNotSupportedException();

        /// <summary>
        /// It performs addition operations at the byte level and returns the result as a reference to <typeparamref name="T"/>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref T AddByteOffset<T>(void* source, int byteOffset) => throw new PlatformNotSupportedException();

        /// <summary>
        /// This method induces the `lea` instruction to perform the operation 
        /// `<c><paramref name="source"/> + <paramref name="elementOffset"/> * <see langword="sizeof"/>(<typeparamref name="T"/>) + <paramref name="addByteOffset"/></c>` 
        /// for three operands.
        /// <para/>
        /// If the size of <typeparamref name="T"/> is one of <see cref="int">1</see>, <see cref="int">2</see>,
        /// <see cref="int">4</see>, or <see cref="int">8</see>, there's a high likelihood that the `lea` instruction will be generated.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref T Add<T>(void* source, int elementOffset, int addByteOffset) => throw new PlatformNotSupportedException();

        /// <summary>
        /// This method induces the `lea` instruction to perform the operation 
        /// `<c><paramref name="source"/> + <paramref name="elementOffset"/> * <see langword="sizeof"/>(<typeparamref name="T"/>) + <paramref name="addByteOffset"/></c>` 
        /// for three operands.
        /// <para/>
        /// If the size of <typeparamref name="T"/> is one of <see cref="int">1</see>, <see cref="int">2</see>,
        /// <see cref="int">4</see>, or <see cref="int">8</see>, there's a high likelihood that the `lea` instruction will be generated.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref T Add<T>(void* source, long elementOffset, int addByteOffset) => throw new PlatformNotSupportedException();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref T Add<T>(void* source, int elementOffset) => throw new PlatformNotSupportedException();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref T Add<T>(void* source, long elementOffset) => throw new PlatformNotSupportedException();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref readonly T ReadOnlyAdd<T>(in T source, int elementOffset) => throw new PlatformNotSupportedException();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref readonly T ReadOnlyAdd<T>(in T source, long elementOffset) => throw new PlatformNotSupportedException();

        /// <summary>
        /// `<c>mov eax, cl</c>`
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ConvI4(bool value) => throw new PlatformNotSupportedException();

        //
        // !================== System.Runtime.CompilerServices.Unsafe (features) ==================!
        // 

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void* AsPointer<T>(ref T value)
        {
            throw new PlatformNotSupportedException();

            // ldarg.0
            // conv.u
            // ret
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SizeOf<T>()
        {
            throw new PlatformNotSupportedException();

            // sizeof !!0
            // ret
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T As<T>(object o) where T : class
        {
            throw new PlatformNotSupportedException();

            // ldarg.0
            // ret
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref TTo As<TFrom, TTo>(ref TFrom source)
        {
            throw new PlatformNotSupportedException();

            // ldarg.0
            // ret
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref T Add<T>(ref T source, int elementOffset)
        {
            throw new PlatformNotSupportedException();

            // ldarg .0
            // ldarg .1
            // sizeof !!0
            // conv.i
            // mul
            // add
            // ret
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref T Add<T>(ref T source, long elementOffset)
        {
            throw new PlatformNotSupportedException();

            // ldarg .0
            // ldarg .1
            // sizeof !!0
            // conv.i
            // mul
            // add
            // ret
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AreSame<T>(in T left, in T right)
        {
            throw new PlatformNotSupportedException();

            // ldarg.0
            // ldarg.1
            // ceq
            // ret
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Compare<T>(in T left, in T right)
        {
            throw new PlatformNotSupportedException();

            // ldarg.0
            // ldarg.1
            // sub
            // conv.i8
            // ret
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Copy<T>(void* destination, in T source)
        {
            throw new PlatformNotSupportedException();

            // ldarg .0
            // ldarg .1
            // ldobj !!0
            // stobj !!0
            // ret
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Copy<T>(ref T destination, void* source)
        {
            throw new PlatformNotSupportedException();

            // ldarg .0
            // ldarg .1
            // ldobj !!0
            // stobj !!0
            // ret
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyBlock(void* destination, void* source, uint byteCount)
        {
            throw new PlatformNotSupportedException();

            // ldarg .0
            // ldarg .1
            // ldarg .2
            // cpblk
            // ret
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyBlock(ref byte destination, in byte source, uint byteCount)
        {
            throw new PlatformNotSupportedException();

            // ldarg .0
            // ldarg .1
            // ldarg .2
            // cpblk
            // ret
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyBlockUnaligned(void* destination, void* source, uint byteCount)
        {
            throw new PlatformNotSupportedException();

            // ldarg .0
            // ldarg .1
            // ldarg .2
            // unaligned. 0x1
            // cpblk
            // ret
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyBlockUnaligned(ref byte destination, in byte source, uint byteCount)
        {
            throw new PlatformNotSupportedException();

            // ldarg .0
            // ldarg .1
            // ldarg .2
            // unaligned. 0x1
            // cpblk
            // ret
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void InitBlock(void* startAddress, byte value, uint byteCount)
        {
            throw new PlatformNotSupportedException();

            // ldarg .0
            // ldarg .1
            // ldarg .2
            // initblk
            // ret
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void InitBlock(ref byte startAddress, byte value, uint byteCount)
        {
            throw new PlatformNotSupportedException();

            // ldarg .0
            // ldarg .1
            // ldarg .2
            // initblk
            // ret
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void InitBlockUnaligned(void* startAddress, byte value, uint byteCount)
        {
            throw new PlatformNotSupportedException();

            // ldarg .0
            // ldarg .1
            // ldarg .2
            // unaligned. 0x1
            // initblk
            // ret
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void InitBlockUnaligned(ref byte startAddress, byte value, uint byteCount)
        {
            throw new PlatformNotSupportedException();

            // ldarg .0
            // ldarg .1
            // ldarg .2
            // unaligned. 0x1
            // initblk
            // ret
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T ReadUnaligned<T>(void* source)
        {
            throw new PlatformNotSupportedException();

            // ldarg.0
            // unaligned. 0x1
            // ldobj !!0
            // ret
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T ReadUnaligned<T>(in byte source)
        {
            throw new PlatformNotSupportedException();

            // ldarg.0
            // unaligned. 0x1
            // ldobj !!0
            // ret
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteUnaligned<T>(void* destination, T value)
        {
            throw new PlatformNotSupportedException();

            // ldarg .0
            // ldarg .1
            // unaligned. 0x1
            // stobj !!0
            // ret
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteUnaligned<T>(ref byte destination, T value)
        {
            throw new PlatformNotSupportedException();

            // ldarg .0
            // ldarg .1
            // unaligned. 0x1
            // stobj !!0
            // ret
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref T AddByteOffset<T>(ref T source, int byteOffset)
        {
            throw new PlatformNotSupportedException();

            // ldarg.0
            // ldarg.1
            // add
            // ret
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref T AddByteOffset<T>(ref T source, long byteOffset)
        {
            throw new PlatformNotSupportedException();

            // ldarg.0
            // ldarg.1
            // add
            // ret
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref T AsRef<T>(void* source)
        {
            throw new PlatformNotSupportedException();
            
            // ladrg.0
            // ret
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref T AsRef<T>(in T source)
        {
            throw new PlatformNotSupportedException();

            // ldarg.0
            // ret
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Read<T>(void* source)
        {
            throw new PlatformNotSupportedException();

            // ldarg.0
            // ldobj !!0
            // ret
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Write<T>(void* destination, T value)
        {
            throw new PlatformNotSupportedException();

            // ldarg.0
            // ldarg.1
            // stobj !!0
            // ret
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref T NullRef<T>()
        {
            throw new PlatformNotSupportedException();

            // ldc.i4.0
            // conv.u
            // ret
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNullRef<T>(in T source)
        {
            throw new PlatformNotSupportedException();

            // ldarg.0
            // ldc.i4.0
            // conv.u
            // ceq
            // ret
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SkipInit<T>(out T value)
        {
            throw new PlatformNotSupportedException();

            // ret
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref T Subtract<T>(ref T source, int elementOffset)
        {
            throw new PlatformNotSupportedException();

            // ldarg .0
            // ldarg .1
            // sizeof !!0
            // conv.i
            // mul
            // sub
            // ret
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref T Subtract<T>(ref T source, long elementOffset)
        {
            throw new PlatformNotSupportedException();

            // ldarg .0
            // ldarg .1
            // sizeof !!0
            // mul
            // sub
            // ret
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref T SubtractByteOffset<T>(ref T source, long byteOffset)
        {
            throw new PlatformNotSupportedException();

            // ldarg .0
            // ldarg .1
            // sub
            // ret
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref T Unbox<T>(object box) where T : struct
        {
            throw new PlatformNotSupportedException();

            // ldarg .0
            // unbox !!0
            // ret
        }
    }
}
