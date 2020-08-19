using System;
using System.Security;

namespace SuperComicLib.LowLevel
{
    public unsafe sealed class NativeClass<T> : PointerMethods<T> where T : class
    {
        #region instance members
        private readonly UnsafeCastClass<T> cast;

        internal NativeClass() => cast = NativeClass.CreateCastClass<T>(typeof(NativeClass<T>));

        [SecurityCritical]
        public T Cast(void* ptr) => cast.Invoke(ptr);

        public override T Default(void* ptr) => Cast(ptr);

        public override void RefMemory(ref T obj, UnsafePointerAction cb)
        {
#if DEBUG
            System.Diagnostics.Debug.Assert(obj != null);
#endif
            TypedReference tr = __makeref(obj);
            cb.Invoke(**(byte***)&tr + NativeClass.PtrSize_i);
        }
        #endregion

        #region static members
        private static NativeClass<T> m_instance;

        public static NativeClass<T> Instance
        {
            get
            {
                if (m_instance == null)
                    m_instance = new NativeClass<T>();
                return m_instance;
            }
        }
        #endregion
    }
}