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

        internal StrKeywordTable() { }

        public int Count
        {
            get => idx;
            internal set => idx = value;
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

        public IEnumeratorPair<string, TokenType> Pair => texts.MakePair(tokenTypes, idx);

        #region hide
        IEnumerator<KeyValuePair<string, TokenType>> IEnumerable<KeyValuePair<string, TokenType>>.GetEnumerator() => null;

        IEnumerator IEnumerable.GetEnumerator() => null;
        #endregion

        public static StrKeywordTable Combine(StrKeywordTable a1, StrKeywordTable b1, bool compress = false)
        {
            StrKeywordTable newTable = 
                new StrKeywordTable(
                    compress
                    ? a1.idx + b1.idx
                    : a1.texts.Length + b1.texts.Length);

            string[] v1 = newTable.texts;
            TokenType[] v2 = newTable.tokenTypes;

            int idx = Add(a1.texts, a1.tokenTypes, v1, v2, 0);
            Add(b1.texts, b1.tokenTypes, v1, v2, idx);

            return newTable;
        }

        private static int Add(string[] s1, TokenType[] s2, string[] dst1, TokenType[] dst2, int startidx)
        {
            int size = s1.Length;
            for (int x = 0; x < size; startidx++, x++)
            {
                dst1[startidx] = s1[x];
                dst2[startidx] = s2[x];
            }

            return startidx;
        }
    }
}
