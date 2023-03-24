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
    [StructLayout(LayoutKind.Sequential), MonoRuntimeNotSupported]
    public readonly unsafe struct stringref : IEquatable<stringref>, IDisposable
    {
        internal readonly byte* _Ptr;

        #region constructor
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal stringref(void* ptr) => _Ptr = (byte*)ptr;

        /// <summary>
        /// Creates a new <see cref="stringref"/>.
        /// </summary>
        /// <param name="length">length of string</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="length"/> is negative.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public stringref(int length)
        {
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length));

            this = newf(length);
        }

        /// <summary>
        /// Creates a new <see cref="stringref"/>.
        /// </summary>
        /// <param name="value">source string</param>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is null</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="value"/>.<see cref="string.Length"/> is negative.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public stringref(string value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (value.Length < 0)
                throw new ArgumentOutOfRangeException(nameof(value));

            this = newf(value);
        }
        #endregion

        #region indexer
        /// <summary>
        /// Same as: <see cref="string.this[int]"/>.
        /// <br/>
        /// but, It does not perform validation, such as range checking. API for high-performance scenarios.
        /// </summary>
        public ref char this[[ValidRange] int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining), AssumeOperationValid, AssumeInputsValid]
            get => ref GetCharPtr()[index];
        }

        /// <summary>
        /// Same as: <see cref="string.this[int]"/>.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining), AssumeOperationValid]
        public ref char at(int index)
        {
            if ((uint)index > (uint)Length)
                throw new ArgumentOutOfRangeException(nameof(index));

            return ref GetCharPtr()[index];
        }
        #endregion

        #region property
        /// <summary>
        /// Same as: <see cref="string.Length"/>.
        /// </summary>
        public int Length
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining), AssumeOperationValid]
            get
            {
                DEBUG_NULL_CHECK(this);

                return *(int*)(_Ptr + sizeof(void*));
            }
        }
        #endregion

        #region methods
        /// <summary>
        /// Get the address of the first character.
        /// </summary>
        /// <returns>null is never returned, but an invalid address may be returned.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining), AssumeOperationValid]
        public char* GetCharPtr()
        {
            DEBUG_NULL_CHECK(this);

            return (char*)(_Ptr + sizeof(void*) + sizeof(int));
        }

        /// <summary>
        /// Gets a <see cref="NativeSpan{T}"/>.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining), AssumeOperationValid]
        public NativeSpan<char> AsSpan()
        {
            DEBUG_NULL_CHECK(this);

            return new NativeSpan<char>(GetCharPtr(), Length);
        }

        /// <summary>
        /// Gets a <see cref="NativeSpan{T}"/>.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining), AssumeOperationValid]
        public NativeSpan<char> AsSpan(int startIndex)
        {
            DEBUG_NULL_CHECK(this);

            return new NativeSpan<char>(GetCharPtr(), (uint)Length).Slice((uint)startIndex);
        }

        /// <summary>
        /// Gets a <see cref="NativeSpan{T}"/>.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining), AssumeOperationValid]
        public NativeSpan<char> AsSpan(int startIndex, int count)
        {
            DEBUG_NULL_CHECK(this);

            return new NativeSpan<char>(GetCharPtr(), (uint)Length).Slice((uint)startIndex, (uint)count);
        }

        /// <summary>
        /// Same as: <see cref="string.ToCharArray()"/>.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining), AssumeOperationValid, NoOverhead]
        [return: NotNull]
        public char[] ToCharArray()
        {
            DEBUG_NULL_CHECK(this);

            int len = Length;
            if (len <= 0)
                return Array.Empty<char>();

            var res = new char[len];

            Marshal.Copy((IntPtr)GetCharPtr(), res, 0, len);

            return res;
        }

        /// <summary>
        /// Same as: <see cref="string.ToCharArray(int, int)"/>.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining), AssumeOperationValid, NoOverhead]
        [return: NotNull]
        public char[] ToCharArray(int startIndex, int length)
        {
            DEBUG_NULL_CHECK(this);

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
        /// Same as: <see cref="string.ToCharArray()"/>.
        /// <br/>
        /// but, Allocates an array in unmanaged memory.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining), AssumeOperationValid, NoOverhead]
        public arrayref<char> ToUnmanagedCharArray()
        {
            DEBUG_NULL_CHECK(this);

            int len = Length;
            if (len <= 0)
                return default;

            var res = arrayref<char>.newf_dotnet(len);

            Marshal.Copy((IntPtr)GetCharPtr(), res.AsManaged(), 0, len);

            return res;
        }

        /// <summary>
        /// Same as: <see cref="string.ToCharArray(int, int)"/>.
        /// <br/>
        /// but, Allocates an array in unmanaged memory.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining), AssumeOperationValid, NoOverhead]
        public arrayref<char> ToUnmanagedCharArray(int startIndex, int length)
        {
            DEBUG_NULL_CHECK(this);

            int len = Length;
            if ((uint)startIndex > (uint)len)
                throw new ArgumentOutOfRangeException(nameof(startIndex));

            if ((uint)(len - startIndex) < (uint)length)
                throw new ArgumentOutOfRangeException(nameof(length));

            if ((len | length) <= 0)
                return default;

            var res = arrayref<char>.newf_dotnet(length);

            Marshal.Copy((IntPtr)(GetCharPtr() + startIndex), res.AsManaged(), 0, length);

            return res;
        }

        /// <summary>
        /// Gets a reference to a managed string allocated in unmanaged memory.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining), AssumeOperationValid]
        public string AsManaged()
        {
            DEBUG_NULL_CHECK(this);

            return ILUnsafe.AsClass<string>(_Ptr);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining), AssumeOperationValid]
        public void Clear()
        {
            DEBUG_NULL_CHECK(this);

            Unsafe.InitBlockUnaligned(GetCharPtr(), 0, (uint)Length << 1);
        }
        #endregion

        #region interface impl
        /// <summary>
        /// Shallow comparison.
        /// </summary>
        public bool Equals(stringref other) => this == other;
        #endregion

        #region dispose
        /// <summary>
        /// <see cref="IDisposable.Dispose()"/>.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose()
        {
            if (_Ptr != null)
                Marshal.FreeHGlobal((IntPtr)(_Ptr - sizeof(void*)));
        }
        #endregion

        #region override
        public override bool Equals(object obj) => obj is stringref other1 && this == other1;
        public override int GetHashCode() => ToString().GetHashCode();
        [return: NotNull]
        public override string ToString() =>
            _Ptr == null
            ? string.Empty
            : AsManaged();
        #endregion

        #region operator
        /// <summary>
        /// Sequential comparison
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(stringref left, stringref right) => left.AsManaged() == right.AsManaged();
        /// <summary>
        /// Sequential comparison
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(stringref left, stringref right) => left.AsManaged() != right.AsManaged();
        #endregion

        #region cast
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator stringref(string v) => new stringref(v);
        #endregion

        #region new stringref
        /// <summary>
        /// new fast. No argument validation.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining), AssumeInputsValid]
        public static stringref newf([ValidRange] int length)
        {
            var ptr = (IntPtr*)Marshal.AllocHGlobal(align_size(length));

            ptr[0] = IntPtr.Zero;
            ptr[1] = typeof(string).TypeHandle.Value;
            *(int*)(ptr + 2) = length;

            return new stringref(ptr + 1);
        }

        /// <summary>
        /// new fast. No argument validation.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining), AssumeInputsValid]
        public static stringref newf([DisallowNullOrEmpty] string value)
        {
            var res = newf(value.Length);

            fixed (char* psrc = value)
            {
                long sizeInBytes = (uint)(value.Length << 1);
                Buffer.MemoryCopy(psrc, res.GetCharPtr(), sizeInBytes, sizeInBytes);
            }

            return res;
        }
        #endregion

        #region helper
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int align_size(int charLength)
        {
            var tSz = (charLength << 1) + sizeof(void*) * 3 + sizeof(char);
            return (tSz + (sizeof(void*) - 1)) & -sizeof(void*);
        }

        [Conditional("DEBUG")]
        internal static void DEBUG_NULL_CHECK(stringref value)
        {
            if (value._Ptr == null)
                throw new NullReferenceException(nameof(stringref));
        }
        #endregion
    }
}
