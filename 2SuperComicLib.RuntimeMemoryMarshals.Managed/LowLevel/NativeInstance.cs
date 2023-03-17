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
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using SuperComicLib.CodeContracts;

namespace SuperComicLib.RuntimeMemoryMarshals
{
    /// <summary>
    /// Provides access to managed class instances allocated in unmanaged memory.
    /// </summary>
    [StructLayout(LayoutKind.Sequential), MonoRuntimeNotSupported]
    public readonly unsafe struct NativeInstance<T> : IDisposable
    {
        internal readonly byte* _Ptr;

        #region constructor
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal NativeInstance(IntPtr* _Ptr) => this._Ptr = (byte*)_Ptr;
        #endregion

        #region property
        /// <summary>
        /// Read runtime value information for <typeparamref name="T"/>.
        /// <para/>
        /// For some scenarios where the type of T, cannot be inferred at compile time.
        /// </summary>
        public RuntimeTypedValueInfo<T> RuntimeValueInfo
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                DEBUG_NULL_CHECK(this);

                return new RuntimeTypedValueInfo<T>(_Ptr);
            }
        }

        /// <summary>
        /// Same as: <see cref="Type.TypeHandle"/>.<see cref="RuntimeTypeHandle.Value">Value</see>
        /// </summary>
        public IntPtr TypeHandle
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining), AssumeOperationValid]
            get => *(IntPtr*)(_Ptr + sizeof(void*));
        }

        /// <summary>
        /// Get the instance size. This size is the sum of the space occupied by the field members in memory.
        /// </summary>
        public int Size
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining), AssumeOperationValid]
            get => NativeClass.InstanceSizeOf(TypeHandle) - (sizeof(void*) << 1);
        }
        #endregion

        #region get data reference
        /// <summary>
        /// Gets a field area memory reference at the specified byte offset.
        /// <br/>
        /// It does not perform validation, such as range checking. API for high-performance scenarios.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining), AssumeOperationValid, AssumeInputsValid]
        public ref byte GetDataReference([ValidRange] int byteOffset)
        {
            DEBUG_NULL_CHECK(this);

            return ref _Ptr[byteOffset + (sizeof(void*) << 1)];
        }

        /// <summary>
        /// Gets a field area memory reference at the specified byte offset.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining), AssumeOperationValid]
        public ref byte GetDataReference_at(int byteOffset)
        {
            DEBUG_NULL_CHECK(this);

            if ((uint)byteOffset >= (uint)Size)
                throw new ArgumentOutOfRangeException(nameof(byteOffset));

            return ref GetDataReference(byteOffset);
        }

        /// <summary>
        /// Gets a field area memory reference.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining), AssumeOperationValid]
        public NativeSpan<byte> GetDataReferenceAsSpan()
        {
            DEBUG_NULL_CHECK(this);

            var tmp = _Ptr + sizeof(void*);
            return new NativeSpan<byte>(tmp + sizeof(void*), NativeClass.InstanceSizeOf(*(IntPtr*)tmp));
        }
        #endregion

        #region get data pointer
        /// <summary>
        /// Gets a field area memory pointer at the specified byte offset.
        /// <br/>
        /// It does not perform validation, such as range checking. API for high-performance scenarios.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining), AssumeOperationValid, AssumeInputsValid]
        public TUnmanaged* GetDataPointer<TUnmanaged>([ValidRange] int byteOffset) where TUnmanaged : unmanaged
        {
            DEBUG_NULL_CHECK(this);

            return (TUnmanaged*)(_Ptr + byteOffset + (sizeof(void*) << 1));
        }

        /// <summary>
        /// Gets a field area memory pointer at the specified byte offset.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining), AssumeOperationValid]
        public TUnmanaged* GetDataPointer_at<TUnmanaged>(int byteOffset) where TUnmanaged : unmanaged =>
            (uint)byteOffset >= (uint)Size
            ? throw new ArgumentOutOfRangeException(nameof(byteOffset))
            : GetDataPointer<TUnmanaged>(byteOffset);
        #endregion

        #region copyto
        #region unmanaged (native instance)
        /// <summary>
        /// Copies field data to <paramref name="other"/>.
        /// </summary>
        /// <returns><see langword="true"/>, if the operation was successful.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining), AssumeOperationValid]
        public bool CopyTo([AllowNull] NativeInstance<T> other)
        {
            if (other._Ptr == null)
                return false;

            CopyTo_unsafe(other);
            return true;
        }

        /// <summary>
        /// Copies field data to <paramref name="other"/>.
        /// <br/>
        /// It does not perform validation, such as range checking. API for high-performance scenarios.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining), AssumeOperationValid, AssumeInputsValid]
        public void CopyTo_unsafe([DisallowNull] NativeInstance<T> other)
        {
            DEBUG_NULL_CHECK(this);
            DEBUG_NULL_CHECK(other);

            ulong sz = (uint)Size;
            Buffer.MemoryCopy(_Ptr + (sizeof(void*) << 1), other._Ptr + (sizeof(void*) << 1), sz, sz);
        }
        #endregion

        #region managed (byte[])
        /// <summary>
        /// Copies field data to destination <paramref name="array"/>.
        /// <br/>
        /// <see cref="System.Collections.Generic.List{T}.CopyTo(T[])"/>
        /// </summary>
        /// <returns>The number of bytes copied.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining), AssumeOperationValid]
        public int CopyTo([AllowNull] byte[] array) =>
            CopyTo(0, array, 0, array.Length);

        /// <summary>
        /// Copies field data to destination <paramref name="array"/>.
        /// <br/>
        /// <see cref="System.Collections.Generic.List{T}.CopyTo(T[], int)"/>
        /// </summary>
        /// <returns>The number of bytes copied.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining), AssumeOperationValid]
        public int CopyTo([AllowNull] byte[] array, int arrayIndex) =>
            CopyTo(0, array, arrayIndex, array.Length - arrayIndex);

        /// <summary>
        /// Copies field data to destination <paramref name="array"/>.
        /// <br/>
        /// <see cref="System.Collections.Generic.List{T}.CopyTo(int, T[], int, int)"/>
        /// </summary>
        /// <returns>The number of bytes copied.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining), AssumeOperationValid, NoOverhead]
        public int CopyTo(int index, [AllowNull] byte[] array, int arrayIndex, int count)
        {
            DEBUG_NULL_CHECK(this);

            if (array == null || (arrayIndex | count) < 0)
                return 0;

            int sz = Size;
            if ((uint)index >= (uint)sz)
                return 0;

            sz = (int)CMath.Min((uint)(sz - index), (uint)(array.Length - arrayIndex)).Min((uint)count);
            Marshal.Copy((IntPtr)(_Ptr + index + (sizeof(void*) << 1)), array, arrayIndex, sz);
            return sz;
        }

        /// <summary>
        /// Copies field data to destination <paramref name="array"/>.
        /// <br/>
        /// <see cref="System.Collections.Generic.List{T}.CopyTo(int, T[], int, int)"/>
        /// <br/>
        /// but, it does not perform validation, such as range checking. API for high-performance scenarios.
        /// </summary>
        /// <returns>The number of bytes copied.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining), AssumeOperationValid, AssumeInputsValid]
        public void CopyTo_unsafe([ValidRange] int index, [DisallowNull] byte[] array, [ValidRange] int arrayIndex, [ValidRange] int count)
        {
            DEBUG_NULL_CHECK(this);

            Marshal.Copy((IntPtr)(_Ptr + index + (sizeof(void*) << 1)), array, arrayIndex, count);
        }
        #endregion
        #endregion

        #region clear
        /// <summary>
        /// Clear the instance data to 0 (zero).
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining), AssumeOperationValid]
        public void Clear()
        {
            DEBUG_NULL_CHECK(this);

            Unsafe.InitBlockUnaligned(_Ptr + (sizeof(void*) << 1), 0, (uint)Size);
        }
        #endregion

        #region override
        public override bool Equals(object obj) => obj is NativeInstance<T> other && _Ptr == other._Ptr;
        public override int GetHashCode() => ((IntPtr)_Ptr).GetHashCode();
        #endregion

        #region dispose
        /// <summary>
        /// Frees unmanaged memory.
        /// <see cref="IDisposable.Dispose"/>.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining), AssumeOperationValid]
        public void Dispose() => Marshal.FreeHGlobal((IntPtr)_Ptr);
        #endregion

        #region operator
        /// <summary>
        /// Compares two <see cref="NativeInstance{T}"/> for equality.
        /// <br/>
        /// Shallow comparison. Performing only address comparisons.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(NativeInstance<T> left, NativeInstance<T> right) => left._Ptr == right._Ptr;
        /// <summary>
        /// Compares two <see cref="NativeInstance{T}"/> for not equality.
        /// <br/>
        /// Shallow comparison. Performing only address comparisons.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(NativeInstance<T> left, NativeInstance<T> right) => left._Ptr != right._Ptr;
        #endregion

        #region helper
        [Conditional("DEBUG")]
        internal static void DEBUG_NULL_CHECK(in NativeInstance<T> value)
        {
            if (value._Ptr == null)
                throw new NullReferenceException(nameof(value));
        }
        #endregion
    }
}
