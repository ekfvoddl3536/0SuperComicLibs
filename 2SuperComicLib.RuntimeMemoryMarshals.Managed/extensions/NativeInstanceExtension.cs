
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

using System.Runtime.CompilerServices;
using System.Security;

namespace SuperComicLib.RuntimeMemoryMarshals
{
    [SuppressUnmanagedCodeSecurity]
    public static unsafe class NativeInstanceExtension
    {
        /// <summary>
        /// Get class reference -OR- read struct data by value
        /// <br/>
        /// Do not use this method if you know whether the <typeparamref name="T"/> is a class or a struct.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining), MonoRuntimeNotSupported]
        public static T GetValueAutomaticReferenceOrByValue<T>(this in NativeInstance<T> @this)
        {
            if (typeof(T).IsClass)
            {
                byte* t0 = @this._Ptr + sizeof(void*);
                return Unsafe.As<byte, T>(ref Unsafe.AsRef<byte>(&t0));
            }
            else
                return Unsafe.AsRef<T>(@this._Ptr + (sizeof(void*) << 1));
        }

        /// <summary>
        /// Set value direct (class type is not allowed)
        /// <br/>
        /// Cannot be used when generic type <typeparamref name="T"/> is a class type.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining), MonoRuntimeNotSupported]
        public static void SetValueDirectByValue<T>(this in NativeInstance<T> @this, in T value)
        {
            if (typeof(T).IsClass)
                throw new System.InvalidOperationException("class type is not allowed.");

            Unsafe.AsRef<T>(@this._Ptr + (sizeof(void*) << 1)) = value;
        }

        /// <summary>
        /// Get class reference
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining), MonoRuntimeNotSupported]
        public static T Value<T>(this in NativeInstance<T> @this) where T : class =>
            ILUnsafe.AsClass<T>(@this._Ptr + sizeof(void*));

        /// <summary>
        /// Copy instance data from <paramref name="source"/>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining), MonoRuntimeNotSupported]
        public static void CopyFrom<T>(this in NativeInstance<T> @this, T source) where T : class
        {
            if (@this._Ptr == null || source == null)
                return;

            int sz = NativeClass.InstanceSizeOf<T>() - (sizeof(void*) << 1);

            if (sz <= 0) return;

            ref var pdst = ref @this._Ptr[sizeof(void*) << 1];
            ref var psrc = ref Unsafe.As<_classDataRef>(source).Item;

            Unsafe.CopyBlockUnaligned(ref pdst, ref psrc, (uint)sz);
        }

        /// <summary>
        /// Copies the instance data to <paramref name="destination"/>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining), MonoRuntimeNotSupported]
        public static void CopyTo<T>(this in NativeInstance<T> @this, T destination) where T : class
        {
            if (@this._Ptr == null || destination == null)
                return;

            int sz = NativeClass.InstanceSizeOf<T>() - (sizeof(void*) << 1);

            if (sz <= 0) return;

            ref var pdst = ref Unsafe.As<_classDataRef>(destination).Item;
            ref var psrc = ref @this._Ptr[sizeof(void*) << 1];

            Unsafe.CopyBlockUnaligned(ref pdst, ref psrc, (uint)sz);
        }

        /// <summary>
        /// Directly references the managed structure data of this <see cref="NativeInstance{T}"/>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining), MonoRuntimeNotSupported]
        public static ref T DirectValue<T>(this in NativeInstance<T> @this) where T : struct =>
            ref Unsafe.AsRef<T>(@this._Ptr + (sizeof(void*) << 1));

        /// <summary>
        /// Gets a reference to the managed structure data of this <see cref="NativeInstance{T}"/>
        /// <br/>
        /// Use in some scenarios where a reference to data must be passed without the <see langword="ref"/> keyword.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining), MonoRuntimeNotSupported]
        public static varef_t<T> AsUnmanagedGCReference<T>(this in NativeInstance<T> @this) where T : struct =>
            new varef_t<T>(@this._Ptr + (sizeof(void*) << 1));

        /// <summary>
        /// Gets a read-only reference to the managed structure data of this <see cref="NativeInstance{T}"/>
        /// <br/>
        /// Use in some scenarios where a read-only reference to data must be passed without the <see langword="ref"/> keyword.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining), MonoRuntimeNotSupported]
        public static cvaref<T> AsUnmanagedReadOnlyGCReference<T>(this in NativeInstance<T> @this) where T : struct =>
            new cvaref<T>(@this._Ptr + (sizeof(void*) << 1));
    }
}
