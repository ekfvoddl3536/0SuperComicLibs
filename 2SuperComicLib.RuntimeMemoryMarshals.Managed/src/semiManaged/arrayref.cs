// MIT License
//
// Copyright (c) 2019-2023. SuperComic (ekfvoddl3535@naver.com)
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

using System;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
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
    public readonly unsafe partial struct arrayref<T> : IEquatable<arrayref<T>>, IDisposable
    {
        internal readonly byte* _pClass;
        internal readonly byte* _pLength;

        #region constructor
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal arrayref(void* pClass, void* pLength)
        {
            _pClass = (byte*)pClass;
            _pLength = (byte*)pLength;
        }

        /// <summary>
        /// Allocates in unmanaged memory, using the memory layout of managed array.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="length"/> is negative.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public arrayref(int length)
        {
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length));

            this = newf(length);
        }

        /// <summary>
        /// Creates a <see cref="arrayref{T}"/> using another unmanaged memory area.<br/>
        /// Since the object occupies external memory, never call <see cref=" Dispose"/>.<para/>
        /// The <see cref="Length"/> is automatically calculated as the number of bytes in the provided.
        /// </summary>
        /// <param name="unmanagedMemory">It takes `3 (clr) or 4 (mono)` * IntPtr.Size bytes to write the header.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public arrayref(in NativeSpan<byte> unmanagedMemory)
        {
            if (JITPlatformEnvironment.IsRunningOnMono)
                this = mono_header((IntPtr*)unmanagedMemory.Source, unmanagedMemory.Length);
            else
                this = clr_header((IntPtr*)unmanagedMemory.Source, unmanagedMemory.Length);
        }
        #endregion

        #region property +size()
        /// <summary>
        /// Gets whether this array is a CoreCLR (.NET Framework, .NET) array.
        /// <br/>
        /// Used to identify arrays allocated using MonoRuntime's array memory layout.
        /// </summary>
        public bool IsCLRArray
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _pLength - _pClass == sizeof(long);
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
            [MethodImpl(MethodImplOptions.AggressiveInlining), AssumeOperationValid]
            get => *(long*)_pLength;
        }

        public bool IsNull
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _pClass == null;
        }
        #endregion

        #region indexer
        /// <summary>
        /// Same as: <see cref="System.Collections.Generic.List{T}.this[int]"/>.
        /// but, it does not perform validation, such as range checking. API for high-performance scenarios.
        /// </summary>
        public ref T this[[ValidRange] int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining), AssumeOperationValid, AssumeInputsValid]
            get => ref ILUnsafe.Add<T>(_pLength, index, sizeof(long));
        }

        /// <summary>
        /// Same as: <see cref="System.Collections.Generic.List{T}.this[int]"/>.
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
        public arrayref<T> Clone()
        {
            int len = Length;

            arrayref<T> dst =
                IsCLRArray
                ? newf_clr(len)
                : newf_mono(len);

            ulong sz = (uint)(len * ILUnsafe.SizeOf<T>());
            Buffer.MemoryCopy(GetDataPointer(), dst.GetDataPointer(), sz, sz);

            return dst;
        }

        /// <summary>
        /// Gets the data starting address of this array instance.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte* GetDataPointer() => _pLength + sizeof(long);

        /// <summary>
        /// Gets the data starting address of this array instance.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeSpan<byte> GetDataReferenceAsSpan()
        {
            var ptr = _pLength + sizeof(long);
            var sz = *(int*)_pLength * ILUnsafe.SizeOf<T>();

            return new NativeSpan<byte>(ptr, sz);
        }

        /// <summary>
        /// Gets the data starting address of this array instance.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeSpan<TTo> GetDataReferenceAsSpan<TTo>() where TTo : unmanaged
        {
            var ptr = _pLength + sizeof(long);
            var sz = *(int*)_pLength * ILUnsafe.SizeOf<T>() / sizeof(TTo);

            return new NativeSpan<TTo>((TTo*)ptr, sz);
        }
        #endregion

        #region dispose
        /// <summary>
        /// Release this unmanaged instance.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining), AssumeOperationValid]
        public void Dispose()
        {
            if (IsCLRArray)
                // clr
                Marshal.FreeHGlobal((IntPtr)(_pClass - sizeof(long)));
            else
                // mono
                Marshal.FreeHGlobal((IntPtr)_pClass);
        }
        #endregion

        #region override
        bool IEquatable<arrayref<T>>.Equals(arrayref<T> other) => this == other;

        public override bool Equals(object obj) => obj is arrayref<T> other && this == other;
        public override int GetHashCode() => ((long)_pClass ^ (long)_pLength).GetHashCode();
        #endregion

        #region operator
        /// <summary>
        /// Shallow comparison. Performing only address comparisons.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(in arrayref<T> left, in arrayref<T> right) => left._pClass == right._pClass;
        /// <summary>
        /// Shallow comparison. Performing only address comparisons.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(in arrayref<T> left, in arrayref<T> right) => left._pClass != right._pClass;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(in arrayref<T> left, T[] right) => left._pClass == ILUnsafe.AsPointer(right);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(in arrayref<T> left, T[] right) => left._pClass != ILUnsafe.AsPointer(right);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(T[] left, in arrayref<T> right) => ILUnsafe.AsPointer(left) == right._pClass;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(T[] left, in arrayref<T> right) => ILUnsafe.AsPointer(left) != right._pClass;
        #endregion

        #region new arrayref
        /// <summary>
        /// new fast. No argument validation. No zero-init.
        /// <br/>
        /// Allocates in unmanaged memory, using the memory layout of managed array.
        /// <br/>
        /// The current runtime is auto-detected. Avoid using this method if know fixed runtime information.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining), AssumeInputsValid]
        public static arrayref<T> newf([ValidRange] int length) =>
            JITPlatformEnvironment.IsRunningOnMono
            ? newf_mono(length)
            : newf_clr(length);

        /// <summary>
        /// new fast. No argument validation. No zero-init.
        /// <br/>
        /// Allocates in unmanaged memory, using the memory layout of CoreCLR managed array.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining), AssumeInputsValid, MonoRuntimeNotSupported]
        public static arrayref<T> newf_clr([ValidRange] int length)
        {
            var sz = (uint)length * ILUnsafe.SizeOf<T>() + sizeof(long) * 3;

            var ptr = (IntPtr*)Marshal.AllocHGlobal((IntPtr)sz);

            newf_setHeader_clr(ptr, length);

            return new arrayref<T>(ptr + 1, ptr + 2);
        }

        /// <summary>
        /// new fast. No argument validation. No zero-init.
        /// <br/>
        /// Allocates in unmanaged memory, using the memory layout of mono (MonoRuntime) managed array.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining), AssumeInputsValid]
        public static arrayref<T> newf_mono([ValidRange] int length)
        {
            var sz = (uint)length * ILUnsafe.SizeOf<T>() + sizeof(long) * 4;

            var ptr = (IntPtr*)Marshal.AllocHGlobal((IntPtr)sz);

            newf_setHeader_mono(ptr, length);

            return new arrayref<T>(ptr, ptr + 3);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static arrayref<T> mono_header(IntPtr* ptr, long size)
        {
            const int REQ_SIZE = 4;

            if (size < REQ_SIZE * sizeof(long))
                return default;

            int maxElementLength = (int)(size - REQ_SIZE) / ILUnsafe.SizeOf<T>();
            newf_setHeader_clr(ptr, maxElementLength);

            return new arrayref<T>(ptr, ptr + 3);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static arrayref<T> clr_header(IntPtr* ptr, long size)
        {
            const int REQ_SIZE = 3;

            if (size < REQ_SIZE * sizeof(long))
                return default;

            int maxElementLength = (int)(size - REQ_SIZE) / ILUnsafe.SizeOf<T>();
            newf_setHeader_clr(ptr, maxElementLength);

            return new arrayref<T>(ptr + 1, ptr + 2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void newf_setHeader_clr(IntPtr* ptr, int length)
        {
            ptr[0] = IntPtr.Zero;
            ptr[1] = *(IntPtr*)ILUnsafe.AsPointer(Array.Empty<T>());
            ptr[2] = (IntPtr)(uint)length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void newf_setHeader_mono(IntPtr* ptr, int length)
        {
            ptr[0] = *(IntPtr*)ILUnsafe.AsPointer(Array.Empty<T>());
            ptr[1] = IntPtr.Zero;
            ptr[2] = IntPtr.Zero;
            ptr[3] = (IntPtr)(uint)length;
        }
        #endregion
    }
}
