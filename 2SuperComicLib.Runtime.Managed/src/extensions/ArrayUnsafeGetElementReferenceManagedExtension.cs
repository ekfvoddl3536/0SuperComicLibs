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

#pragma warning disable
using System;
using System.Runtime.CompilerServices;
using System.Security;

namespace SuperComicLib.Runtime
{
    [SuppressUnmanagedCodeSecurity, SecurityCritical]
    public static unsafe class ArrayUnsafeGetElementReferenceManagedInt32IndexMOVRRExtension
    {
        // :::NOTE:::
        //  아마도, mov r32/r32 연산이 movsxd 보다 조금 더 빠를 것입니다.

        #region clr
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref T refdata_clr<T>(this T[] array, int index) => ref array.refdata_clr((long)(uint)index);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref T refdata_clr<T>(this T[] array, uint index) => ref array.refdata_clr((long)index);
        #endregion

        #region mono
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref T refdata_mono<T>(this T[] array, int index) => ref array.refdata_mono((long)(uint)index);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref T refdata_mono<T>(this T[] array, uint index) => ref array.refdata_mono((long)index);
        #endregion

        #region auto-detect
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref T refdata<T>(this T[] array) =>
            ref JITPlatformEnvironment.IsRunningOnMono
            ? ref array.refdata_mono()
            : ref array.refdata_clr();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref T refdata<T>(this T[] array, long index) =>
            ref JITPlatformEnvironment.IsRunningOnMono
            ? ref array.refdata_mono(index)
            : ref array.refdata_clr(index);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref T refdata<T>(this T[] array, int index) =>
            ref JITPlatformEnvironment.IsRunningOnMono
            ? ref refdata_mono(array, index)
            : ref refdata_clr(array, index);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref T refdata<T>(this T[] array, uint index) =>
            ref JITPlatformEnvironment.IsRunningOnMono
            ? ref refdata_mono(array, index)
            : ref refdata_clr(array, index);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref Refpoint<T> refpoint<T>(this T[] array) =>
            ref JITPlatformEnvironment.IsRunningOnMono
            ? ref array.refpoint_mono()
            : ref array.refpoint_clr();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref Refpoint<T> refpoint<T>(this T[] array, long index) =>
            ref JITPlatformEnvironment.IsRunningOnMono
            ? ref array.refpoint_mono(index)
            : ref array.refpoint_clr(index);
        #endregion
    }
}
