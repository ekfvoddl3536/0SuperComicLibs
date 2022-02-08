using System.Runtime.InteropServices;

namespace SuperComicLib.Collections
{
    [StructLayout(LayoutKind.Sequential)]
    public readonly unsafe struct RawMemory
    {
        // 바이트 기준이 아니므로, 정확하지 않음
        public readonly int _size;
        public readonly void* _value;

        public RawMemory(int size, void* value)
        {
            _size = size;
            _value = value;
        }
    }
}
