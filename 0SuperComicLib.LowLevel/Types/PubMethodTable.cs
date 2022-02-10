#pragma warning disable
using System;
using System.Runtime.InteropServices;

namespace SuperComicLib.LowLevel
{
    [StructLayout(LayoutKind.Sequential)]
    public readonly unsafe struct PubMethodTable
    {
        public readonly uint Flags1;
        public readonly uint Size;
        public readonly ushort Flags2;
        public readonly ushort Token;
        public readonly ushort NumVirtualMethods;
        public readonly ushort NumInterfaces;
        public readonly PubMethodTable* ParentMT;
        public readonly IntPtr pModule;
    }
}
