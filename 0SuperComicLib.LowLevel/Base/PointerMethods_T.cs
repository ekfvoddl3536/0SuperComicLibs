using System.Security;

namespace SuperComicLib.LowLevel
{
    public unsafe abstract class PointerMethods<T>
    {
        protected PointerMethods() { }

        [SecurityCritical]
        public abstract T Default(void* ptr);

        [SecurityCritical]
        public abstract void RefMemory(ref T obj, UnsafePointerAction cb);
    }
}
