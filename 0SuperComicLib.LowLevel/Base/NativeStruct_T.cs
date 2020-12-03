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

        public override TRet Read<TRet>(ref T inst, int offset)
        {
            TypedReference tr = __makeref(inst);
            return *(TRet*)(*(byte**)&tr + offset);
        }

        public override void Set<TSet>(ref T inst, TSet value, int offset)
        {
            TypedReference tr = __makeref(inst);
            *(TSet*)(*(byte**)&tr + offset) = value;
        }

        public override T Default(void* ptr) => Read(ptr);

        public override void RefMemory(ref T obj, UnsafePointerAction cb)
        {
            TypedReference tr = __makeref(obj);
            cb.Invoke(*(byte**)&tr);
        }
        #endregion

        #region static members
        private static NativeStruct<T> m_instance;

        public static NativeStruct<T> Instance => m_instance ?? (m_instance = new NativeStruct<T>());
        #endregion
    }
}