using System;
using System.Runtime.InteropServices;

namespace SuperComicLib.LowLevel
{
    using static NativeMehods0;
    public sealed unsafe class NativeHeapMgr : IDisposable
    {
        internal const uint
            PAGE_NOACCESS = 0x01,
            PAGE_READONLY = 0x02,
            PAGE_READWRITE = 0x04,
            MEM_RESERVE = 0x2000,
            MEM_COMMIT = 0x1000,
            MEM_RELEASE = 0x8000;

        private void* _lpaddr;
        private IntPtr _size;

        public NativeHeapMgr(int bytes) : this((long)bytes) { }

        public NativeHeapMgr(long bytes)
        {
            if (bytes <= 0 || Environment.OSVersion.Platform >= PlatformID.Unix)
                throw new InvalidOperationException();

            _size = new IntPtr(bytes);
            _lpaddr = (void*)Marshal.AllocHGlobal(_size);
        }

        #region 속성
        public int Size => _size.ToInt32();

        public long LongSize => _size.ToInt64();
        #endregion

        #region 구조체 읽기/쓰기
        public T ReadValue<T>(int offset) where T : unmanaged => *(T*)this[offset];

        public ref T RefValue<T>(int offset) where T : unmanaged => ref *(T*)this[offset];

        public void WriteValue<T>(int offset, T value) where T : unmanaged => *(T*)this[offset] = value;
        #endregion

        #region 주소 읽기/쓰기
        public T ReadAddrs<T>(int offset) where T : class => 
            NativeClass.CreateCastClass<T>(typeof(NativeHeapMgr)).Invoke((*(IntPtr*)this[offset]).ToPointer());
        
        public void WriteAddrs(int offset, object value) => 
            NativeClass.PinnedAddr(value, ptr => *(IntPtr*)this[offset] = ptr);
        #endregion

        #region 로우레벨
        public void SetBlock(int offset, byte[] datas)
        {
            if (datas == null)
                throw new ArgumentNullException();
            if (offset + datas.Length > Size)
                throw new ArgumentOutOfRangeException();
            fixed (byte* ptr = datas)
                NativeClass.Internal_memcpyff(ptr, 0, this[offset], 0, (uint)datas.Length);
        }

        public byte[] ReadBlock(int offset, int count)
        {
            if (offset < 0)
                throw new InvalidOperationException();
            if (offset + count > Size)
                throw new ArgumentOutOfRangeException();
            byte[] res = new byte[count];
            fixed (byte* ptr = res)
                NativeClass.Internal_memcpyff(this[offset], 0, ptr, 0, *(uint*)&count);
            return res;
        }

        public void Free(int offset, int count)
        {
            if (offset < 0)
                throw new InvalidOperationException();
            if (offset + count > Size)
                throw new ArgumentOutOfRangeException();
            RtlZeroMemory(this[offset], new IntPtr(count));
        }
        #endregion

        #region 유틸
        public byte* this[int offset] => (byte*)_lpaddr + offset;

        public byte* MemoryBlock() => (byte*)_lpaddr;

        public T* AsPointer<T>() where T : unmanaged => (T*)_lpaddr;

        public T* AsPointer<T>(int offset) where T : unmanaged => (T*)this[offset];

        public IntPtr ToIntPtr() => (IntPtr)_lpaddr;
        #endregion

        #region 소멸자
        ~NativeHeapMgr() => Dispose();

        public void Dispose()
        {
            _size = IntPtr.Zero;

            Marshal.FreeHGlobal(new IntPtr(_lpaddr));
            _lpaddr = null;

            GC.SuppressFinalize(this);
        }
        #endregion

        #region 보안
        public ProtectedSecureHeapResult Protect()
        {
            VirtualProtect(_lpaddr, _size, PAGE_NOACCESS, out _);
            return new ProtectedSecureHeapResult(this);
        }

        public ReadonlyNoSecureHeapResult Readonly()
        {
            VirtualProtect(_lpaddr, _size, PAGE_READONLY, out _);
            return new ReadonlyNoSecureHeapResult(this);
        }
        #endregion

        #region 확장
#pragma warning disable
        public struct ProtectedSecureHeapResult
        {
            private NativeHeapMgr mgr;

            internal ProtectedSecureHeapResult(NativeHeapMgr mgr) => this.mgr = mgr;

            public void Unprotect()
            {
                VirtualProtect(mgr._lpaddr, mgr._size, PAGE_READWRITE, out _);
                mgr = null;
            }
        }

        public struct ReadonlyNoSecureHeapResult
        {
            private NativeHeapMgr mgr;

            internal ReadonlyNoSecureHeapResult(NativeHeapMgr mgr) => this.mgr = mgr;

            public void Release()
            {
                VirtualProtect(mgr._lpaddr, mgr._size, PAGE_READWRITE, out _);
                mgr = null;
            }
        }
#pragma warning restore
        #endregion
    }
}
