namespace SuperComicLib.LowLevel
{
    public unsafe delegate T UnsafeCastClass<T>(void* ptr) where T : class;

    public unsafe delegate T UnsafeReadPointerStruct<T>(void* ptr) where T : struct;

    public unsafe delegate void UnsafePointerAction(byte* ptr);

    internal unsafe delegate void UnsafeMemoryCopyBlock(void* src, void* dst, uint count);
}
