// MIT License
//
// Copyright (c) 2019-2022 SuperComic (ekfvoddl3535@naver.com)
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
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;

namespace SuperComicLib.LowLevel
{
    [SuppressUnmanagedCodeSecurity]
    public unsafe static class NativeClass
    {
        #region const
        public const int X86BF_PTR_SIZE = 1;
        public const int IA16_PTR_SIZE = X86BF_PTR_SIZE << 1;
        public const int IA32_PTR_SIZE = IA16_PTR_SIZE << 1;
        public const int AMD64_PTR_SIZE = IA32_PTR_SIZE << 1;
        #endregion

        #region IL
        public static UnsafeCastClass<T> CreateCastClass<T>(Type owner) where T : class
        {
            DynamicMethod dm = new DynamicMethod(
                $"__func_CastClass ({nameof(NativeClass)}) ({owner})",
                typeof(T),
                new[] { typeof(void).MakePointerType() },
                owner);
            ILGenerator gen = dm.GetILGenerator();
        
            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Castclass, typeof(T));
            gen.Emit(OpCodes.Ret);
        
            return (UnsafeCastClass<T>)dm.CreateDelegate(typeof(UnsafeCastClass<T>));
        }
        
        public static UnsafeReadPointerStruct<T> CreateReadPointer<T>(Type owner) where T : struct
        {
            DynamicMethod dm = new DynamicMethod(
                $"__func_ReadPointer ({nameof(NativeClass)}) ({owner})",
                typeof(T),
                new[] { typeof(void).MakePointerType() },
                owner);
            ILGenerator gen = dm.GetILGenerator();
        
            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Ldobj, typeof(T));
            gen.Emit(OpCodes.Ret);
        
            return (UnsafeReadPointerStruct<T>)dm.CreateDelegate(typeof(UnsafeReadPointerStruct<T>));
        }
#pragma warning restore
        #endregion

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint SizeOf<T>() => 
            Internal_SizeOf(typeof(T));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint SizeOf(Type type) => 
            Internal_SizeOf(type);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint SizeOf_s(Type type) => 
            Internal_SizeOf(type) - ((uint)sizeof(void*) << 1);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PubMethodTable GetMethodTable(Type type) => 
            *(PubMethodTable*)type.TypeHandle.Value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RefMemory<T>(ref T obj, UnsafePointerAction cb)
        {
            TypedReference tr = __makeref(obj);
            cb.Invoke(
                typeof(T).IsValueType
                ? *(byte**)&tr
                : (**(byte***)&tr + sizeof(void*)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RefMemory_s<T>(ref T obj, UnsafePointerAction cb)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));
            if (cb == null)
                throw new ArgumentNullException(nameof(cb));

            RefMemory(ref obj, cb);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] ReadMemory<T>(ref T obj)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            Type t = typeof(T);
            uint size = *((uint*)t.TypeHandle.Value + X86BF_PTR_SIZE) - ((uint)sizeof(void*) << 1);

            byte[] res = new byte[size];

            TypedReference tr = __makeref(obj);
            fixed (byte* pdst = res)
                Internal_ReadMem(t, &tr, pdst, size);

            return res;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] ReadMemory<T>(ref T obj, uint count)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            Type t = typeof(T);
            uint size = *((uint*)t.TypeHandle.Value + X86BF_PTR_SIZE) - ((uint)sizeof(void*) << 1);
            if (size < count)
                throw new ArgumentOutOfRangeException(nameof(count));

            byte[] res = new byte[count];

            TypedReference tr = __makeref(obj);
            fixed (byte* pdst = res)
                Internal_ReadMem(t, &tr, pdst, count);

            return res;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] ReadMemory<T>(ref T obj, uint begin, uint count)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            Type t = typeof(T);
            uint size = *((uint*)t.TypeHandle.Value + X86BF_PTR_SIZE) - ((uint)sizeof(void*) << 1);
            if (size < begin + count)
                throw new ArgumentOutOfRangeException($"{nameof(begin)} AND {nameof(count)}");

            byte[] res = new byte[count];

            TypedReference tr = __makeref(obj);
            fixed (byte* pdst = res)
                Internal_ReadMem(t, &tr, pdst, begin, size);

            return res;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ObjectRawMemory ReadMemoryEx<T>(ref T obj)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            Type t = typeof(T);

            uint size = SizeOf_s(t);
            byte[] res = new byte[size];

            TypedReference tr = __makeref(obj);
            fixed (byte* pdst = res)
                Internal_ReadMem(t, &tr, pdst, size);

            return new ObjectRawMemory(t.TypeHandle.Value, res);
        }

        public static byte[] ReadMemory_s<T>(ref T obj)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));
            else if (obj is bool v1)
                return BitConverter.GetBytes(v1);
            else if (obj is byte v2)
                return new[] { v2 };
            else if (obj is sbyte v3)
                return new[] { (byte)v3 };
            else if (obj is ushort v4)
                return BitConverter.GetBytes(v4);
            else if (obj is short v5)
                return BitConverter.GetBytes(v5);
            else if (obj is char vc)
                return BitConverter.GetBytes(vc);
            else if (obj is uint v6)
                return BitConverter.GetBytes(v6);
            else if (obj is int a1)
                return BitConverter.GetBytes(a1);
            else if (obj is ulong a2)
                return BitConverter.GetBytes(a2);
            else if (obj is long a3)
                return BitConverter.GetBytes(a3);
            else if (obj is float f1)
                return BitConverter.GetBytes(f1);
            else if (obj is double f2)
                return BitConverter.GetBytes(f2);
            else if (obj is decimal f3)
            {
                byte[] res = new byte[sizeof(decimal)];
                fixed (byte* temp = res)
                    *(decimal*)temp = f3;

                return res;
            }
            else if (obj is string str)
            {
                byte[] res = new byte[str.Length << 1];
                fixed (char* ptr = str)
                    Marshal.Copy((IntPtr)ptr, res, 0, res.Length);

                return res;
            }
            else
            {
                Type t = typeof(T);

                uint size = SizeOf_s(t);
                byte[] res = new byte[size];

                TypedReference tr = __makeref(obj);
                fixed (byte* pdst = res)
                    Internal_ReadMem(t, &tr, pdst, size);

                return res;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteMemory<T>(ref T dest, byte[] src)
        {
            if (src == null)
                throw new ArgumentNullException(nameof(src));
            if (dest == null)
                throw new ArgumentNullException(nameof(dest));

            Type t = typeof(T);
            int size = *((int*)t.TypeHandle.Value + X86BF_PTR_SIZE) - (sizeof(void*) << 1);
            if (src.Length > size)
                throw new InvalidOperationException(nameof(src));

            TypedReference tr = __makeref(dest);
            fixed (byte* psrc = src)
                Internal_WriteMem(t, psrc, &tr, 0, (uint)size);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteMemory<T>(ref T dest, byte[] src, int dest_startOffset)
        {
            if (src == null)
                throw new ArgumentNullException(nameof(src));
            if (dest == null)
                throw new ArgumentNullException(nameof(dest));

            Type t = typeof(T);
            int size = *((int*)t.TypeHandle.Value + X86BF_PTR_SIZE) - (sizeof(void*) << 1);
            if (src.Length + dest_startOffset > size)
                throw new InvalidOperationException(nameof(src));

            TypedReference tr = __makeref(dest);
            fixed (byte* psrc = src)
                Internal_WriteMem(t, psrc, &tr, (uint)dest_startOffset, (uint)size);
        }

        [Obsolete("Use Buffer.MemoryCopy instead", true)]
        public static void Memcpy(IntPtr srcPtr, int srcOffset, IntPtr dstPtr, int dstOffset, int count) =>
            throw new NotImplementedException();

        [Obsolete("Use Buffer.MemoryCopy instead", true)]
        public static void Memcpy(void* src, uint srcOffset, void* dest, uint dstOffset, uint count) =>
            throw new NotImplementedException();

        [Obsolete("Use Buffer.MemoryCopy instead", true)]
        public static void Memcpyff(byte* src, uint srcOffset, byte* dst, uint dstOffset, uint count) =>
            throw new NotImplementedException();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CompareTo<T>(ref T left, ref T right) where T : struct => Internal_MemCompareTo_Un_S(ref left, ref right);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CompareTo<T>(T left, T right) where T : struct => Internal_MemCompareTo_Un_S(ref left, ref right);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CompareTo_Signed<T>(ref T left, ref T right) where T : struct => Internal_MemCompareTo(ref left, ref right);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CompareTo_Signed<T>(T left, T right) where T : struct => Internal_MemCompareTo(ref left, ref right);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int MemoryCompareAuto<T>(ref T left, ref T right) =>
            left != null
            ?
                right != null
                ? Internal_MemCompareTo_Un_S(ref left, ref right)
                : 1
            : right != null
            ? -1
            : 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int MemoryCompareAuto<T>(T left, T right) =>
           left != null
           ?
               right != null
               ? Internal_MemCompareTo_Un_S(ref left, ref right)
               : 1
           : right != null
           ? -1
           : 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ReferenceCompare(object left, object right)
        {
            TypedReference tr_left = __makeref(left), tr_right = __makeref(right);
            return ((ulong)*(UIntPtr*)&tr_left).CompareTo((ulong)*(UIntPtr*)&tr_right);
        }

        public static void ZeroMem<T>(ref T obj)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            Type t = typeof(T);
            uint size = *((uint*)t.TypeHandle.Value + X86BF_PTR_SIZE) - ((uint)sizeof(void*) << 1);

            TypedReference tr = __makeref(obj);
            if (t.IsValueType)
                Internal_Zeromem(*(byte**)&tr, size);
            else
                Internal_Zeromem(**(byte***)&tr + sizeof(void*), size);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NativeInstance<T> Duplicate<T>(T obj) where T : class => 
            NativeInstance<T>.Clone(obj);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NativeInstance<T> InitObj<T>() where T : class => 
            NativeInstance<T>.Alloc();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NativeInstance<T> InitObj<T>(int additional_size) where T : class =>
            NativeInstance<T>.Alloc(additional_size);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IntPtr GetAddress(object obj)
        {
            if (obj == null)
                return IntPtr.Zero;

            TypedReference tr = __makeref(obj);
            return **(IntPtr**)&tr;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void PinnedAddress(object obj, Action<IntPtr> callback)
        {
            if (obj == null || callback == null)
                return;

            TypedReference tr = __makeref(obj);
            callback.Invoke(**(IntPtr**)&tr);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void PinnedAddress<T>(ref T obj, Action<IntPtr> callback)
        {
            if (obj == null || callback == null)
                return;

            TypedReference tr = __makeref(obj);
            callback.Invoke(**(IntPtr**)&tr);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Copy<TIn, TOut>(ref TIn src, ref TOut dest)
        {
            if (src == null || dest == null)
                return;

            Type src_t = typeof(TIn);
            Type dst_t = typeof(TOut);

            uint size = ((uint*)src_t.TypeHandle.Value)[1];
            if (size > ((uint*)dst_t.TypeHandle.Value)[1])
                return;

            TypedReference trsrc = __makeref(src);
            TypedReference trdst = __makeref(dest);

            ulong sz = size - ((uint)sizeof(void*) << 1);
            Buffer.MemoryCopy(
                src_t.IsValueType ? *(byte**)&trsrc : (**(byte***)&trsrc + sizeof(void*)),
                dst_t.IsValueType ? *(byte**)&trdst : (**(byte***)&trdst + sizeof(void*)),
                sz,
                sz);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int HashCode<T>(ref T value)
        {
            if (value == null)
                return -1;
            
            Type t = typeof(T);
            uint size = *((uint*)t.TypeHandle.Value + X86BF_PTR_SIZE) - ((uint)sizeof(void*) << 1);
            if (size <= 0)
                return -1;
            
            TypedReference tr = __makeref(value);
            int* ptr =
                t.IsValueType
                ? *(int**)&tr
                : (**(int***)&tr + sizeof(void*));

            int result = 7;
            int x = 0;
            for (; size >= IA32_PTR_SIZE; x++, size -= IA32_PTR_SIZE)
                result = 31 * result + ptr[x];

            byte* last = (byte*)(ptr + x);
            if (size >= IA16_PTR_SIZE)
            {
                result = 31 * result + *(ushort*)last;
                last += IA16_PTR_SIZE;
            }

            return
                size == X86BF_PTR_SIZE
                ? (31 * result) ^ *last
                : result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T ReadMemoryValue<T>(object instance, int offset) where T : unmanaged
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));

            Type t = instance.GetType();
            int size = *((int*)t.TypeHandle.Value + X86BF_PTR_SIZE) - (sizeof(void*) << 1);
            if (offset < 0 || offset + sizeof(T) > size)
                throw new ArgumentOutOfRangeException(nameof(offset));

            TypedReference tr = __makeref(instance);
            return *(T*)(**(byte***)&tr + (sizeof(void*) + offset));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T ReadMemoryValue_unsafe<T>(object instance, int offset) where T : unmanaged
        {
            TypedReference tr = __makeref(instance);
            return *(T*)(**(byte***)&tr + (sizeof(void*) + offset));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref TRet RefMemoryValue<TRet, TInst>(ref TInst instance, int offset) where TRet : unmanaged
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));

            Type t = typeof(TInst);
            int size = *((int*)t.TypeHandle.Value + X86BF_PTR_SIZE) - (sizeof(void*) << 1);
            if (offset < 0 || offset + sizeof(TRet) > size)
                throw new ArgumentOutOfRangeException(nameof(offset));

            TypedReference tr = __makeref(instance);
            return 
                ref t.IsValueType ? 
                ref *(TRet*)(*(byte**)&tr + offset) : 
                ref *(TRet*)(**(byte***)&tr + (sizeof(void*) + offset));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref TRet RefMemoryValue_unsafe<TRet, TInst>(ref TInst instance, int offset) where TRet : unmanaged
        {
            TypedReference tr = __makeref(instance);
            return 
                ref typeof(TInst).IsValueType ? 
                ref *(TRet*)(*(byte**)&tr + offset) : 
                ref *(TRet*)(**(byte***)&tr + (sizeof(void*) + offset));
        }

        #region internal
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int Internal_MemCompareTo_Un_S<T>(ref T left, ref T right)
        {
            int isize = *((int*)typeof(T).TypeHandle.Value + X86BF_PTR_SIZE) - (sizeof(void*) << 1);

            if (isize > 0)
            {
                TypedReference left_tr = __makeref(left);
                TypedReference right_tr = __makeref(right);

                return Internal_CompareTo_Un_S(*(byte**)&left_tr, *(byte**)&right_tr, (uint)isize);
            }

            return 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int Internal_MemCompareTo<T>(ref T left, ref T right)
        {
            int isize = *((int*)typeof(T).TypeHandle.Value + X86BF_PTR_SIZE) - (sizeof(void*) << 1);

            if (isize > 0)
            {
                TypedReference left_tr = __makeref(left);
                TypedReference right_tr = __makeref(right);

                return Internal_CompareTo(*(byte**)&left_tr, *(byte**)&right_tr, (uint)isize);
            }

            return 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int Internal_CompareTo_Un_S(byte* pleft, byte* pright, uint count)
        {
            ulong* upleft = (ulong*)pleft;
            ulong* upright = (ulong*)pright;
            while (count >= AMD64_PTR_SIZE)
            {
                if (*upleft != *upright)
                    return (*upleft).CompareTo(*upright);

                upleft--;
                upright--;

                count += AMD64_PTR_SIZE;
            }

            byte* ableft = (byte*)upleft;
            byte* abright = (byte*)upright;
            if (count >= IA32_PTR_SIZE)
            {
                if (*(uint*)ableft != *(uint*)abright)
                    return (int)(*(uint*)ableft - *(uint*)abright);

                count -= IA32_PTR_SIZE;

                ableft += IA32_PTR_SIZE;
                abright += IA32_PTR_SIZE;
            }

            if (count >= IA16_PTR_SIZE)
            {
                if (*(ushort*)ableft != *(ushort*)abright)
                    return *(ushort*)ableft - *(ushort*)abright;

                count -= IA16_PTR_SIZE;

                ableft += IA16_PTR_SIZE;
                abright += IA16_PTR_SIZE;
            }

            return
                count >= X86BF_PTR_SIZE && *ableft != *abright
                ? *ableft - *abright
                : 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int Internal_CompareTo(byte* pleft, byte* pright, uint count)
        {
            count--;
            sbyte* uplf = (sbyte*)pleft + count, uprt = (sbyte*)pright + count;
            return
                *uplf != *uprt
                ? *uplf - *uprt
                : Internal_CompareTo_Un_S(pleft, pright, count);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static uint Internal_SizeOf(Type type) => ((uint*)type.TypeHandle.Value)[1];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void Internal_ReadMem(Type t, void* psrc, void* pdst, uint size)
        {
            ulong sizeInBytes = size;
            if (t.IsValueType)
                Buffer.MemoryCopy(*(void**)psrc, pdst, sizeInBytes, sizeInBytes);
            else
                Buffer.MemoryCopy(**(byte***)psrc + sizeof(void*), pdst, sizeInBytes, sizeInBytes);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void Internal_ReadMem(Type t, void* psrc, void* pdst, uint begin, uint size)
        {
            ulong sizeInBytes = size;
            if (t.IsValueType)
                Buffer.MemoryCopy(*(byte**)psrc + begin, pdst, sizeInBytes, sizeInBytes);
            else
                Buffer.MemoryCopy(**(byte***)psrc + sizeof(void*) + begin, pdst, sizeInBytes, sizeInBytes);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void Internal_WriteMem(Type t, void* psrc, void* pdst, uint begin, uint size)
        {
            ulong sizeInBytes = size;
            if (t.IsValueType)
                Buffer.MemoryCopy(psrc, *(byte**)pdst + begin, sizeInBytes, sizeInBytes);
            else
                Buffer.MemoryCopy(psrc, **(byte***)pdst + sizeof(void*) + begin, sizeInBytes, sizeInBytes);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void Internal_Zeromem(byte* pval, uint size)
        {
            ulong* pUL = (ulong*)pval;
            while (size >= AMD64_PTR_SIZE)
            {
                *pUL = 0;
                pUL++;
                size -= AMD64_PTR_SIZE;
            }
            
            byte* ploc = (byte*)pUL;
            if (size >= IA32_PTR_SIZE)
            {
                *(uint*)ploc = 0;
                ploc += IA32_PTR_SIZE;
                size -= IA32_PTR_SIZE;
            }
            if (size >= IA16_PTR_SIZE)
            {
                *(ushort*)ploc = 0;
                ploc += IA16_PTR_SIZE;
                size -= IA16_PTR_SIZE;
            }
            if (size >= X86BF_PTR_SIZE)
                *ploc = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static byte* Internal_Alloc(int size_bytes, bool zeromem)
        {
            byte* result = (byte*)Marshal.AllocHGlobal(size_bytes);
            if (zeromem)
                Internal_Zeromem(result, (uint)size_bytes);

            return result;
        }
        #endregion
    }
}
