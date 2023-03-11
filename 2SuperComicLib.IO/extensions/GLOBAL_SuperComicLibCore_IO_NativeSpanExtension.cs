// MIT License
//
// Copyright (c) 2020-2023. SuperComic (ekfvoddl3535@naver.com)
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
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;

namespace SuperComicLib.IO
{
    [SuppressUnmanagedCodeSecurity]
    public static unsafe class GLOBAL_SuperComicLibCore_IO_NativeSpanExtension
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyToStream<T>(this in NativeSpan<T> src, [DisallowNull] Stream dest, [DisallowNullOrEmpty] byte[] buffer) where T : unmanaged => 
            Internal_CopyToStream((byte*)src.Source, (byte*)src.end()._ptr, dest, buffer);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyToStream<T>(this in NativeSpan<T> src, [DisallowNull] Stream dest, int buffer_size) where T : unmanaged =>
            Internal_CopyToStream((byte*)src.Source, (byte*)src.end()._ptr, dest, new byte[buffer_size]);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyToStream<T>(this in NativeSpan<T> src, [DisallowNull] Stream dest) where T : unmanaged =>
            Internal_CopyToStream((byte*)src.Source, (byte*)src.end()._ptr, dest, new byte[16384]);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyToStream<T>(this in NativeConstSpan<T> src, [DisallowNull] Stream dest, int buffer_size) where T : unmanaged =>
            Internal_CopyToStream((byte*)src.DangerousGetPointer(), (byte*)src.end()._ptr, dest, new byte[buffer_size]);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyToStream<T>(this in NativeConstSpan<T> src, [DisallowNull] Stream dest, [DisallowNullOrEmpty] byte[] buffer) where T : unmanaged =>
            Internal_CopyToStream((byte*)src.DangerousGetPointer(), (byte*)src.end()._ptr, dest, buffer);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyToStream<T>(this in NativeConstSpan<T> src, [DisallowNull] Stream dest) where T : unmanaged =>
            Internal_CopyToStream((byte*)src.DangerousGetPointer(), (byte*)src.end()._ptr, dest, new byte[16384]);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Internal_CopyToStream(byte* iter, byte* end, Stream dest, byte[] buffer)
        {
            while (iter != end)
            {
                int count = (int)CMathi.Min((nuint_t)(end - iter), (uint)buffer.Length);
                Marshal.Copy((IntPtr)iter, buffer, 0, count);

                iter += count;

                dest.Write(buffer, 0, count);
            }
        }
    }
}
