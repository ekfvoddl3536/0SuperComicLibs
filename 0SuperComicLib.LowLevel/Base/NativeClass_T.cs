using System;
using System.Security;

namespace SuperComicLib.LowLevel
{
    [SuppressUnmanagedCodeSecurity]
    public unsafe sealed class NativeClass<T> : PointerMethods<T>
        where T : class
    {
        #region instance members
        private readonly UnsafeCastClass<T> cast;

        internal NativeClass() => cast = NativeClass.CreateCastClass<T>(typeof(NativeClass<T>));

        public T Cast(void* ptr) => cast.Invoke(ptr);

        public override TRet Get<TRet>(ref T inst, int offset) =>
            inst != null 
            ? Read_unsafe<TRet>(ref inst, offset)
            : default;

        public TRet Read_unsafe<TRet>(ref T inst, int offset) where TRet : unmanaged
        {
            TypedReference tr = __makeref(inst);
            return *(TRet*)(**(byte***)&tr + (sizeof(void*) + offset));
        }

        public override void Set<TSet>(ref T inst, TSet value, int offset)
        {
            if (inst == null)
                throw new ArgumentNullException(nameof(inst));

            Set_unsafe(ref inst, value, offset);
        }

        public void Set_unsafe<TSet>(ref T inst, TSet value, int offset) where TSet : unmanaged
        {
            TypedReference tr = __makeref(inst);
            *(TSet*)(**(byte***)&tr + (sizeof(void*) + offset)) = value;
        }

        public override T Default(void* ptr) => Cast(ptr);

        public override void RefMemory(ref T obj, UnsafePointerAction cb)
        {
            TypedReference tr = __makeref(obj);
            cb.Invoke(**(byte***)&tr + sizeof(void*));
        }
        #endregion

        #region static members
        private static NativeClass<T> m_instance;

        public static NativeClass<T> Instance => m_instance ?? (m_instance = new NativeClass<T>());
        #endregion
    }
}