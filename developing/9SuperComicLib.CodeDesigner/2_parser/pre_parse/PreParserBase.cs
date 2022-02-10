using System;
using System.Collections;

namespace SuperComicLib.CodeDesigner
{
    public abstract class PreParserBase : ITokenEnumerator
    {
        protected ITokenEnumerator source;
        protected Token current;
        private readonly bool ignoreEOL;

        protected PreParserBase(ITokenEnumerator source, bool ignoreEOL)
        {
            this.source = source;
            this.ignoreEOL = ignoreEOL;
            current = Token.Empty;
        }

        public virtual bool IsEnd => source.IsEnd;
        public virtual Token Current => current;
        object IEnumerator.Current => Current;

        public virtual bool MoveNext()
        {
        loop:
            if (!source.MoveNext())
                return false;

            Token current = source.Current;
            if (ignoreEOL && current.type == TokenType.EOL)
                goto loop;

            this.current = current;
            return true;
        }

        public virtual void Reset()
        {
            current = null;
            source.Reset();
        }

        #region disposable
        protected virtual void Dispose(bool disposing)
        {
            if (source != null)
            {
                source.Dispose();
                source = null;
            }

            current = null;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
