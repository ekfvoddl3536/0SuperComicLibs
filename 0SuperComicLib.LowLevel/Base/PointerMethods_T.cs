using System;
using System.Security;

namespace SuperComicLib.LowLevel
{
    public unsafe abstract class PointerMethods<T>
    {
        protected readonly UnsafePinnedAsIntPtr<T> pinned;

        protected PointerMethods() => pinned = NativeClass.CreatePinnedPtr<T>(typeof(PointerMethods<T>));

        [SecurityCritical]
        public virtual void Pinned(T obj, Action<IntPtr> cb)
        {
            if (obj != null && cb != null)
                pinned.Invoke(obj, cb);
        }

        public abstract T Default(void* ptr);

        public virtual IntPtr GetAddr(ref T obj) => NativeClass.GetAddr(ref obj);

        public virtual void GetPinnedPtr(ref T obj, Action<IntPtr> cb) => NativeClass.PinnedAddr(ref obj, cb);
    }
}
