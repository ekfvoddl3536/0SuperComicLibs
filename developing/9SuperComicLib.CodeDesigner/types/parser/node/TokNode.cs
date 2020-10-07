using System;

namespace SuperComicLib.CodeDesigner
{
    public sealed class TokNode : INode
    {
        private Token m_token;
        private INode m_parent;

        public TokNode(Token token) => m_token = token;

        public INode this[int index] => null;

        public INode Parent
        {
            get => m_parent;
            set => m_parent = value;
        }
        public INode C0 => null;
        public INode C1 => null;
        public INode C2 => null;
        public INode C3 => null;
        public int ChildCount => 0;

        public void Add(INode node) { }

        public Token GetToken() => m_token;

        public INodeEnumerator GetEnumerator() => null;

        public int DeepCount(int limit, int find) => find;

        public void Dispose()
        {
            m_token = null;
            m_parent = null;

            GC.SuppressFinalize(this);
        }

#if DEBUG
        public override string ToString() => m_token.ToString();
#endif
    }
}
