namespace SuperComicLib.LowLevel
{
    public unsafe delegate T UnsafeCastClass<T>(void* ptr) where T : class;

    public unsafe delegate T UnsafeReadPointerStruct<T>(void* ptr) where T : struct;

    public unsafe delegate void UnsafePointerAction(byte* ptr);

    internal unsafe delegate void UnsafeMemoryCopyBlock(void* src, void* dst, uint count);

    internal static class Constants
    {
        internal const uint
            PAGE_NOACCESS = 0x01,
            PAGE_READONLY = 0x02,
            PAGE_READWRITE = 0x04,
            MEM_RESERVE = 0x2000,
            MEM_COMMIT = 0x1000,
            MEM_RELEASE = 0x8000;
    }
}
