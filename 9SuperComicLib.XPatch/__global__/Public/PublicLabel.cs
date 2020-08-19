using System.Reflection.Emit;
using System.Runtime.InteropServices;

namespace SuperComicLib.XPatch
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public readonly unsafe struct PublicLabel
    {
        public readonly int m_offset;
        public readonly bool valid;

        internal PublicLabel(int v)
        {
            m_offset = v;
            valid = true;
        }

        public static implicit operator Label(PublicLabel value) => *(Label*)&value;
        public static implicit operator PublicLabel(Label value) => new PublicLabel(*(int*)&value);

        public override bool Equals(object obj) => false;
        public override int GetHashCode() => m_offset.GetHashCode();

        public static bool operator ==(PublicLabel left, PublicLabel right) =>
            left.m_offset == right.m_offset &&
            left.valid == right.valid;
        public static bool operator !=(PublicLabel left, PublicLabel right) =>
            left.m_offset != right.m_offset ||
            left.valid != right.valid;
    }
}
