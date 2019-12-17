using System;

namespace SuperComicLib.LowLevel
{
    public unsafe sealed class NativeStruct<T> : PointerMethods<T> where T : struct
    {
        private UnsafeUnboxStruct<T> unbox;
        private UnsafeReadPointerStruct<T> read;

        public NativeStruct()
        {
            Type owner = typeof(NativeStruct<T>);

            unbox = NativeClass.CreateUnbox<T>(owner);
            read = NativeClass.CreateReadPointer<T>(owner);
        }

        public T Unbox(object obj) => unbox.Invoke(obj);

        public T Read(void* ptr) => read.Invoke(ptr);

        public override T Default(void* ptr) => Read(ptr);

        protected override void Dispose(bool disposing)
        {
            unbox = null;
            read = null;
        }
    }
}