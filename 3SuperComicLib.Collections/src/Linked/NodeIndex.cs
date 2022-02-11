using System.Diagnostics;

namespace SuperComicLib.Collections
{
    [DebuggerDisplay("Value = {Value}")]
    public readonly struct NodeIndex<T>
    {
        internal readonly ILinkedListSlim_Internal<T> m_owner;
        internal readonly int m_index;

        internal NodeIndex(ILinkedListSlim_Internal<T> owner, int index)
        {
            m_owner = owner;
            m_index = index;
        }

        public int RawIndex => m_index;

        public bool IsInvalid => m_owner == null || m_index < 0;

        public ref T Value => ref m_owner.ByRefValue(m_index);

        public NodeIndex<T> Next => new NodeIndex<T>(m_owner, m_owner.GetNextNode(m_index));

        public NodeIndex<T> Prev => new NodeIndex<T>(m_owner, m_owner.GetPrevNode(m_index));

        public override bool Equals(object obj) => false;
        public override int GetHashCode() => m_owner.GetHashCode() ^ m_index;

        public static bool operator ==(NodeIndex<T> left, NodeIndex<T> right) => left.m_owner == right.m_owner && left.m_index == right.m_index;
        public static bool operator !=(NodeIndex<T> left, NodeIndex<T> right) => left.m_owner != right.m_owner || left.m_index != right.m_index;

        public static NodeIndex<T> operator ++(NodeIndex<T> self) => self.Next;
        public static NodeIndex<T> operator --(NodeIndex<T> self) => self.Prev;
    }
}
