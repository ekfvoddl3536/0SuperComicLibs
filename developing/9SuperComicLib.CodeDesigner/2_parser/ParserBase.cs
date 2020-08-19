using System;
using System.Collections.Generic;
using SuperComicLib.Collections;

namespace SuperComicLib.CodeDesigner
{
    public abstract class ParserBase : IDisposable
    {
        protected Grammar m_grammar;
        protected IExceptionHandler m_handler;

        protected ParserBase(Grammar grammar) : this(grammar, ExceptionHandlerFactory.Default) { }

        protected ParserBase(Grammar grammar, IExceptionHandler handler)
        {
            m_grammar = grammar;
            m_handler = handler;
        }

        #region lock
        public INode[] Parse(Token[] input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));
            if (input.Length == 0)
                throw new ArgumentOutOfRangeException(nameof(input));
            if (OnParsePrepare() == false)
                return null;

            List<INode> nodes = new List<INode>();
            IForwardIterator<Token> iterator = input.Begin();

            Stack<int> tempstack = new Stack<int>();
            Stack<INode> tempNodes = new Stack<INode>();

            IExceptionHandler handler = m_handler;
            handler.Reset();
            do
            {
                INode result = OnParse(iterator, tempstack, tempNodes);
                if (result == null)
                    handler.Fail(FMSG.I);
                else
                    nodes.Add(result);

                tempstack.Clear();
                tempNodes.Clear();
            } while (iterator.LazyAdd() && handler.FailCount == 0);
            iterator.Dispose();

            OnParseCleanup(handler.FailCount > 0);
            return nodes.ToArray();
        }

        protected virtual bool OnParsePrepare() => true;

        protected virtual void OnParseCleanup(bool faulted) { }

        protected abstract INode OnParse(IForwardIterator<Token> iterator, Stack<int> tempstack, Stack<INode> tempNodes);

        // public virtual INode[] Parse(Token[] input)
        // {
        //     int x = 0, max = input.Length;
        // 
        //     Stack<int> stack = new Stack<int>();
        //     List<INode> result = new List<INode>();
        //     stack.Push(0);
        // 
        //     int[] gtb = m_gotoTable;
        //     Map<TableItem[]> tb = m_table;
        //     Grammar g = m_grammar;
        // 
        //     Stack<INode> temp = new Stack<INode>();
        // 
        //     while (x < max)
        //     {
        //         int hashidx = (int)input[x].type;
        //         TableItem item = tb.Get(hashidx).Find(stack.Peek());
        //         if (item.IsInvalid)
        //         {
        //             // error
        //             m_handler.Fail("invalid token: " + input[x].ToString());
        //             break;
        //         }
        // 
        //         int type = item.actType;
        //         if (type == act_SHIFT)
        //         {
        //             // stack.Push((int)input[x].type);
        //             temp.Push(new TokNode(input[x]));
        // 
        //             stack.Push(hashidx);
        //             stack.Push(item.nextstate);
        //             x++;
        //         }
        //         else if (type == act_REDUCE)
        //         {
        //             GItem git = g[item.nextstate];
        //             int rlen = git.express.Length;
        // 
        //             INode exnode = new ExNode(rlen);
        //             while (--rlen >= 0)
        //             {
        //                 exnode.Add(temp.Pop());
        //                 stack.Pop();
        //                 stack.Pop();
        //             }
        // 
        //             temp.Push(exnode);
        //             
        //             int nx = stack.Peek();
        //             stack.Push(git.produce);
        //             stack.Push(gtb[nx]);
        //         }
        //         else if (type == act_ACCEPT)
        //         {
        //             result.AddRange(temp.ToArray());
        // 
        //             temp.Clear();
        //             stack.Clear();
        // 
        //             stack.Push(0);
        //         }
        //     }
        // 
        //     // stack.Clear();
        // 
        //     return result.ToArray();
        // }
        #endregion

        #region disposable
        protected virtual void Dispose(bool disposing)
        {
            if (m_grammar != null)
            {
                m_grammar.Dispose();
                m_grammar = null;

                m_handler = null;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
