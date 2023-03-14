﻿// MIT License
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

#pragma warning disable
using System;
using System.Runtime.CompilerServices;
using System.Security;

namespace SuperComicLib.RuntimeMemoryMarshals
{
    [SuppressUnmanagedCodeSecurity, SecurityCritical]
    public static unsafe class ArrayUnsafeGetElementReferenceManagedSCLMemoryExtension
    {
        #region dotnet
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref T refdata_dotnet<T>(in this Memory<T> memory) => 
            ref memory._source.refdata_dotnet((IntPtr)(uint)memory._start);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref readonly T refdata_dotnet<T>(in this ConstMemory<T> memory) => 
            ref memory.DangerousGetArray().refdata_dotnet((IntPtr)(uint)memory._start);
        #endregion

        #region dotnet
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref T refdata_mono<T>(in this Memory<T> memory) =>
            ref memory._source.refdata_mono((IntPtr)(uint)memory._start);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref readonly T refdata_mono<T>(in this ConstMemory<T> memory) =>
            ref memory.DangerousGetArray().refdata_mono((IntPtr)(uint)memory._start);
        #endregion

        #region auto-detect
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref T refdata<T>(in this Memory<T> memory) =>
            ref JITPlatformEnvironment.IsRunningOnMono
            ? ref refdata_mono<T>(memory)
            : ref refdata_dotnet<T>(memory);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref readonly T refdata<T>(in this ConstMemory<T> memory) =>
            ref JITPlatformEnvironment.IsRunningOnMono
            ? ref refdata_mono<T>(memory)
            : ref refdata_dotnet<T>(memory);
        #endregion
    }
}
