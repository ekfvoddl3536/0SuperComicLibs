namespace SuperComicLib.LowLevel
{
    public unsafe sealed class NativeClass<T> : PointerMethods<T> where T : class
    {
        private UnsafeCastClass<T> cast;

        public NativeClass() => cast = NativeClass.CreateCastClass<T>(typeof(NativeClass<T>));

        public T Cast(void* ptr) => cast.Invoke(ptr);

        public override T Default(void* ptr) => Cast(ptr);

        protected override void Dispose(bool disposing) => cast = null;
    }
}