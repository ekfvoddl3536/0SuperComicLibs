using System.Security;

namespace SuperComicLib.LowLevel
{
    [SuppressUnmanagedCodeSecurity]
    public unsafe abstract class PointerMethods<T>
    {
        protected PointerMethods() { }

        public abstract T Default(void* ptr);

        public abstract TOut Get<TOut>(ref T inst, int offset) where TOut : unmanaged;

        public abstract void Set<TIn>(ref T inst, TIn value, int offset) where TIn : unmanaged;

        public abstract void RefMemory(ref T obj, UnsafePointerAction cb);
    }
}
