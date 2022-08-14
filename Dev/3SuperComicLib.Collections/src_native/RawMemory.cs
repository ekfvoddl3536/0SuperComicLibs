using System.Runtime.InteropServices;

namespace SuperComicLib.Collections
{
    [StructLayout(LayoutKind.Sequential)]
    public readonly unsafe struct RawMemory
    {
        // 바이트 기준이 아니므로, 정확하지 않음
        public readonly void* _value;
        public readonly int _size;

        public RawMemory(void* value, int size)
        {
            _value = value;
            _size = size;
        }
    }
}
