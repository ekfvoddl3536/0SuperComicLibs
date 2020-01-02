using System;

namespace SuperComicLib.LowLevel
{
    using static NativeHeapMgr;
    using static NativeMehods0;

    public sealed unsafe class NativeProtectData<T> : IDisposable where T : unmanaged
    {
        private T* _lpaddr;
        private IntPtr _dwsize;

        public NativeProtectData(T value)
        {
            _dwsize = (IntPtr)sizeof(T);
            _lpaddr = (T*)VirtualAlloc(null, _dwsize, MEM_RESERVE | MEM_COMMIT, PAGE_READWRITE);
            if (_lpaddr == null)
                throw new InsufficientMemoryException();
            *_lpaddr = value;
            VirtualProtect(_lpaddr, _dwsize, PAGE_NOACCESS, out _);
        }

        public T Value
        {
            get
            {
                VirtualProtect(_lpaddr, _dwsize, PAGE_READONLY, out _);
                T result = *_lpaddr;
                VirtualProtect(_lpaddr, _dwsize, PAGE_NOACCESS, out _);
                return result;
            }
            set
            {
                VirtualProtect(_lpaddr, _dwsize, PAGE_READWRITE, out _);
                *_lpaddr = value;
                VirtualProtect(_lpaddr, _dwsize, PAGE_NOACCESS, out _);
            }
        }

        public static bool operator ==(NativeProtectData<T> left, NativeProtectData<T> right) => left._lpaddr == right._lpaddr;
        public static bool operator !=(NativeProtectData<T> left, NativeProtectData<T> right) => left._lpaddr != right._lpaddr;

        public override bool Equals(object obj) => false;

        public override int GetHashCode() => base.GetHashCode();

        public void Dispose()
        {
            VirtualProtect(_lpaddr, _dwsize, PAGE_READWRITE, out _);
            VirtualFree(_lpaddr, _dwsize, MEM_RELEASE);
            _lpaddr = null;
            _dwsize = IntPtr.Zero;
        }
    }
}
