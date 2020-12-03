using System;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Security;

namespace SuperComicLib.LowLevel
{
    [SecurityCritical]
    public unsafe static class NativeClass
    {
        #region const
        public const int X86BF_PTR_SIZE = 1;
        public const int IA16_PTR_SIZE = X86BF_PTR_SIZE << 1;
        public const int IA32_PTR_SIZE = IA16_PTR_SIZE << 1;
        public const int AMD64_PTR_SIZE = IA32_PTR_SIZE << 1;
        #endregion

        #region field
        internal static readonly UnsafeMemoryCopyBlock memcpblk = CreateMemcpblk();

        public static readonly int PtrSize_i = IntPtr.Size;
        public static readonly uint PtrSize_u = (uint)PtrSize_i;
        public static readonly bool Is64BitBCL =
#if WIN32
            false;
#elif WIN64
            true;
#else
            PtrSize_i == sizeof(long);
#endif
        #endregion

        #region IL
        private static UnsafeMemoryCopyBlock CreateMemcpblk()
        {
            Type vp = typeof(void).MakePointerType();
            DynamicMethod dm = new DynamicMethod(
                $"__MEMCPBLK__", typeof(void),
                new[] { vp, vp, typeof(uint) },
                typeof(NativeClass));
            ILGenerator g = dm.GetILGenerator();

            g.Emit(OpCodes.Ldarg_1);
            g.Emit(OpCodes.Ldarg_0);
            g.Emit(OpCodes.Ldarg_2);
            g.Emit(OpCodes.Cpblk);
            g.Emit(OpCodes.Ret);

            return (UnsafeMemoryCopyBlock)dm.CreateDelegate(typeof(UnsafeMemoryCopyBlock));
        }

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

        public static uint SizeOf<T>() => Internal_SizeOf(typeof(T));

        public static uint SizeOf(Type type) =>
            type == null
            ? 0
            : Internal_SizeOf(type);

        public static uint SizeOf_s(Type type) =>
            type == null
            ? 0
            : Internal_SizeOf(type) - (PtrSize_u << 1);

        public static PubMethodTable GetMethodTable(Type type) =>
            type == null
            ? default
            : *(PubMethodTable*)type.TypeHandle.Value;

        public static To Convert<From, To>(ref From fm)
            where From : unmanaged
            where To : unmanaged
        {
            fixed (From* ptr = &fm)
                return *(To*)ptr;
        }

        public static To Convert<From, To>(From fm) where From : unmanaged where To : unmanaged
            => *(To*)&fm;

        public static void RefMemory<T>(ref T obj, UnsafePointerAction cb)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));
            if (cb == null)
                throw new ArgumentNullException(nameof(cb));

            TypedReference tr = __makeref(obj);
            cb.Invoke(
                typeof(T).IsValueType
                ? *(byte**)&tr
                : (**(byte***)&tr + PtrSize_i));
        }

        public static byte[] ReadMemory<T>(ref T obj)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            Type t = obj.GetType();
            uint size = *((uint*)t.TypeHandle.Value + X86BF_PTR_SIZE) - (PtrSize_u << 1);

            byte[] res = new byte[size];

            TypedReference tr = __makeref(obj);
            fixed (byte* pdst = res)
                Internal_ReadMem(t, &tr, pdst, size);

            return res;
        }

        public static byte[] ReadMemory<T>(ref T obj, uint count)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            Type t = obj.GetType();
            uint size = *((uint*)t.TypeHandle.Value + X86BF_PTR_SIZE) - (PtrSize_u << 1);
            if (size < count)
                throw new ArgumentOutOfRangeException(nameof(count));

            byte[] res = new byte[count];

            TypedReference tr = __makeref(obj);
            fixed (byte* pdst = res)
                Internal_ReadMem(t, &tr, pdst, count);

            return res;
        }

        public static byte[] ReadMemory<T>(ref T obj, uint begin, uint count)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            Type t = obj.GetType();
            uint size = *((uint*)t.TypeHandle.Value + X86BF_PTR_SIZE) - (PtrSize_u << 1);
            if (size < begin + count)
                throw new ArgumentOutOfRangeException($"{nameof(begin)} AND {nameof(count)}");

            byte[] res = new byte[count];

            TypedReference tr = __makeref(obj);
            fixed (byte* pdst = res)
                Internal_ReadMem(t, &tr, pdst, begin, size);

            return res;
        }

        public static UnsafeCLIMemoryData ReadMemoryEx<T>(ref T obj)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            Type t = obj.GetType();

            uint size = SizeOf_s(t);
            byte[] res = new byte[size];

            TypedReference tr = __makeref(obj);
            fixed (byte* pdst = res)
                Internal_ReadMem(t, &tr, pdst, size);

            return new UnsafeCLIMemoryData(t.TypeHandle.Value, res);
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
                byte[] res = new byte[str.Length * 2];
                fixed (char* ptr = str)
                    Marshal.Copy((IntPtr)ptr, res, 0, res.Length);

                return res;
            }
            else
            {
                Type t = obj.GetType();

                uint size = SizeOf_s(t);
                byte[] res = new byte[size];

                TypedReference tr = __makeref(obj);
                fixed (byte* pdst = res)
                    Internal_ReadMem(t, &tr, pdst, size);

                return res;
            }
        }

        public static void WriteMemory<T>(ref T dest, byte[] src)
        {
            if (src == null)
                throw new ArgumentNullException(nameof(src));
            if (dest == null)
                throw new ArgumentNullException(nameof(dest));

            Type t = dest.GetType();
            int size = *((int*)t.TypeHandle.Value + X86BF_PTR_SIZE) - (PtrSize_i << 1);
            if (src.Length > size)
                throw new InvalidOperationException(nameof(src));

            TypedReference tr = __makeref(dest);
            fixed (byte* psrc = src)
                Internal_WriteMem(t, psrc, &tr, 0, (uint)size);
        }

        public static void WriteMemory<T>(ref T dest, byte[] src, int dest_startOffset)
        {
            if (src == null)
                throw new ArgumentNullException(nameof(src));
            if (dest == null)
                throw new ArgumentNullException(nameof(dest));

            Type t = dest.GetType();
            int size = *((int*)t.TypeHandle.Value + X86BF_PTR_SIZE) - (PtrSize_i << 1);
            if (src.Length + dest_startOffset > size)
                throw new InvalidOperationException(nameof(src));

            TypedReference tr = __makeref(dest);
            fixed (byte* psrc = src)
                Internal_WriteMem(t, psrc, &tr, (uint)dest_startOffset, (uint)size);
        }

        public static void Memcpy(IntPtr srcPtr, int srcOffset, IntPtr dstPtr, int dstOffset, int count)
        {
            if (srcOffset < 0)
                throw new ArgumentOutOfRangeException(nameof(srcOffset));
            if (dstOffset < 0)
                throw new ArgumentOutOfRangeException(nameof(dstOffset));
            if (count <= 0)
                throw new ArgumentOutOfRangeException(nameof(count));
            if (srcPtr == IntPtr.Zero)
                throw new InvalidOperationException(nameof(srcPtr));
            if (dstPtr == IntPtr.Zero)
                throw new InvalidOperationException(nameof(dstPtr));

            memcpblk.Invoke((byte*)srcPtr + srcOffset, (byte*)dstPtr + dstOffset, (uint)count);
        }

        public static void Memcpy(void* src, uint srcOffset, void* dest, uint dstOffset, uint count)
        {
            if (src == null)
                throw new ArgumentException(nameof(src));
            if (dest == null)
                throw new ArgumentException(nameof(dest));
            if (count == 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            memcpblk.Invoke((byte*)src + srcOffset, (byte*)dest + dstOffset, count);
        }

        public static void Memcpyff(byte* src, uint srcOffset, byte* dst, uint dstOffset, uint count)
        {
            if (src == null)
                throw new ArgumentException(nameof(src));
            if (dst == null)
                throw new ArgumentException(nameof(dst));
            if (count == 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            Internal_memcpyff(src, srcOffset, dst, dstOffset, count);
        }

        public static int CompareTo<T>(ref T left, ref T right) where T : struct => Internal_MemCompareTo_Un_S(ref left, ref right);

        public static int CompareTo<T>(T left, T right) where T : struct => Internal_MemCompareTo_Un_S(ref left, ref right);

        public static int CompareTo_Signed<T>(ref T left, ref T right) where T : struct => Internal_MemCompareTo(ref left, ref right);

        public static int CompareTo_Signed<T>(T left, T right) where T : struct => Internal_MemCompareTo(ref left, ref right);

        public static int MemoryCompareAuto<T>(ref T left, ref T right) =>
            left != null
            ?
                right != null
                ? Internal_MemCompareTo_Un_S(ref left, ref right)
                : 1
            : right != null
            ? -1
            : 0;

        public static int MemoryCompareAuto<T>(T left, T right) =>
           left != null
           ?
               right != null
               ? Internal_MemCompareTo_Un_S(ref left, ref right)
               : 1
           : right != null
           ? -1
           : 0;

        public static int ReferenceCompare(object left, object right)
        {
            if (left != null)
            {
                if (right != null)
                {
                    TypedReference pleft = __makeref(left);
                    TypedReference pright = __makeref(right);

                    IntPtr typehnd_left = **(IntPtr**)&pleft;
                    IntPtr typehnd_right = **(IntPtr**)&pright;

                    return
                        Is64BitBCL
                        ? typehnd_left.ToInt64().CompareTo(typehnd_right.ToInt64())
                        : typehnd_left.ToInt32().CompareTo(typehnd_right.ToInt32());
                }
                // left = not null, right = null
                return 1;
            }
            return
                right != null
                ? -1
                : 0;
        }

        public static void ZeroMem<T>(ref T obj)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            Type t = obj.GetType();
            uint size = *((uint*)t.TypeHandle.Value + X86BF_PTR_SIZE) - (PtrSize_u << 1);

            TypedReference tr = __makeref(obj);
            if (t.IsValueType)
                Internal_Zeromem(*(byte**)&tr, size);
            else
                Internal_Zeromem(**(byte***)&tr + PtrSize_i, size);
        }

        public static NativeInstance<T> Duplicate<T>(T obj) where T : class => NativeInstance<T>.Dup(obj);

        public static NativeInstance<T> InitObj<T>() where T : class => NativeInstance<T>.Alloc();

        public static NativeInstance<T> InitObj<T>(int size) where T : class => NativeInstance<T>.Alloc(size);

        public static IntPtr GetAddress(object obj)
        {
            if (obj == null)
                return IntPtr.Zero;

            TypedReference tr = __makeref(obj);
            return **(IntPtr**)&tr;
        }

        public static void PinnedAddress(object obj, Action<IntPtr> callback)
        {
            if (obj == null || callback == null)
                return;

            TypedReference tr = __makeref(obj);
            callback.Invoke(**(IntPtr**)&tr);
        }

        public static void PinnedAddress<T>(ref T obj, Action<IntPtr> callback)
        {
            if (obj == null || callback == null)
                return;

            TypedReference tr = __makeref(obj);
            callback.Invoke(**(IntPtr**)&tr);
        }

        public static void Copy<TIn, TOut>(ref TIn src, ref TOut dest)
        {
            if (src == null || dest == null)
                return;

            Type src_t = typeof(TIn);
            Type dst_t = typeof(TOut);

            uint size = ((uint*)src_t.TypeHandle.Value)[1];
            if (size != ((uint*)dst_t.TypeHandle.Value)[1])
                return;

            TypedReference trsrc = __makeref(src);
            TypedReference trdst = __makeref(dest);

            Internal_memcpyff(
                src_t.IsValueType ? *(byte**)&trsrc : (**(byte***)&trsrc + PtrSize_i),
                0,
                dst_t.IsValueType ? *(byte**)&trdst : (**(byte***)&trdst + PtrSize_i),
                0,
                size - (PtrSize_u << 1));
        }

        public static int HashCode<T>(ref T value)
        {
            if (value == null)
                return -1;

            Type t = typeof(T);
            uint size = *((uint*)t.TypeHandle.Value + X86BF_PTR_SIZE) - (PtrSize_u << 1);
            if (size < 0)
                return -1;

            TypedReference tr = __makeref(value);
            int* ptr =
                t.IsValueType
                ? *(int**)&tr
                : (**(int***)&tr + PtrSize_i);

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
                ? 31 * result + *last
                : result;
        }

        public static T ReadMemoryValue<T>(object instance, int offset) where T : unmanaged
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));

            Type t = instance.GetType();
            int size = *((int*)t.TypeHandle.Value + X86BF_PTR_SIZE) - (PtrSize_i << 1);
            if (offset < 0 || offset + sizeof(T) > size)
                throw new ArgumentOutOfRangeException(nameof(offset));

            TypedReference tr = __makeref(instance);
            return *(T*)(**(byte***)&tr + (PtrSize_i + offset));
        }

        public static T ReadMemoryValue_unsafe<T>(object instance, int offset) where T : unmanaged
        {
            TypedReference tr = __makeref(instance);
            return *(T*)(**(byte***)&tr + (PtrSize_i + offset));
        }

        public static ref TRet RefMemoryValue<TRet, TInst>(ref TInst instance, int offset) where TRet : unmanaged
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));

            Type t = typeof(TInst);
            int size = *((int*)t.TypeHandle.Value + X86BF_PTR_SIZE) - (PtrSize_i << 1);
            if (offset < 0 || offset + sizeof(TRet) > size)
                throw new ArgumentOutOfRangeException(nameof(offset));

            TypedReference tr = __makeref(instance);
#pragma warning disable
            if (t.IsValueType)
                return ref *(TRet*)(*(byte**)&tr + offset);
            else
                return ref *(TRet*)(**(byte***)&tr + (PtrSize_i + offset));
#pragma warning restore
        }

        public static ref TRet RefMemoryValue_unsafe<TRet, TInst>(ref TInst instance, int offset) where TRet : unmanaged
        {
            TypedReference tr = __makeref(instance);
#pragma warning disable
            if (typeof(TInst).IsValueType)
                return ref *(TRet*)(*(byte**)&tr + offset);
            else
                return ref *(TRet*)(**(byte***)&tr + (PtrSize_i + offset));
#pragma warning restore
        }

        #region internal
        internal static int Internal_MemCompareTo_Un_S<T>(ref T left, ref T right)
        {
            int isize = *((int*)typeof(T).TypeHandle.Value + X86BF_PTR_SIZE) - (PtrSize_i << 1);

            if (isize > 0)
            {
                uint size = (uint)isize;

                byte[] left_mem = new byte[isize];
                byte[] right_mem = new byte[isize];

                TypedReference left_tr = __makeref(left);
                TypedReference right_tr = __makeref(right);

                fixed (byte* pleft = left_mem)
                fixed (byte* pright = right_mem)
                {
                    memcpblk.Invoke(*(void**)&left_tr, pleft, size);
                    memcpblk.Invoke(*(void**)&right_tr, pright, size);

                    return Internal_CompareTo_Un_S(pleft, pright, size);
                }
            }

            return 0;
        }

        internal static int Internal_MemCompareTo<T>(ref T left, ref T right)
        {
            int isize = *((int*)typeof(T).TypeHandle.Value + X86BF_PTR_SIZE) - (PtrSize_i << 1);

            if (isize > 0)
            {
                uint size = (uint)isize;

                byte[] left_mem = new byte[isize];
                byte[] right_mem = new byte[isize];

                TypedReference left_tr = __makeref(left);
                TypedReference right_tr = __makeref(right);

                fixed (byte* pleft = left_mem)
                fixed (byte* pright = right_mem)
                {
                    memcpblk.Invoke(*(void**)&left_tr, pleft, size);
                    memcpblk.Invoke(*(void**)&right_tr, pright, size);

                    return Internal_CompareTo(pleft, pright, size);
                }
            }

            return 0;
        }

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
                count -= AMD64_PTR_SIZE;
            }

            byte* ableft = (byte*)upleft;
            byte* abright = (byte*)upright;
            if (count >= IA32_PTR_SIZE)
            {
                if (*(uint*)ableft != *(uint*)abright)
                    return (*(uint*)ableft).CompareTo(*(uint*)abright);

                count -= IA32_PTR_SIZE;

                ableft -= IA32_PTR_SIZE;
                abright -= IA32_PTR_SIZE;
            }
            if (count >= IA16_PTR_SIZE)
            {
                if (*(ushort*)ableft != *(ushort*)abright)
                    return *(ushort*)ableft - *(ushort*)abright;

                count -= IA16_PTR_SIZE;

                ableft -= IA16_PTR_SIZE;
                abright -= IA16_PTR_SIZE;
            }
            return
                count >= X86BF_PTR_SIZE && *ableft != *abright
                ? *ableft - *abright
                : 0;
        }

        internal static int Internal_CompareTo(byte* pleft, byte* pright, uint count)
        {
            count--;
            sbyte* uplf = (sbyte*)pleft + count, uprt = (sbyte*)pright + count;
            return
                *uplf != *uprt
                ? *uplf - *uprt
                : Internal_CompareTo_Un_S(pleft, pright, count);
        }

        internal static void Internal_memcpyff(byte* src, uint srcOffset, byte* dst, uint dstOffset, uint count)
        {
            ulong* usrc = (ulong*)(src + srcOffset);
            ulong* udst = (ulong*)(dst + dstOffset);
            while (count >= AMD64_PTR_SIZE)
            {
                *udst = *usrc;
                usrc++;
                udst++;
                count -= AMD64_PTR_SIZE;
            }

            byte* absrc = (byte*)usrc;
            byte* abdst = (byte*)udst;
            if (count >= IA32_PTR_SIZE)
            {
                *(uint*)abdst = *(uint*)absrc;
                count -= IA32_PTR_SIZE;

                abdst += IA32_PTR_SIZE;
                absrc += IA32_PTR_SIZE;
            }
            if (count >= IA16_PTR_SIZE)
            {
                *(ushort*)dst = *(ushort*)src;
                count -= IA16_PTR_SIZE;

                abdst += IA16_PTR_SIZE;
                absrc += IA16_PTR_SIZE;
            }
            if (count == X86BF_PTR_SIZE)
                *dst = *src;
        }

        internal static uint Internal_SizeOf(Type type) => ((uint*)type.TypeHandle.Value)[1];

        internal static void Internal_ReadMem(Type t, void* psrc, void* pdst, uint size)
        {
            if (t.IsValueType)
                memcpblk.Invoke(*(void**)psrc, pdst, size);
            else
                memcpblk.Invoke(**(byte***)psrc + PtrSize_i, pdst, size);
        }

        internal static void Internal_ReadMem(Type t, void* psrc, void* pdst, uint begin, uint size)
        {
            if (t.IsValueType)
                memcpblk.Invoke(*(byte**)psrc + begin, pdst, size);
            else
                memcpblk.Invoke(**(byte***)psrc + PtrSize_i + begin, pdst, size);
        }

        internal static void Internal_WriteMem(Type t, void* psrc, void* pdst, uint begin, uint size)
        {
            if (t.IsValueType)
                memcpblk.Invoke(psrc, *(byte**)pdst + begin, size);
            else
                memcpblk.Invoke(psrc, **(byte***)pdst + PtrSize_i + begin, size);
        }

        internal static void Internal_Zeromem(byte* pval, uint size)
        {
            ulong* pUL = (ulong*)pval;
            while (size >= AMD64_PTR_SIZE)
            {
                *pUL = 0;
                pUL--;
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
        #endregion
    }
}
