using System;
using System.Collections;

namespace SuperComicLib.CodeDesigner
{
    /// <summary>
    /// start: for init + jump to cond + declare loop_Label
    /// mid: block + for last
    /// end: decalre cond_Label + for cond (use: loop_Label)
    /// </summary>
    public sealed class ForExpressionEnumerator : INodeEnumerator
    {
        // [0] forInit
        // [1] forCond
        // [2] forLast
        // [3] forBlock
        private INode[] m_nodes; 
        private int m_index;

        public ForExpressionEnumerator(INodeEnumerator ne)
        {
#if DEBUG
            System.Diagnostics.Contracts.Contract.Requires(ne != null);
            System.Diagnostics.Debug.Assert(ne.Current?.GetToken()?.type == TokenType._for);
#endif
            ne.MoveNext();
            // for (. E ; E ; E ) eol E
            // for (. 0 ; 1 ; 2 ; ) eol 3 E

            INode[] nodes = new INode[4];
            INode temp;
            for (int x = 0; x < 4 && ne.MoveNext();)
            {
                temp = ne.Current;
                TokenType tt = temp.GetToken().type;
                if (tt != TokenType.rparen || 
                    tt != TokenType.EOL)
                    nodes[x++] = temp;
                else if (tt == TokenType.semi_colon)
                    nodes[x++] = EmptyNode.Instance;
            }

            temp = nodes[1];
            nodes[1] = nodes[3];
            nodes[3] = temp;
#if DEBUG
            while (ne.MoveNext())
                System.Diagnostics.Debug.WriteLine("[ WARN ] !!!!!!!!!!!!!!!!!!! 아직 남아있는 노드 !!!!!!!!!!!!!!!");
#endif

            m_index = -1;
            m_nodes = nodes;
        }

        public int TokenCount => -1;
        public int Count => 4;
        public int Index => m_index;
        public INode Parent => null;
        public INode Current => m_nodes[m_index];
        object IEnumerator.Current => Current;

        public bool MoveNext() => ++m_index < 4;

        public INode Peek()
        {
#if DEBUG
            throw new NotImplementedException();
#else
            return null;
#endif
        }

        public INode Peek(int idx)
        {
#if DEBUG
            throw new NotImplementedException();
#else
            return null;
#endif
        }

        public void Reset() => m_index = -1;

        public void Dispose()
        {
            INode[] vs = m_nodes;
            for (int x = 0; x < 4;)
                vs[x++] = null;

            m_nodes = null;
            m_index = 0;

            GC.SuppressFinalize(this);
        }
    }
}
