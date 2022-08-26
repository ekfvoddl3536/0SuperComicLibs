#pragma warning disable IDE1006 // 명명 스타일
using System;
using System.Runtime.InteropServices;

namespace SuperComicLib
{
    /// <summary>
    /// Provides limited nuint operations
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public readonly unsafe partial struct size_t : IEquatable<size_t>, IComparable<size_t>
    {
    }
}
