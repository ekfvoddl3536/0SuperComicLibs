using System;
using System.Security;
using System.Security.Permissions;

namespace SuperComicLib.LowLevel
{
    [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
    public sealed unsafe class BinaryStructureInfo : IDisposable
    {
        private byte[] datas;
        private IntPtr m_typehnd;

        public BinaryStructureInfo(UnsafeCLIMemoryData memdata)
        {
            if (memdata.IsEmpty)
                throw new ArgumentNullException(nameof(memdata));

            m_typehnd = memdata.typehnd;
            datas = memdata.memory;
        }

        public int Length => datas.Length;

        public IntPtr Syncblock => *(IntPtr*)((byte*)m_typehnd - NativeClass.PtrSize_i);

        public IntPtr TypeHandle => m_typehnd;

        public PubMethodTable MethodTable => **(PubMethodTable**)m_typehnd;

        public byte[] ToArray()
        {
            int size = datas.Length;
            byte[] vs = new byte[size];
            Array.Copy(datas, 0, vs, 0, size);
            return vs;
        }

        public ref byte this[int idx] => ref datas[idx];

        public void Pinned(UnsafePointerAction action)
        {
            fixed (byte* ptr = datas)
                action.Invoke(ptr);
        }

        public byte[] ReadBytes(int idx, int count)
        {
            if (count < 0 || idx < 0 || datas.Length <= idx + count)
                throw new ArgumentOutOfRangeException();
            byte[] nres = new byte[count];
            fixed (byte* ptr = datas)
            fixed (byte* dst = nres)
                NativeClass.Internal_memcpyff(ptr, 0U, dst, (uint)idx, (uint)count);
            return nres;
        }

        public void WriteBytes(int idx, byte[] bytes)
        {
            if (bytes == null)
                throw new ArgumentNullException(nameof(bytes));
            if (idx < 0 || datas.Length <= idx + bytes.Length)
                throw new ArgumentOutOfRangeException(nameof(idx));
            fixed (byte* ptr = bytes)
            fixed (byte* ptr2 = datas)
                NativeClass.Internal_memcpyff(ptr, 0, ptr2, (uint)idx, (uint)bytes.Length);
        }

        [SecurityCritical]
        public T Read<T>(int idx) where T : struct
        {
            if (idx < 0 || datas.Length <= idx)
                throw new ArgumentOutOfRangeException(nameof(idx));
            NativeStruct<T> rp = NativeStruct<T>.Instance;
            fixed (byte* ptr = datas)
                return rp.Read(ptr + idx);
        }

        public ref T Ref<T>(int idx) where T : unmanaged
        {
            if (idx < 0 || datas.Length <= idx)
                throw new ArgumentOutOfRangeException(nameof(idx));
            fixed (byte* ptr = datas)
                return ref *(T*)(ptr + idx);
        }

        [SecurityCritical]
        public T Cast<T>(int idx) where T : class
        {
            if (idx < 0 || datas.Length <= idx)
                throw new ArgumentOutOfRangeException(nameof(idx));
            NativeClass<T> cc = NativeClass<T>.Instance;
            fixed (byte* ptr = datas)
                return cc.Cast(ptr + idx);
        }

        public void Write<T>(T value, int idx) where T : struct
        {
            if (idx < 0 || datas.Length <= idx)
                throw new ArgumentOutOfRangeException(nameof(idx));

            byte[] vs = NativeClass.ReadMemory_s(ref value);
            fixed (byte* src = vs)
            fixed (byte* dst = datas)
                NativeClass.memcpblk.Invoke(src, dst + idx, (uint)vs.Length);
        }

        public void Write<T>(ref T rvalue, int idx) where T : struct
        {
            if (idx < 0 || datas.Length <= idx)
                throw new ArgumentOutOfRangeException(nameof(idx));
            byte[] vs = NativeClass.ReadMemory_s(ref rvalue);
            fixed (byte* src = vs)
            fixed (byte* dst = datas)
                NativeClass.memcpblk.Invoke(src, dst + idx, (uint)vs.Length);
        }

        public void WriteRefval(object value, int idx)
        {
            if (idx < 0 || datas.Length <= idx)
                throw new ArgumentOutOfRangeException(nameof(idx));

            TypedReference tr = __makeref(value);
            fixed (byte* dst = datas)
                *(IntPtr*)(dst + idx) = **(IntPtr**)&tr;
        }

        public void Dispose()
        {
            if (datas != null)
            {
                datas = null;
                m_typehnd = IntPtr.Zero;
            }
        }
    }
}
