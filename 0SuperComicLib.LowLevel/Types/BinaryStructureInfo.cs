using System;
using System.Security.Permissions;

namespace SuperComicLib.LowLevel
{
    [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
    public unsafe sealed class BinaryStructureInfo : IDisposable
    {
        private byte[] datas;

        public BinaryStructureInfo(byte[] memory)
        {
            if (memory == null)
                throw new ArgumentNullException(nameof(memory));
            if (memory.Length < IntPtr.Size * 3) // this 포인터 + 필드 시작 (정렬됨) + 엔드 포인터(?) intptr * 3
                throw new InvalidOperationException(nameof(memory));
            fixed (byte* ptr = memory)
            {
                This = (IntPtr)ptr;

                datas = new byte[memory.Length - IntPtr.Size * 2]; // 필드를 제외한 2개의 포인터 공간 제거
                int len = datas.Length;

                fixed (byte* bdptr = datas)
                {
                    ulong* src = (ulong*)(ptr + IntPtr.Size); // this 다음부터
                    ulong* dst = (ulong*)bdptr;
                    while (len >= NativeClass.AMD64_PTR_SIZE)
                    {
                        *dst = *src;
                        src++;
                        dst++;
                        len -= NativeClass.AMD64_PTR_SIZE;
                    }
                    if (len == NativeClass.IA32_PTR_SIZE)
                        *(uint*)dst = *(uint*)src;
                    Blank = *(IntPtr*)src;
                }
            }
            
        }

        public int Length => datas.Length;

        public IntPtr This { get; }

        public IntPtr Blank { get; }

        public byte[] ToArray(bool fieldOnly = false)
        {
            if (fieldOnly)
            {
                int size = datas.Length;
                byte[] vs = new byte[size];
                Buffer.BlockCopy(datas, 0, vs, 0, size);
                return vs;
            }
            return ToArrayAll();
        }

        private byte[] ToArrayAll()
        {
            byte[] vs = new byte[datas.Length + IntPtr.Size * 2];
            int len = datas.Length;

            fixed (byte* ptr = vs)
            fixed (byte* bdptr = datas)
            {
                *(IntPtr*)ptr = This;
                ulong* src = (ulong*)bdptr;
                ulong* dst = (ulong*)(ptr + IntPtr.Size);
                while (len >= NativeClass.AMD64_PTR_SIZE)
                {
                    *dst = *src;
                    src++;
                    dst++;
                    len -= NativeClass.AMD64_PTR_SIZE;
                }
                if (len == NativeClass.IA32_PTR_SIZE)
                    *(uint*)dst = *(uint*)src;
                *(IntPtr*)dst = Blank;
            }

            return vs;
        }

        public ref byte this[int idx] => ref datas[idx];

        public void FixedPointer(UnsafePointerAction<byte> action)
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
            {
                NativeClass.Internal_memcpyff(ptr, *(uint*)&idx, dst, 0, *(uint*)&count);
            }
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
            {
                NativeClass.Internal_memcpyff(ptr, 0, ptr2, *(uint*)&idx, (uint)bytes.Length);
            }
        }

        public T Read<T>(int idx) where T : struct
        {
            if (idx < 0 || datas.Length <= idx)
                throw new ArgumentOutOfRangeException(nameof(idx));
            UnsafeReadPointerStruct<T> rp = NativeClass.CreateReadPointer<T>(typeof(BinaryStructureInfo));
            fixed (byte* ptr = datas)
            {
                byte* src = ptr + idx;
                T res = rp.Invoke(src);
                return res;
            }
        }

        public T Cast<T>(int idx) where T : class
        {
            if (idx < 0 || datas.Length <= idx)
                throw new ArgumentOutOfRangeException(nameof(idx));
            UnsafeCastClass<T> cc = NativeClass.CreateCastClass<T>(typeof(BinaryStructureInfo));
            fixed (byte* ptr = datas)
            {
                byte* src = ptr + idx;
                T res = cc.Invoke(src);
                return res;
            }
        }

        public void Write<T>(T value, int idx) where T : struct
        {
            if (idx < 0 || datas.Length <= idx)
                throw new ArgumentOutOfRangeException(nameof(idx));
            byte[] vs = NativeClass.ReadMemory_s(ref value);
            fixed (byte* src = vs)
            fixed (byte* dst = datas)
            {
                NativeClass.Memcpy(src, *(uint*)&idx, dst, 0U, (uint)vs.Length);
            }
        }

        public void Write<T>(ref T rvalue, int idx) where T : struct
        {
            if (idx < 0 || datas.Length <= idx)
                throw new ArgumentOutOfRangeException(nameof(idx));
            byte[] vs = NativeClass.ReadMemory_s(ref rvalue);
            fixed (byte* src = vs)
            fixed (byte* dst = datas)
            {
                NativeClass.Memcpy(src, *(uint*)&idx, dst, 0U, (uint)vs.Length);
            }
        }

        public void Dispose() => datas = null;
    }
}
