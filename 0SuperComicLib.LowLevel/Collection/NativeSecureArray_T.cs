using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using SuperComicLib.LowLevel;

namespace SuperComicLib.Collection
{
    using static NativeMehods0;
    using static NativeHeapMgr;

    [StructLayout(LayoutKind.Sequential)]
    public sealed unsafe class NativeSecureArray<T> : IDisposable, IEnumerable<T> where T : unmanaged
    {
        private T* _lpaddr;
        private IntPtr _dwsize;
        private IntPtr _typesize;

        public NativeSecureArray(int length)
        {
            if (length <= 0)
                throw new InvalidOperationException();
            _typesize = (IntPtr)sizeof(T);
            _dwsize = new IntPtr(_typesize.ToInt32() * length);
            _lpaddr = (T*)VirtualAlloc(null, _dwsize, MEM_RESERVE | MEM_COMMIT, PAGE_READWRITE);
            if (_lpaddr == null)
                throw new InsufficientMemoryException();
            VirtualLock(_lpaddr, _dwsize);
            VirtualProtect(_lpaddr, _dwsize, PAGE_NOACCESS, out _);
        }

        public NativeSecureArray(T[] source)
        {
            if (source == null)
                throw new ArgumentNullException();
            _typesize = (IntPtr)sizeof(T);
            _dwsize = new IntPtr(_typesize.ToInt32() * source.Length);
            if (_dwsize == IntPtr.Zero)
                throw new InvalidOperationException();
            _lpaddr = (T*)VirtualAlloc(null, _dwsize, MEM_RESERVE | MEM_COMMIT, PAGE_READWRITE);
            if (_lpaddr == null)
                throw new InsufficientMemoryException();
            VirtualLock(_lpaddr, _dwsize);
            for (int x = source.Length - 1; x >= 0; x--)
                _lpaddr[x] = source[x];
            VirtualProtect(_lpaddr, _dwsize, PAGE_NOACCESS, out _);
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (int x = 0, len = Length; x < len; x++)
                yield return this[x];
            yield break;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public T[] ToArray()
        {
            T[] vs = new T[Length];
            VirtualProtect(_lpaddr, _dwsize, PAGE_READWRITE, out _);
            fixed (T* ptr = vs)
                NativeClass.Internal_memcpyff((byte*)_lpaddr, 0, (byte*)ptr, 0, (uint)_dwsize);
            VirtualProtect(_lpaddr, _dwsize, PAGE_NOACCESS, out _);
            return vs;
        }

        public void CopyTo(NativeSecureArray<T> dest)
        {
            if (dest == null)
                throw new ArgumentNullException();
            for (int x = Math.Min(dest.Length, Length) - 1; x >= 0; x--)
                dest._lpaddr[x] = _lpaddr[x];
        }

        public int Length => _dwsize.ToInt32() / _typesize.ToInt32();

        public long LongLength => _dwsize.ToInt64() / _typesize.ToInt64();

        public T this[int idx]
        {
            get
            {
                T* ptr = _lpaddr + idx;
                VirtualProtect(ptr, _typesize, PAGE_READONLY, out _);
                T res = *ptr;
                VirtualProtect(ptr, _typesize, PAGE_NOACCESS, out _);
                return res;
            }

            set
            {
                T* ptr = _lpaddr + idx;
                VirtualProtect(ptr, _typesize, PAGE_READWRITE, out _);
                *ptr = value;
                VirtualProtect(ptr, _typesize, PAGE_NOACCESS, out _);
            }
        }

        ~NativeSecureArray() => Dispose();

        public void Dispose()
        {
            VirtualProtect(_lpaddr, _dwsize, PAGE_READWRITE, out _);
            VirtualUnlock(_lpaddr, _dwsize);
            VirtualFree(_lpaddr, _dwsize, MEM_RELEASE);
            _lpaddr = null;
            _dwsize = IntPtr.Zero;
            _typesize = IntPtr.Zero;

            GC.SuppressFinalize(this);
        }
    }
}
