using System;
using System.Collections;
using System.Collections.Generic;

namespace SuperComicLib.CodeDesigner
{
    public sealed class FilteredTokenEnumerator : IEnumerator<Token>
    {
        private ToTokenEnumerator source;
        private TokenType[] fs;
        private int flen;
        private bool em;

        public FilteredTokenEnumerator(INodeEnumerator ne, bool deep, bool applyMove, bool exceptMode, params TokenType[] filters)
        {
            source = new ToTokenEnumerator(ne, deep, applyMove);
            fs = filters;
            flen = filters.Length;
            em = exceptMode;
        }


        public Token Current => source.current;
        object IEnumerator.Current => source.current;

        public bool MoveNext()
        {
        loop:
            if (!source.MoveNext())
                return false;

            TokenType[] vs = fs;
            int tmp = flen;

            TokenType tt = source.current.type;
            if (em)
            {
                while (--tmp >= 0)
                    if (vs[tmp] == tt)
                        goto loop;
            }
            else
            {
                while (--tmp >= 0)
                    if (vs[tmp] == tt)
                        return true;

                goto loop;
            }

            return true;
        }

        public void Reset() { }

        public void Dispose()
        {
            source = null;
            fs = null;
            flen = 0;
            em = false;
            GC.SuppressFinalize(this);
        }
    }
}
