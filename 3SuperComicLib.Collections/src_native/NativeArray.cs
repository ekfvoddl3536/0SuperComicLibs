using System;
using System.Runtime.InteropServices;

namespace SuperComicLib.Collections
{
    [StructLayout(LayoutKind.Sequential)]
    public readonly unsafe partial struct NativeArray<T> : IDisposable, IRawContainer<T>, IReadOnlyRawContainer<T>, IEquatable<NativeArray<T>>
        where T : unmanaged
    {
    }
}
