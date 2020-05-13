using System;
using System.Security;

namespace SuperComicLib.LowLevel
{
    public unsafe sealed class NativeStruct<T> : PointerMethods<T> where T : struct
    {
        #region instance members
        private readonly UnsafeReadPointerStruct<T> read;

        internal NativeStruct() => read = NativeClass.CreateReadPointer<T>(typeof(NativeStruct<T>));

        [SecurityCritical]
        public T Read(void* ptr) => read.Invoke(ptr);

        public override T Default(void* ptr) => Read(ptr);
        #endregion

        #region static members
        private static NativeStruct<T> m_instance;

        public static NativeStruct<T> Instance
        {
            get
            {
                if (m_instance == null)
                    m_instance = new NativeStruct<T>();
                return m_instance;
            }
        }
        #endregion
    }
}