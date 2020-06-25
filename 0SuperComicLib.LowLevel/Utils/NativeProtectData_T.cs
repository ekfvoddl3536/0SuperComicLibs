using System;
using System.Runtime.InteropServices;

namespace SuperComicLib.LowLevel
{
    using static NativeHeapMgr;
    using static NativeMehods0;

    public sealed unsafe class NativeProtectData<T> : IDisposable 
        where T : unmanaged
    {
        private T* _lpaddr;
        private IntPtr _dwsize;

        public NativeProtectData(T value)
        {
            if (Environment.OSVersion.Platform >= PlatformID.Unix)
                throw new InvalidOperationException();

            _dwsize = (IntPtr)sizeof(T);
            _lpaddr = (T*)Marshal.AllocHGlobal(_dwsize);
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
            if (_lpaddr != null)
            {
                VirtualProtect(_lpaddr, _dwsize, PAGE_READWRITE, out _);
                _dwsize = IntPtr.Zero;
                Marshal.FreeHGlobal(new IntPtr(_lpaddr));
                _lpaddr = null;
            }
        }
    }
}
