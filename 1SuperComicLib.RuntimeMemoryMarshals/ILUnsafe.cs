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
        #region Group 1. (CUSTOM, +object, 'readonly' operation)
        #region AsClass(+object), AsPointer(+object), GetDataRef(+object), GetDataPtr(+object)
        /// <summary>
        /// Convert a pointer address to a managed class.
        /// <br/>
        /// <c>(<typeparamref name="T"/>)<paramref name="source"/></c>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T AsClass<T>(void* source) where T : class => throw new PlatformNotSupportedException();

        /// <summary>
        /// Convert a pointer address to a managed class.
        /// <br/>
        /// <c>(<typeparamref name="T"/>)<paramref name="source"/></c>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T AsClass<T>(IntPtr source) where T : class => throw new PlatformNotSupportedException();

        /// <summary>
        /// Convert a reference address to a managed class.
        /// <br/>
        /// <c>(<typeparamref name="TTo"/>)(<see langword="ref"/> <paramref name="source"/>)</c>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TTo AsClass<TFrom, TTo>(ref TFrom source) where TTo : class => throw new PlatformNotSupportedException();

        /// <summary>
        /// Get the instance address of the managed class.
        /// <br/>
        /// <c>(<see langword="void"/>*)<paramref name="reference"/></c>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void* AsPointer(object reference) => throw new PlatformNotSupportedException();
        /// <summary>
        /// Get the instance address of the managed class.
        /// <br/>
        /// <c>(<see langword="byte"/>*)<paramref name="reference"/> + <paramref name="byteOffset"/></c>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void* AsPointer(object reference, long byteOffset) => throw new PlatformNotSupportedException();

        /// <summary>
        /// Get a reference to the instance data.
        /// <br/>
        /// <c>(<see langword="ref"/> <typeparamref name="TField"/>)(<paramref name="reference"/>.firstField)</c>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref TField GetDataRef<TField>(object reference) => throw new PlatformNotSupportedException();
        /// <summary>
        /// Get a reference to the instance data.
        /// <br/>
        /// <c>(<see langword="ref"/> <typeparamref name="TField"/>)((<see langword="byte"/>*)<paramref name="reference"/>.firstField + <paramref name="byteOffset"/>)</c>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref TField GetDataRef<TField>(object reference, long byteOffset) => throw new PlatformNotSupportedException();

        /// <summary>
        /// Get a pointer to the instance data.
        /// <br/>
        /// <c>(<typeparamref name="TField"/>*)(<paramref name="reference"/>.firstField)</c>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TField* GetDataPtr<TField>(object reference) where TField : unmanaged => throw new PlatformNotSupportedException();
        /// <summary>
        /// Get a pointer to the instance data.
        /// <br/>
        /// <c>(<typeparamref name="TField"/>*)((<see langword="byte"/>*)<paramref name="reference"/>.firstField + <paramref name="byteOffset"/>)</c>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TField* GetDataPtr<TField>(object reference, long byteOffset) where TField : unmanaged => throw new PlatformNotSupportedException();
        #endregion

        #region 'readonly' As, Add, Subtract
        #region As
        /// <summary>
        /// Convert type
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref readonly TTo ReadOnlyAs<TFrom, TTo>(in TFrom source) => throw new PlatformNotSupportedException();
        #endregion

        #region Add
        /// <summary>
        /// <c>(<typeparamref name="T"/>*)(<see langword="ref"/> <paramref name="source"/>) + <paramref name="offset"/></c>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref readonly T ReadOnlyAdd<T>(in T source, int offset) => throw new PlatformNotSupportedException();

        /// <summary>
        /// <c>(<typeparamref name="T"/>*)(<see langword="ref"/> <paramref name="source"/>) + <paramref name="offset"/></c>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref readonly T ReadOnlyAdd<T>(in T source, long offset) => throw new PlatformNotSupportedException();

        /// <summary>
        /// <c>(<typeparamref name="TFrom"/>*)(<see langword="ref"/> <paramref name="source"/>) + <paramref name="offset"/></c>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref readonly TTo ReadOnlyAdd<TFrom, TTo>(in TFrom source, long offset) => throw new PlatformNotSupportedException();

        /// <summary>
        /// <c>(<see langword="byte"/>*)((<typeparamref name="TFrom"/>*)(<see langword="ref"/> <paramref name="source"/>) + <paramref name="offset"/>) + <paramref name="displacement"/></c>
        /// <para/>
        /// <c>LEA rax, [<paramref name="source"/> + <paramref name="offset"/> * <see langword="sizeof"/>(<typeparamref name="TFrom"/>) + <paramref name="displacement"/>]</c>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref readonly TTo ReadOnlyAdd<TFrom, TTo>(in TFrom source, long offset, int displacement) => throw new PlatformNotSupportedException();

        /// <summary>
        /// <c>(<typeparamref name="TSize"/>*)(<see langword="ref"/> <paramref name="source"/>) + <paramref name="offset"/></c>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref readonly TTo ReadOnlyAdd<TFrom, TSize, TTo>(in TFrom source, long offset) => throw new PlatformNotSupportedException();

        /// <summary>
        /// <c>(<see langword="byte"/>*)((<typeparamref name="TSize"/>*)(<see langword="ref"/> <paramref name="source"/>) + <paramref name="offset"/>) + <paramref name="displacement"/></c>
        /// <para/>
        /// <c>LEA rax, [<paramref name="source"/> + <paramref name="offset"/> * <see langword="sizeof"/>(<typeparamref name="TSize"/>) + <paramref name="displacement"/>]</c>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref readonly TTo ReadOnlyAdd<TFrom, TSize, TTo>(in TFrom source, long offset, int displacement) => throw new PlatformNotSupportedException();
        #endregion

        #region Subtract
        /// <summary>
        /// <c>(<typeparamref name="T"/>*)(<see langword="ref"/> <paramref name="source"/>) - <paramref name="offset"/></c>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref readonly T ReadOnlySubtract<T>(in T source, int offset) => throw new PlatformNotSupportedException();

        /// <summary>
        /// <c>(<typeparamref name="T"/>*)(<see langword="ref"/> <paramref name="source"/>) - <paramref name="offset"/></c>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref readonly T ReadOnlySubtract<T>(in T source, long offset) => throw new PlatformNotSupportedException();

        /// <summary>
        /// <c>(<typeparamref name="TFrom"/>*)(<see langword="ref"/> <paramref name="source"/>) - <paramref name="offset"/></c>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref readonly TTo ReadOnlySubtract<TFrom, TTo>(in TFrom source, long offset) => throw new PlatformNotSupportedException();

        /// <summary>
        /// <c>(<typeparamref name="TSize"/>*)(<see langword="ref"/> <paramref name="source"/>) - <paramref name="offset"/></c>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref readonly TTo ReadOnlySubtract<TFrom, TSize, TTo>(in TFrom source, long offset) => throw new PlatformNotSupportedException();
        #endregion
        #endregion
        #endregion

        #region Group 2. (CUSTOM, void*, bool->int)
        /// <summary>
        /// It performs addition operations at the byte level and returns the result as a reference to <typeparamref name="T"/>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref T AddByteOffset<T>(void* source, long byteOffset) => throw new PlatformNotSupportedException();

        #region Add
        /// <summary>
        /// This method induces the `lea` instruction to perform the operation 
        /// `<c><paramref name="source"/> + <paramref name="offset"/> * <see langword="sizeof"/>(<typeparamref name="T"/>) + <paramref name="displacement"/></c>` 
        /// for three operands.
        /// <para/>
        /// If the size of <typeparamref name="T"/> is one of <see cref="int">1</see>, <see cref="int">2</see>,
        /// <see cref="int">4</see>, or <see cref="int">8</see>, there's a high likelihood that the `lea` instruction will be generated.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref T Add<T>(void* source, int offset, int displacement) => throw new PlatformNotSupportedException();

        /// <summary>
        /// This method induces the `lea` instruction to perform the operation 
        /// `<c><paramref name="source"/> + <paramref name="offset"/> * <see langword="sizeof"/>(<typeparamref name="T"/>) + <paramref name="displacement"/></c>` 
        /// for three operands.
        /// <para/>
        /// If the size of <typeparamref name="T"/> is one of <see cref="int">1</see>, <see cref="int">2</see>,
        /// <see cref="int">4</see>, or <see cref="int">8</see>, there's a high likelihood that the `lea` instruction will be generated.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref T Add<T>(void* source, long offset, int displacement) => throw new PlatformNotSupportedException();

        /// <summary>
        /// <c>(<typeparamref name="T"/>*)<paramref name="source"/> + <paramref name="offset"/></c>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref T Add<T>(void* source, int offset) => throw new PlatformNotSupportedException();

        /// <summary>
        /// <c>(<typeparamref name="T"/>*)<paramref name="source"/> + <paramref name="offset"/></c>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref T Add<T>(void* source, long offset) => throw new PlatformNotSupportedException();
        #endregion

        #region Subtract
        /// <summary>
        /// <c>(<typeparamref name="T"/>*)<paramref name="source"/> - <paramref name="offset"/></c>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref T Subtract<T>(void* source, int offset) => throw new PlatformNotSupportedException();

        /// <summary>
        /// <c>(<typeparamref name="T"/>*)<paramref name="source"/> - <paramref name="offset"/></c>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref T Subtract<T>(void* source, long offset) => throw new PlatformNotSupportedException();
        #endregion

        /// <summary>
        /// `<c>mov eax, cl</c>`
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ConvI4(bool value) => throw new PlatformNotSupportedException();
        #endregion

        #region Group 3. (CUSTOM, +Increment, +Decrement)
        #region increment
        /// <summary>
        /// <c>(<typeparamref name="T"/>*)(<see langword="ref"/> <paramref name="source"/>) + 1</c>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref T Increment<T>(ref T source) => throw new PlatformNotSupportedException();

        /// <summary>
        /// <c>(<typeparamref name="TSize"/>*)(<see langword="ref"/> <paramref name="source"/>) + 1</c>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref TFrom Increment<TFrom, TSize>(ref TFrom source) => throw new PlatformNotSupportedException();

        /// <summary>
        /// <c>(<typeparamref name="TSize"/>*)(<see langword="ref"/> <paramref name="source"/>) + 1</c>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref TTo Increment<TFrom, TSize, TTo>(ref TFrom source) => throw new PlatformNotSupportedException();
        #endregion

        #region decrement
        /// <summary>
        /// <c>(<typeparamref name="T"/>*)(<see langword="ref"/> <paramref name="source"/>) - 1</c>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref T Decrement<T>(ref T source) => throw new PlatformNotSupportedException();

        /// <summary>
        /// <c>(<typeparamref name="TSize"/>*)(<see langword="ref"/> <paramref name="source"/>) - 1</c>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref TFrom Decrement<TFrom, TSize>(ref TFrom source) => throw new PlatformNotSupportedException();

        /// <summary>
        /// <c>(<typeparamref name="TSize"/>*)(<see langword="ref"/> <paramref name="source"/>) - 1</c>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref TTo Decrement<TFrom, TSize, TTo>(ref TFrom source) => throw new PlatformNotSupportedException();
        #endregion
        #endregion

        #region Group 4. (CUSTOM, +refpoint<>)
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref refpoint<T> AsRefpoint<T>(void* source) => throw new PlatformNotSupportedException();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref refpoint<T> AsRefpoint<T>(ref T source) => throw new PlatformNotSupportedException();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref refpoint<TTo> AsRefpoint<TFrom, TTo>(ref TFrom source) => throw new PlatformNotSupportedException();
        #endregion

        #region Group 5. (EXTEND, +Add, +Subtract, +AsRef)
        #region Add
        /// <summary>
        /// <c>(<typeparamref name="TFrom"/>*)(<see langword="ref"/> <paramref name="source"/>) + <paramref name="offset"/></c>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref TTo Add<TFrom, TTo>(ref TFrom source, long offset) => throw new PlatformNotSupportedException();

        /// <summary>
        /// <c>(<see langword="byte"/>*)((<typeparamref name="TFrom"/>*)(<see langword="ref"/> <paramref name="source"/>) + <paramref name="offset"/>) + <paramref name="displacement"/></c>
        /// <para/>
        /// <c>LEA rax, [<paramref name="source"/> + <paramref name="offset"/> * <see langword="sizeof"/>(<typeparamref name="TFrom"/>) + <paramref name="displacement"/>]</c>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref TTo Add<TFrom, TTo>(ref TFrom source, long offset, int displacement) => throw new PlatformNotSupportedException();

        /// <summary>
        /// <c>(<typeparamref name="TSize"/>*)(<see langword="ref"/> <paramref name="source"/>) + <paramref name="offset"/></c>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref TTo Add<TFrom, TSize, TTo>(ref TFrom source, long offset) => throw new PlatformNotSupportedException();

        /// <summary>
        /// <c>(<see langword="byte"/>*)((<typeparamref name="TSize"/>*)(<see langword="ref"/> <paramref name="source"/>) + <paramref name="offset"/>) + <paramref name="displacement"/></c>
        /// <para/>
        /// <c>LEA rax, [<paramref name="source"/> + <paramref name="offset"/> * <see langword="sizeof"/>(<typeparamref name="TSize"/>) + <paramref name="displacement"/>]</c>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref TTo Add<TFrom, TSize, TTo>(ref TFrom source, long offset, int displacement) => throw new PlatformNotSupportedException();

        /// <summary>
        /// <c>(<typeparamref name="T"/>*)(<see langword="ref"/> <paramref name="source"/>) + <paramref name="offset"/></c>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref T Add<T>(ref T source, ulong offset) => throw new PlatformNotSupportedException();

        /// <summary>
        /// <c>(<see langword="byte"/>*)((<typeparamref name="T"/>*)(<see langword="ref"/> <paramref name="source"/>) + <paramref name="offset"/>) + <paramref name="displacement"/></c>
        /// <para/>
        /// <c>LEA rax, [<paramref name="source"/> + <paramref name="offset"/> * <see langword="sizeof"/>(<typeparamref name="T"/>) + <paramref name="displacement"/>]</c>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref T Add<T>(ref T source, ulong offset, int displacement) => throw new PlatformNotSupportedException();

        /// <summary>
        /// <c>(<see langword="byte"/>*)((<typeparamref name="T"/>*)(<see langword="ref"/> <paramref name="source"/>) + <paramref name="offset"/>) + <paramref name="displacement"/></c>
        /// <para/>
        /// <c>LEA rax, [<paramref name="source"/> + <paramref name="offset"/> * <see langword="sizeof"/>(<typeparamref name="T"/>) + <paramref name="displacement"/>]</c>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref T Add<T>(ref T source, long offset, int displacement) => throw new PlatformNotSupportedException();
        #endregion

        #region Subtract
        /// <summary>
        /// <c>(<typeparamref name="TFrom"/>*)(<see langword="ref"/> <paramref name="source"/>) - <paramref name="offset"/></c>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref TTo Subtract<TFrom, TTo>(ref TFrom source, long offset) => throw new PlatformNotSupportedException();

        /// <summary>
        /// <c>(<typeparamref name="TSize"/>*)(<see langword="ref"/> <paramref name="source"/>) - <paramref name="offset"/></c>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref TTo Subtract<TFrom, TSize, TTo>(ref TFrom source, long offset) => throw new PlatformNotSupportedException();
        #endregion

        #region AsRef
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref TTo AsRef<TFrom, TTo>(in TFrom source) => throw new PlatformNotSupportedException();
        #endregion
        #endregion

        #region Group X. (System.Runtime.CompilerServices.Unsafe)
        //
        // !================== System.Runtime.CompilerServices.Unsafe (features) ==================!
        //
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void* AsPointer<T>(ref T source)
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
        public static long ByteOffset<T>(in T origin, in T target)
        {
            throw new PlatformNotSupportedException();

            // ldarg.0
            // ldarg.1
            // sub
            // conv.i8
            // ret
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsAddressGreaterThan<T>(in T left, in T right)
        {
            throw new PlatformNotSupportedException();

            // ldarg.0
            // ldarg.1
            // cgt.un
            // ret
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsAddressLessThan<T>(in T left, in T right)
        {
            throw new PlatformNotSupportedException();

            // ldarg.0
            // ldarg.1
            // clt.un
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
        public static void SkipInit<T>(out T source)
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
        #endregion
    }
}
