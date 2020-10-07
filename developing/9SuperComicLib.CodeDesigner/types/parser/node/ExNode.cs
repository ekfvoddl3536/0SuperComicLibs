using System;
using System.Collections;

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

        public Token GetToken() => Token.Empty;

        public INodeEnumerator GetEnumerator() => new Enumerator(this);

        public int DeepCount(int limit, int find)
        {
            for (int x = 0; x < m_count && find < limit; x++)
                if (m_items[x].ChildCount > 0)
                    find = m_items[x].DeepCount(limit, find + 1);

            return find;
        }

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

#if DEBUG
        public override string ToString() => $"{nameof(ExNode)}: {ChildCount}";
#endif

        #region block
        internal sealed class Enumerator : INodeEnumerator
        {
            public ExNode target;
            public int index;

            public Enumerator(ExNode target)
            {
                this.target = target;
                index = target.ChildCount;
            }

            public INode Current => target[index];
            object IEnumerator.Current => Current;

            public INode Peek() =>
                index > 0
                ? target[index - 1]
                : null;

            public INode Peek(int idx)
            {
                int n;
                return 
                    // ldloc.0대신 dup 을 사용하기 위함
                    (n = index - idx) > 0
                    ? target[n]
                    : null;
            }

            public int TokenCount
            {
                get
                {
                    int cnt = 0;
                    int idx = index;
                    while (--idx >= 0)
                        if (target[idx].ChildCount == 0)
                            cnt++;

                    return cnt;
                }
            }
            public int Count => target.m_count;
            public int Index => index;

            public bool MoveNext() => --index >= 0;

            public void Reset() => index = target.ChildCount;

            public int DeepCount(int limit) => target.DeepCount(limit, 1);

            public void Dispose()
            {
                target = null;
                index = 0;

                GC.SuppressFinalize(this);
            }
        }
        #endregion
    }
}
