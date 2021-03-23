using System;

namespace SuperComicLib.CodeDesigner
{
    public sealed class EmptyNode : INode
    {
        public static readonly INode Instance = new EmptyNode();

        private EmptyNode() { }

        public INode this[int index] => null;

        public INode Parent
        {
            get => null;
            set { }
        }

        public INode C0 => null;
        public INode C1 => null;
        public INode C2 => null;
        public INode C3 => null;
        public int ChildCount => -1;

        public void Add(INode node) { }

        public void Dispose()
        {
#if DEBUG
            throw new InvalidOperationException($"do not dispose {nameof(EmptyNode)}");
#endif
        }

        public INodeEnumerator GetEnumerator() => null;

        public Token GetToken() => Token.Empty;
    }
}
