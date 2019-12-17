using System;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace SuperComicLib.LowLevel
{
    public unsafe static class NativeClass
    {
        public const int X86BF_PTR_SIZE = 1;
        public const int IA16_PTR_SIZE = X86BF_PTR_SIZE << 1;
        public const int IA32_PTR_SIZE = IA16_PTR_SIZE << 1;
        public const int AMD64_PTR_SIZE = IA32_PTR_SIZE << 1;

        private static readonly UnsafePinnedObjectAsIntPtr pinnedptr = CreatePinnedPtr(typeof(NativeClass));
        private static readonly UnsafeMemoryCopyBlock memcpblk = CreateMemcpblk();

        private static UnsafeMemoryCopyBlock CreateMemcpblk()
        {
            Type vp = typeof(void).MakePointerType();
            DynamicMethod dm = new DynamicMethod($"__MEMCPBLK__", typeof(void), new[] { vp, vp, typeof(uint) }, typeof(NativeClass));
            ILGenerator g = dm.GetILGenerator();

            g.DeclareLocal(typeof(object), true);
            g.Emit(OpCodes.Ldarg_1);
            g.Emit(OpCodes.Ldarg_0);
            g.Emit(OpCodes.Ldarg_2);
            g.Emit(OpCodes.Cpblk);
            g.Emit(OpCodes.Ret);

            return (UnsafeMemoryCopyBlock)dm.CreateDelegate(typeof(UnsafeMemoryCopyBlock));
        }

        public static UnsafePinnedObjectAsIntPtr CreatePinnedPtr(Type owner)
        {
            DynamicMethod dm = new DynamicMethod($"__func_PinnedPtr ({nameof(NativeClass)}) ({owner.ToString()})", typeof(void), new[] { typeof(object), typeof(Action<IntPtr>) }, owner);
            ILGenerator g = dm.GetILGenerator();

            g.DeclareLocal(typeof(object), true);
            g.Emit(OpCodes.Ldarg_0);
            g.Emit(OpCodes.Stloc_0);
            g.Emit(OpCodes.Ldarg_1);
            g.Emit(OpCodes.Ldloc_0);
            g.Emit(IntPtr.Size == sizeof(int) ? OpCodes.Conv_I4 : OpCodes.Conv_I8);
            g.Emit(OpCodes.Call, typeof(Action<IntPtr>).GetMethod(nameof(Action.Invoke)));
            g.Emit(OpCodes.Ret);

            return (UnsafePinnedObjectAsIntPtr)dm.CreateDelegate(typeof(UnsafePinnedObjectAsIntPtr));
        }

        public static UnsafeCastClass<T> CreateCastClass<T>(Type owner) where T : class
        {
            DynamicMethod dm = new DynamicMethod($"__func_CastClass ({nameof(NativeClass)}) ({owner.ToString()})", typeof(T), new[] { typeof(void).MakePointerType() }, owner);
            ILGenerator gen = dm.GetILGenerator();

            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Castclass, typeof(T));
            gen.Emit(OpCodes.Ret);

            return (UnsafeCastClass<T>)dm.CreateDelegate(typeof(UnsafeCastClass<T>));
        }

        public static UnsafeUnboxStruct<T> CreateUnbox<T>(Type owner) where T : struct
        {
            DynamicMethod dm = new DynamicMethod($"__func_Unbox ({nameof(NativeClass)}) ({owner.ToString()})", typeof(T), new[] { typeof(object) }, owner);
            ILGenerator gen = dm.GetILGenerator();

            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Unbox, typeof(T));
            gen.Emit(OpCodes.Ret);

            return (UnsafeUnboxStruct<T>)dm.CreateDelegate(typeof(UnsafeUnboxStruct<T>));
        }

        public static UnsafeReadPointerStruct<T> CreateReadPointer<T>(Type owner) where T : struct
        {
            DynamicMethod dm = new DynamicMethod($"__func_ReadPointer ({nameof(NativeClass)}) ({owner.ToString()})", typeof(T), new[] { typeof(void).MakePointerType() }, owner);
            ILGenerator gen = dm.GetILGenerator();

            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Ldobj, typeof(T));
            gen.Emit(OpCodes.Ret);

            return (UnsafeReadPointerStruct<T>)dm.CreateDelegate(typeof(UnsafeReadPointerStruct<T>));
        }

        public static To Convert<From, To>(ref From fm)
            where From : unmanaged
            where To : unmanaged
        {
            fixed (From* ptr = &fm)
                return *(To*)ptr;
        }

        public static To Convert_s<From, To>(ref From fm)
            where From : unmanaged
            where To : unmanaged
        {
            if (sizeof(From) != sizeof(To))
                return default;
            fixed (From* ptr = &fm)
                return *(To*)ptr;
        }

        public static To Convert_cs<From, To>(From fm) where From : unmanaged where To : unmanaged
            => sizeof(From) != sizeof(To) ? default : *(To*)&fm;

        public static IntPtr GetAddr(object obj)
        {
            if (obj == null)
                return IntPtr.Zero;
            TypedReference tr = __makeref(obj);
            return **(IntPtr**)&tr;
        }

        public static IntPtr GetAddr<T>(ref T obj)
        {
            if (obj == null)
                return IntPtr.Zero;
            TypedReference tr = __makeref(obj);
            return **(IntPtr**)&tr;
        }

        public static IntPtr GetAddr<T>(ref T[] array)
        {
            if (array == null || array.Length == 0)
                return IntPtr.Zero;
            TypedReference tr = __makeref(array[0]);
            return **(IntPtr**)&tr;
        }

        public static void PinnedAddr(object obj, Action<IntPtr> callback)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));
            if (callback == null)
                throw new ArgumentNullException(nameof(callback));
            pinnedptr.Invoke(obj, callback);
        }

        public static void PinnedAddr<T>(ref T obj, Action<IntPtr> callback)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));
            if (callback == null)
                throw new ArgumentNullException(nameof(callback));
            pinnedptr.Invoke(obj, callback);
        }

        public static void PinnedAddr<T>(ref T[] array, Action<IntPtr> callback)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));
            if (callback == null)
                throw new ArgumentNullException(nameof(callback));
            if (array.Length == 0)
                throw new InvalidOperationException(nameof(array));
            ref T obj = ref array[0];
            pinnedptr.Invoke(obj, callback);
        }

        public static void PinnedAddrss<T>(ref T[] array, Action<IntPtr> callback)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));
            if (callback == null)
                throw new ArgumentNullException(nameof(callback));
            for (int x = 0, max = array.Length; x < max; x++)
            {
                ref T obj = ref array[x];
                pinnedptr.Invoke(obj, callback);
            }
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public static void RefMemory<T>(ref T obj, UnsafePointerAction<byte> cb)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));
            if (cb == null)
                throw new ArgumentNullException(nameof(cb));

            uint size = *((uint*)obj.GetType().TypeHandle.Value + X86BF_PTR_SIZE);

            if (typeof(T).IsValueType)
            {
                byte[] tempb = new byte[size];

                Type t = typeof(T);
                int len = Marshal.SizeOf(t);
                IntPtr ptr = Marshal.AllocHGlobal(len);

                pinnedptr.Invoke(obj, original =>
                {
                    fixed (byte* cbptr = tempb)
                    {
                        memcpblk.Invoke(original.ToPointer(), cbptr, size);
                        cb.Invoke(cbptr);
                    }
                    Marshal.Copy(tempb, IntPtr.Size, ptr, len);
                });

                obj = (T)Marshal.PtrToStructure(ptr, t);
                Marshal.FreeHGlobal(ptr);
            }
            else
                pinnedptr.Invoke(obj, ptr => cb.Invoke((byte*)ptr));
        }

        public static byte[] ReadMemory<T>(ref T obj)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            uint size = *((uint*)obj.GetType().TypeHandle.Value + X86BF_PTR_SIZE);

            byte[] res = new byte[size];
            pinnedptr.Invoke(obj, original =>
            {
                fixed (byte* ptr = res)
                {
                    memcpblk.Invoke(original.ToPointer(), ptr, size);
                }
            });
            return res;
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public static byte[] ReadMemory_s<T>(ref T obj) where T : new()
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
                int[] vs = decimal.GetBits(f3);
                byte[] res = new byte[sizeof(decimal)];
                fixed (byte* temp = res)
                fixed (int* src = vs)
                {
                    int* dst = (int*)temp;
                    dst[0] = src[0];
                    dst[1] = src[1];
                    dst[2] = src[2];
                    dst[3] = src[3];
                }
                return res;
            }
            else if (obj is string str)
            {
                byte[] res = new byte[str.Length * 2];
                fixed (char* ptr = str)
                {
                    Marshal.Copy((IntPtr)ptr, res, 0, res.Length);
                }
                return res;
            }
            else
            {
                int size = *((int*)obj.GetType().TypeHandle.Value + X86BF_PTR_SIZE) - (IntPtr.Size << 1);

                byte[] res = new byte[size];
                pinnedptr.Invoke(obj, original =>
                {
                    fixed (byte* ptr = res)
                    {
                        byte* src = (byte*)original;
                        src += IntPtr.Size;

                        memcpblk.Invoke(src, ptr, (uint)size);
                    }
                });
                return res;
            }
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public static void WriteMemory<T>(ref T dest, byte[] src) where T : new()
        {
            if (dest == null)
                throw new ArgumentNullException(nameof(dest));

            int psize = *((int*)dest.GetType().TypeHandle.Value + X86BF_PTR_SIZE);
            if (src == null || src.Length < psize)
                throw new InvalidOperationException(nameof(src));

            if (typeof(T).IsValueType)
            {
                Type t = typeof(T);
                int len = Marshal.SizeOf(t);
                IntPtr ptr = Marshal.AllocHGlobal(len);
                Marshal.Copy(src, IntPtr.Size, ptr, len);
                dest = (T)Marshal.PtrToStructure(ptr, t);
                Marshal.FreeHGlobal(ptr);
            }
            else
                pinnedptr.Invoke(dest, ptr =>
                {
                    fixed (byte* sp = src)
                    {
                        memcpblk.Invoke(sp, ptr.ToPointer(), (uint)psize);
                    }
                });
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public static void WriteMemory<T>(ref T dest, byte[] src, int srcOffset) where T : new()
        {
            if (dest == null)
                throw new ArgumentNullException(nameof(dest));
            if (srcOffset < 0)
                throw new ArgumentOutOfRangeException(nameof(srcOffset));

            int psize = *((int*)dest.GetType().TypeHandle.Value + X86BF_PTR_SIZE);
            if (src == null || src.Length < srcOffset + psize)
                throw new InvalidOperationException(nameof(src));

            if (typeof(T).IsValueType)
            {
                Type t = typeof(T);
                int len = Marshal.SizeOf(t);
                IntPtr ptr = Marshal.AllocHGlobal(len);
                Marshal.Copy(src, IntPtr.Size + srcOffset, ptr, len);
                dest = (T)Marshal.PtrToStructure(ptr, t);
                Marshal.FreeHGlobal(ptr);
            }
            else
                pinnedptr.Invoke(dest, ptr =>
                {
                    fixed (byte* sp = src)
                    {
                        memcpblk.Invoke(sp + srcOffset, ptr.ToPointer(), (uint)psize);
                    }
                });
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

            memcpblk.Invoke((byte*)srcPtr + srcOffset, (byte*)dstPtr + dstOffset, *(uint*)&count);
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
            if (count == IA32_PTR_SIZE)
                *(uint*)dst = *(uint*)src;
        }
    }
}
