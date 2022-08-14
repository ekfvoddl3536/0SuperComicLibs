using System.Runtime.InteropServices;

namespace SuperComicLib.IO
{
    [StructLayout(LayoutKind.Sequential, Size = sizeof(int) * 16)]
    public struct Int32x16Table
    {
        public int first;
    }
}
