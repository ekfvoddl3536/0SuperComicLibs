using System;

namespace SuperComicLib.CodeDesigner
{
    public sealed class ExNode : INode
    {
        private int m_count;
        private INode[] m_items;
        private INode m_parent;

        public ExNode(int count)
        {
            m_items = new INode[count];
            m_count = count;
        }

        public INode this[int index] => m_items[index];

        public INode Parent
        {
            get => m_parent;
            set => m_parent = value;
        }
        public INode C0 => m_items[0];
        public INode C1 => m_items[1];
        public INode C2 => m_items[2];
        public INode C3 => m_items[3];

        public int ChildCount => m_count;

        public void Add(INode node)
        {
            INode[] vs = m_items;
            for (int x = 0, cnt = m_count; x < cnt; x++)
                if (vs[x] == null)
                {
                    vs[x] = node;
                    node.Parent = this;
                    break;
                }
        }

        public Token GetToken() => null;

        public void Dispose()
        {
            if (m_items != null)
            {
                int x = m_count;
                while (--x >= 0)
                    m_items[x].Dispose();

                m_parent = null;
                m_items = null;
                m_count = 0;
            }
            GC.SuppressFinalize(this);
        }
    }
}
