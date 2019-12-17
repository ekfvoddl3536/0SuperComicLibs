using System;

namespace SuperComicLib.LowLevel
{
    using static NativeMehods0;
    public sealed unsafe class NativeHeapMgr : IDisposable
    {
        internal const uint
            PAGE_READWRITE = 0x04,
            MEM_RESERVE = 0x2000,
            MEM_COMMIT = 0x1000,
            MEM_RELEASE = 0x8000;

        private void* lpmem;

        public NativeHeapMgr(int size)
        {
            lpmem = VirtualAlloc(null, size, MEM_RESERVE | MEM_COMMIT, PAGE_READWRITE);
            if (lpmem == null)
                throw new InsufficientMemoryException(size.ToString());
        }

        public byte* MemoryBlock() => (byte*)lpmem;

        public T* AsPointer<T>() where T : unmanaged => (T*)lpmem;

        public T* AsPointer<T>(int offset) where T : unmanaged => (T*)((byte*)lpmem + offset);

        public IntPtr ToIntPtr() => (IntPtr)lpmem;

        public void Dispose()
        {
            VirtualFree(lpmem, 0, MEM_RELEASE);
            lpmem = null;
        }
    }
}
