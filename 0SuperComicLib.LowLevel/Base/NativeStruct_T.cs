using System;
using System.Security;

namespace SuperComicLib.LowLevel
{
    [SuppressUnmanagedCodeSecurity]
    public unsafe sealed class NativeStruct<T> : PointerMethods<T> 
        where T : struct
    {
        #region instance members
        private readonly UnsafeReadPointerStruct<T> read;

        internal NativeStruct() => read = NativeClass.CreateReadPointer<T>(typeof(NativeStruct<T>));

        public T Read(void* ptr) => read.Invoke(ptr);

        public override TOut Get<TOut>(ref T inst, int offset)
        {
            TypedReference tr = __makeref(inst);
            return *(TOut*)(*(byte**)&tr + offset);
        }

        public override void Set<TIn>(ref T inst, TIn value, int offset)
        {
            TypedReference tr = __makeref(inst);
            *(TIn*)(*(byte**)&tr + offset) = value;
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