﻿// MIT License
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

using System.Runtime.CompilerServices;
using System.Security;

namespace SuperComicLib.Runtime
{
    [SuppressUnmanagedCodeSecurity]
    public static unsafe class NativeInstanceTClassExtension
    {
        /// <summary>
        /// Get class reference
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining), MonoRuntimeNotSupported]
        public static T GetValue<T>(this NativeInstance<T> @this) where T : class =>
            ILUnsafe.AsClass<byte, T>(ref @this._Ptr[sizeof(long)]);

        /// <summary>
        /// Copy instance data from <paramref name="source"/>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining), MonoRuntimeNotSupported]
        public static void DataCopyFrom<T>(this in NativeInstance<T> @this, T source) where T : class
        {
            if (@this._Ptr == null || source == null)
                return;

            int sz = NativeClass.InstanceSizeOf<T>() - (sizeof(long) << 1);

            if (sz <= 0) return;

            ref var pdst = ref @this._Ptr[sizeof(long) << 1];
            ref var psrc = ref ILUnsafe.As<_classDataRef>(source).Item;

            ILUnsafe.CopyBlockUnaligned(ref pdst, psrc, (uint)sz);
        }

        /// <summary>
        /// Copies the instance data to <paramref name="destination"/>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining), MonoRuntimeNotSupported]
        public static void DataCopyTo<T>(this in NativeInstance<T> @this, T destination) where T : class
        {
            if (@this._Ptr == null || destination == null)
                return;

            int sz = NativeClass.InstanceSizeOf<T>() - (sizeof(long) << 1);

            if (sz <= 0) return;

            ref var pdst = ref ILUnsafe.As<_classDataRef>(destination).Item;
            ref var psrc = ref @this._Ptr[sizeof(long) << 1];

            ILUnsafe.CopyBlockUnaligned(ref pdst, psrc, (uint)sz);
        }
    }
}
