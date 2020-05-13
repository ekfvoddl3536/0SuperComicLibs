using System;

namespace SuperComicLib.LowLevel
{
    public readonly struct UnsafeCLIMemoryData
    {
        internal readonly IntPtr typehnd;
        internal readonly byte[] memory;

        internal UnsafeCLIMemoryData(IntPtr typehnd, byte[] memory)
        {
            this.typehnd = typehnd;
            this.memory = memory;
        }

        internal bool IsEmpty => typehnd == IntPtr.Zero || memory == null;

        public override bool Equals(object obj) => false;

        public override int GetHashCode() => 0;

        public static bool operator ==(UnsafeCLIMemoryData left, UnsafeCLIMemoryData right) =>
            left.memory == right.memory;

        public static bool operator !=(UnsafeCLIMemoryData left, UnsafeCLIMemoryData right) =>
            left.memory != right.memory;
    }
}
