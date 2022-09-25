using System.Security;

namespace SuperComicLib.LowLevel
{
    [SuppressUnmanagedCodeSecurity]
    public unsafe delegate T UnsafeCastClass<T>(void* ptr) where T : class;

    [SuppressUnmanagedCodeSecurity]
    public unsafe delegate T UnsafeReadPointerStruct<T>(void* ptr) where T : struct;

    [SuppressUnmanagedCodeSecurity]
    public unsafe delegate void UnsafePointerAction(byte* ptr);
}
