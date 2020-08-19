using System;
using System.Collections;
using System.Collections.Generic;
using SuperComicLib.Collections;

namespace SuperComicLib.CodeDesigner
{
    public sealed class StrKeywordTable : IDisposable, IEnumerable<KeyValuePair<string, TokenType>>
    {
        public string[] texts;
        public TokenType[] tokenTypes;
        private int idx;

        public StrKeywordTable(int size)
        {
            texts = new string[size];
            tokenTypes = new TokenType[size];
        }

        public void Add(string value, TokenType tokenType)
        {
#if DEBUG
            if (idx >= texts.Length)
                throw new IndexOutOfRangeException();
#endif
            ref int tmp = ref idx;
            texts[tmp] = value;
            tokenTypes[tmp] = tokenType;
            tmp++;
        }

        public void EnsureCapacity(int need_size)
        {
            if (idx + need_size > texts.Length)
            {
                int i = idx;
                int newmax = i + need_size;

                string[] ns = new string[newmax];
                TokenType[] nt = new TokenType[newmax];

                ref string[] old0 = ref texts;
                ref TokenType[] old1 = ref tokenTypes;

                Array.Copy(old0, ns, i);
                Array.Copy(old1, nt, i);

                old0 = ns;
                old1 = nt;
            }
        }

        public void Dispose()
        {
            texts = null;
            tokenTypes = null;
            idx = 0;

            GC.SuppressFinalize(this);
        }

        public IEnumerablePair<string, TokenType> Pair => texts.MakePair(tokenTypes, idx);

        #region hide
        IEnumerator<KeyValuePair<string, TokenType>> IEnumerable<KeyValuePair<string, TokenType>>.GetEnumerator() => null;

        IEnumerator IEnumerable.GetEnumerator() => null;
        #endregion
    }
}
