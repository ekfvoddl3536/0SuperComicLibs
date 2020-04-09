using System;

namespace SuperComicLib.LowLevel
{
    public delegate void UnsafePinnedAsIntPtr<T>(T target, Action<IntPtr> callback);

    public unsafe delegate T UnsafeCastClass<T>(void* ptr) where T : class;

    public unsafe delegate T UnsafeReadPointerStruct<T>(void* ptr) where T : struct;

    public unsafe delegate void UnsafePointerAction<T>(T* ptr) where T : unmanaged;

    internal unsafe delegate void UnsafeMemoryCopyBlock(void* src, void* dst, uint count);
}
