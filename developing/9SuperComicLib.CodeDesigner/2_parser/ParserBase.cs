using System;
using System.Collections.Generic;
using System.IO;

namespace SuperComicLib.CodeDesigner
{
    public abstract class ParserBase : IDisposable
    {
        protected internal Grammar m_grammar;
        protected internal IExceptionHandler m_handler;
        protected internal bool leaveOpen;

        protected ParserBase() { }

        protected ParserBase(Grammar grammar) : this(grammar, ExceptionHandlerFactory.Default, false) { }

        protected ParserBase(Grammar grammar, IExceptionHandler handler) : this(grammar, handler, false) { }

        protected ParserBase(Grammar grammar, IExceptionHandler handler, bool leaveOpen)
        {
            m_grammar = grammar;
            m_handler = handler;

            this.leaveOpen = leaveOpen;
        }

        #region parse
        public INode Parse(ITokenEnumerator inpu) => Parse(inpu, true);

        public INode Parse(ITokenEnumerator input, bool autoDisposing)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));
            if (OnParsePrepare() == false)
                return null;

            Stack<int> tempstack = new Stack<int>();
            Stack<INode> tempnodes = new Stack<INode>();

            m_handler.Reset();

            input.MoveNext();
            INode result = OnParse(input, tempstack, tempnodes);

            tempstack.Clear();
            tempnodes.Clear();

            if (autoDisposing)
                input.Dispose();

            OnParseCleanup(m_handler.FailCount > 0);
            return result;
        }

        protected virtual bool OnParsePrepare() => true;

        protected virtual void OnParseCleanup(bool faulted) { }

        protected abstract INode OnParse(IEnumerator<Token> iterator, Stack<int> tempstack, Stack<INode> tempnodes);
        #endregion


        #region serialize & deserialize
        protected internal virtual void OnSerialize(BinaryWriter writer) { }
        protected internal virtual void OnDeserialize(BinaryReader reader) { }
        #endregion

        #region disposable
        protected virtual void Dispose(bool disposing)
        {
            if (m_grammar != null && leaveOpen == false)
                m_grammar.Dispose();

            m_grammar = null;
            m_handler = null;

            leaveOpen = false;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
