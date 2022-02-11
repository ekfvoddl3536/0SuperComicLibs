using System;
using System.Runtime.InteropServices;
using SuperComicLib.Core;

namespace SuperComicLib.Collections
{
    public readonly unsafe struct CoTaskAllocater : IRawAllocater
    {
        public IntPtr stdAlloc(int cb, bool initDefault)
        {
            IntPtr p0 = Marshal.AllocCoTaskMem(cb);

            if (initDefault)
                MemoryBlock.Clear(p0, cb);

            return p0;
        }

        public void stdFree(IntPtr ptr) => Marshal.FreeCoTaskMem(ptr);
    }
}
