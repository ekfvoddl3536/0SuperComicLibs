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

#pragma warning disable IDE1006
#pragma warning disable CS1591
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using SuperComicLib.CodeContracts;

namespace SuperComicLib.RuntimeMemoryMarshals
{
    /// <summary>
    /// Provides a reference to a managed array allocated in unmanaged memory.
    /// <br/>
    /// Supports both unmanaged and managed array scenarios simultaneously.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public readonly unsafe struct ArrayRef<T> : IEquatable<ArrayRef<T>>, IDisposable
    {
        internal readonly byte* _pClass;
        internal readonly byte* _pLength;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal ArrayRef(IntPtr* pClass, IntPtr* pLength)
        {
            _pClass = (byte*)pClass;
            _pLength = (byte*)pLength;
        }

        #region property
        /// <summary>
        /// Gets whether this array is a dotnet (CoreCLR) array.
        /// <br/>
        /// Used to identify arrays allocated using MonoRuntime's array memory layout.
        /// </summary>
        public bool IsDotnetArray
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (IntPtr)(_pLength - _pClass) == (IntPtr)sizeof(void*);
        }

        /// <summary>
        /// Same as: <see cref="Array.Length"/>
        /// </summary>
        public int Length
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining), AssumeOperationValid]
            get => *(int*)_pLength;
        }

        /// <summary>
        /// Same as: <see cref="Array.LongLength"/>
        /// </summary>
        public long LongLength
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining), AssumeOperationValid, X64Only]
            get => *(long*)_pLength;
        }
        #endregion

        #region indexer
        /// <summary>
        /// Same as: <see cref="P:System.Collections.ArrayList.Item(System.Int32)"/>.
        /// but, it does not perform validation, such as range checking. API for high-performance scenarios.
        /// </summary>
        public ref T this[[ValidRange] int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining), AssumeOperationValid, AssumeInputsValid]
            get => ref ILUnsafe.Add<T>(_pLength, index, sizeof(void*));
        }

        /// <summary>
        /// Same as: <see cref="P:System.Collections.ArrayList.Item(System.Int32)"/>.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining), AssumeOperationValid]
        public ref T at(int index)
        {
            if ((uint)index >= *(uint*)_pLength)
                throw new ArgumentOutOfRangeException(nameof(index));

            return ref this[index];
        }
        #endregion

        #region methods
        /// <summary>
        /// Gets a reference to a managed array allocated in unmanaged memory.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining), AssumeOperationValid]
        public T[] AsManaged() => ILUnsafe.AsClass<T[]>(_pClass);

        /// <summary>
        /// Gets a reference to a managed array allocated in unmanaged memory.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining), AssumeOperationValid]
        public Memory<T> AsMemory() => new Memory<T>(ILUnsafe.AsClass<T[]>(_pClass));

        /// <summary>
        /// Create a copy.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining), AssumeOperationValid]
        public ArrayRef<T> Clone()
        {
            int len = Length;

            ArrayRef<T> dst =
                IsDotnetArray
                ? NativeClass.GetUninitializedUnmanagedArray_dotnet<T>(len)
                : NativeClass.GetUninitializedUnmanagedArray_mono<T>(len);

            ulong sz = (uint)(len * Unsafe.SizeOf<T>());
            Buffer.MemoryCopy(GetDataPointer(), dst.GetDataPointer(), sz, sz);

            return dst;
        }

        /// <summary>
        /// Gets the data starting address of this array instance.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte* GetDataPointer() => _pLength + sizeof(void*);

        /// <summary>
        /// Gets the data starting address of this array instance.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeSpan<byte> GetDataReferenceAsSpan()
        {
            var ptr = _pLength + sizeof(void*);
            var sz = *(int*)_pLength * Unsafe.SizeOf<T>();

            return new NativeSpan<byte>(ptr, sz);
        }
        #endregion

        #region dispose
        /// <summary>
        /// Release this unmanaged instance.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining), AssumeOperationValid]
        public void Dispose()
        {
            if (IsDotnetArray)
                // dotnet
                Marshal.FreeHGlobal((IntPtr)(_pClass - sizeof(void*)));
            else
                // mono
                Marshal.FreeHGlobal((IntPtr)_pClass);
        }
        #endregion

        #region override
        bool IEquatable<ArrayRef<T>>.Equals(ArrayRef<T> other) => this == other;

        public override bool Equals(object obj) => obj is ArrayRef<T> other && this == other;
        public override int GetHashCode() => ((nint_t)_pClass ^ (nint_t)_pLength).GetHashCode();
        #endregion

        #region operator
        /// <summary>
        /// Shallow comparison. Performing only address comparisons.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(in ArrayRef<T> left, in ArrayRef<T> right) => (((nint_t)left._pClass ^ (nint_t)right._pClass) | ((nint_t)left._pLength ^ (nint_t)right._pLength)) == 0;
        /// <summary>
        /// Shallow comparison. Performing only address comparisons.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(in ArrayRef<T> left, in ArrayRef<T> right) => !(left == right);
        #endregion
    }
}
