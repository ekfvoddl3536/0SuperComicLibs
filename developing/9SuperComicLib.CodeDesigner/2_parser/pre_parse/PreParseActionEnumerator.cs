using System;
using System.Collections;

namespace SuperComicLib.CodeDesigner
{
    public class PreParseActionEnumerator : ITokenEnumerator
    {
        protected ITokenEnumerator source;
        private Token current;
        private Token prev;
        private bool peeked;

        public PreParseActionEnumerator(ITokenEnumerator source)
        {
            this.source = source;
            current = Token.Empty;
        }

        public bool IsEnd => source.IsEnd;
        public Token Current => current;
        object IEnumerator.Current => current;

        public bool MoveNext()
        {
            ITokenEnumerator source = this.source;
            if (peeked)
            {
                Token tempCurrent = source.Current;
                OnMODCurrent(prev, current, ref tempCurrent);

                current = tempCurrent;
                peeked = false;

                return true;
            }

            if (!source.MoveNext())
                return false;

            loop:
            prev = current;
            current = source.Current;
            if (OnPreParse(current) && source.MoveNext())
            {
                peeked = true;
                if (OnPeeked(prev, current, source.Current))
                    goto loop;
            }

            return true;
        }

        protected virtual bool OnPeeked(Token previous, Token current, Token peek) => false;

        protected virtual void OnMODCurrent(Token previous_2, Token previous, ref Token current) { }

        protected virtual bool OnPreParse(Token now) => false;

        public void Reset()
        {
            current = null;
            peeked = false;
            source.Reset();

            OnReset();
        }

        protected virtual void OnReset() { }

        #region disposable
        protected virtual void Dispose(bool disposing)
        {
            if (source != null)
            {
                source.Dispose();
                source = null;
            }

            current = null;
            peeked = false;
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
