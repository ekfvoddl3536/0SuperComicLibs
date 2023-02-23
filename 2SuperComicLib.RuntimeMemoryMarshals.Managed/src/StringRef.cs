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

#pragma warning disable CS1591
#pragma warning disable IDE1006
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using SuperComicLib.CodeContracts;

namespace SuperComicLib.RuntimeMemoryMarshals
{
    [StructLayout(LayoutKind.Sequential), MonoRuntimeNotSupported]
    public readonly unsafe struct StringRef : IEquatable<StringRef>, IDisposable
    {
        internal readonly byte* _Ptr;

        #region constructor
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal StringRef(IntPtr* _Ptr) => this._Ptr = (byte*)_Ptr;
        #endregion

        #region indexer
        /// <summary>
        /// Same as: <see cref="string.this[int]"/>.
        /// <br/>
        /// but, It does not perform validation, such as range checking. API for high-performance scenarios.
        /// </summary>
        public ref readonly char this[[ValidRange] int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining), AssumeOperationValid, AssumeInputsValid]
            get => ref GetCharPtr()[index];
        }

        /// <summary>
        /// Same as: <see cref="string.this[int]"/>.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining), AssumeOperationValid]
        public ref readonly char at(int index)
        {
            if ((uint)index > (uint)Length)
                throw new ArgumentOutOfRangeException(nameof(index));

            return ref GetCharPtr()[index];
        }
        #endregion

        #region property
        /// <summary>
        /// Same as: <see cref="Type.TypeHandle"/>.
        /// </summary>
        public IntPtr TypeHandle
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining), AssumeOperationValid]
            get => *(IntPtr*)(_Ptr + sizeof(void*));
        }

        /// <summary>
        /// Same as: <see cref="string.Length"/>.
        /// </summary>
        public int Length
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining), AssumeOperationValid]
            get => *(int*)(_Ptr + (sizeof(void*) << 1));
        }
        #endregion

        #region methods
        [MethodImpl(MethodImplOptions.AggressiveInlining), AssumeOperationValid]
        public char* GetCharPtr() => (char*)(_Ptr + (sizeof(void*) << 1) + sizeof(int));

        /// <summary>
        /// Gets a <see cref="NativeSpan{T}"/>.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining), AssumeOperationValid]
        public NativeSpan<char> AsSpan() => new NativeSpan<char>(GetCharPtr(), Length);

        /// <summary>
        /// Same as: <see cref="string.ToCharArray()"/>.
        /// <br/>
        /// but, Allocates an array in unmanaged memory.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining), AssumeOperationValid, NoOverhead]
        public ArrayRef<char> ToUnmanagedCharArray()
        {
            int len = Length;
            if (len <= 0)
                return default;

            var res = NativeClass.GetUninitializedUnmanagedArray_dotnet<char>(len);

            Marshal.Copy((IntPtr)GetCharPtr(), res.AsManaged(), 0, len);

            return res;
        }

        /// <summary>
        /// Same as: <see cref="string.ToCharArray()"/>.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining), AssumeOperationValid, NoOverhead]
        [return: NotNull]
        public char[] ToCharArray()
        {
            int len = Length;
            if (len <= 0)
                return Array.Empty<char>();

            var res = new char[len];

            Marshal.Copy((IntPtr)GetCharPtr(), res, 0, len);

            return res;
        }

        /// <summary>
        /// Same as: <see cref="string.ToCharArray(int, int)"/>.
        /// <br/>
        /// but, Allocates an array in unmanaged memory.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining), AssumeOperationValid, NoOverhead]
        public ArrayRef<char> ToUnmanagedCharArray(int startIndex, int length)
        {
            int len = Length;
            if ((uint)startIndex > (uint)len)
                throw new ArgumentOutOfRangeException(nameof(startIndex));

            if ((uint)(len - startIndex) < (uint)length)
                throw new ArgumentOutOfRangeException(nameof(length));

            if ((len | length) <= 0)
                return default;

            var res = NativeClass.GetUninitializedUnmanagedArray_dotnet<char>(length);

            Marshal.Copy((IntPtr)(GetCharPtr() + startIndex), res.AsManaged(), 0, length);

            return res;
        }

        /// <summary>
        /// Same as: <see cref="string.ToCharArray(int, int)"/>.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining), AssumeOperationValid, NoOverhead]
        [return: NotNull]
        public char[] ToCharArray(int startIndex, int length)
        {
            int len = Length;
            if ((uint)startIndex > (uint)len)
                throw new ArgumentOutOfRangeException(nameof(startIndex));

            if ((uint)(len - startIndex) < (uint)length)
                throw new ArgumentOutOfRangeException(nameof(length));

            if ((len | length) <= 0)
                return Array.Empty<char>();

            var res = new char[length];

            Marshal.Copy((IntPtr)(GetCharPtr() + startIndex), res, 0, length);

            return res;
        }

        /// <summary>
        /// Gets a reference to a managed string allocated in unmanaged memory.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining), AssumeOperationValid]
        public string AsManaged() => ILUnsafe.AsClass<string>(_Ptr + sizeof(void*));

        [MethodImpl(MethodImplOptions.AggressiveInlining), AssumeOperationValid]
        public void Clear() => Unsafe.InitBlockUnaligned(GetCharPtr(), 0, (uint)Length << 1);
        #endregion

        #region interface impl
        /// <summary>
        /// Shallow comparison.
        /// </summary>
        public bool Equals(StringRef other) => this == other;
        #endregion

        #region dispose
        /// <summary>
        /// <see cref="IDisposable.Dispose()"/>.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose() => Marshal.FreeHGlobal((IntPtr)_Ptr);
        #endregion

        #region override
        public override bool Equals(object obj) => obj is StringRef other1 && this == other1;

        public override int GetHashCode() => ToString().GetHashCode();

        /// <exception cref="InvalidOperationException">Internal pointer is null</exception>
        public override string ToString() =>
            _Ptr == null
            ? throw new InvalidOperationException("This object has not been initialized")
            : AsManaged();
        #endregion

        #region operator
        /// <summary>
        /// Shallow comparison.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(StringRef left, StringRef right) => left._Ptr == right._Ptr;
        /// <summary>
        /// Shallow comparison.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(StringRef left, StringRef right) => left._Ptr == right._Ptr;
        #endregion
    }
}
