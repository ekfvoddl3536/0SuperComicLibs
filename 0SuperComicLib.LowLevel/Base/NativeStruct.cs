using System;

namespace SuperComicLib.LowLevel
{
    public unsafe sealed class NativeStruct<T> : PointerMethods<T> where T : struct
    {
        private UnsafeReadPointerStruct<T> read;

        public NativeStruct() => read = NativeClass.CreateReadPointer<T>(typeof(NativeStruct<T>));

        [Obsolete("이 메소드는 unbox와 동일한 IL 코드를 만듭니다, 대신 (T)object 를 사용하십시오", true)]
        public T Unbox(object obj) => default;

        public T Read(void* ptr) => read.Invoke(ptr);

        public override T Default(void* ptr) => Read(ptr);

        protected override void Dispose(bool disposing) => read = null;
    }
}