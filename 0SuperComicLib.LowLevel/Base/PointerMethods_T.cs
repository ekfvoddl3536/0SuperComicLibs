using System.Security;

namespace SuperComicLib.LowLevel
{
    public unsafe abstract class PointerMethods<T>
    {
        protected PointerMethods() { }

        [SecurityCritical]
        public abstract T Default(void* ptr);
    }
}
