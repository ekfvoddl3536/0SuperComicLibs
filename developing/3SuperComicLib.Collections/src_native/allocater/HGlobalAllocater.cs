using System;
using System.Runtime.InteropServices;
using SuperComicLib.Core;

namespace SuperComicLib.Collections
{
    public readonly unsafe struct HGlobalAllocater : IRawAllocater
    {
        public IntPtr stdAlloc(int cb, bool initDefault)
        {
            IntPtr p0 = Marshal.AllocHGlobal(cb);

            if (initDefault)
                MemoryBlock.Clear(p0, cb);

            return p0;
        }

        public void stdFree(IntPtr ptr) => Marshal.FreeHGlobal(ptr);
    }
}
