using System.Reflection.Emit;
using System.Runtime.InteropServices;

namespace SuperComicLib.Runtime
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public readonly unsafe struct PublicLabel
    {
        public readonly int m_offset;
        public readonly bool valid;

        public PublicLabel(int offset)
        {
            m_offset = offset;
            valid = true;
        }

        public PublicLabel(Label label) : this(*(int*)&label)
        {
        }

        public static implicit operator Label(PublicLabel value) => *(Label*)&value;

        public override bool Equals(object obj) => false;
        public override int GetHashCode() => m_offset.GetHashCode();

        public static bool operator ==(PublicLabel left, PublicLabel right) => left.m_offset == right.m_offset;
        public static bool operator !=(PublicLabel left, PublicLabel right) => left.m_offset != right.m_offset;
    }
}
