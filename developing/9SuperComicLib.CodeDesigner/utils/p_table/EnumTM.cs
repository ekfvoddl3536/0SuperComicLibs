using System;
using System.Collections.Generic;

namespace SuperComicLib.CodeDesigner
{
    internal sealed class EnumTM : ISymbolMap
    {
        private Dictionary<HashedString, int> nont = new Dictionary<HashedString, int>();
        private List<Range> list;

        public TokenType Terminal(string text) => (TokenType)Enum.Parse(typeof(TokenType), text);

        public bool Terminal(string text, out TokenType result) => Enum.TryParse(text, out result);

        public int NonTerminal(string text)
        {
            if (text[0] == '<')
                text = text.Substring(1, text.Length - 2);

            HashedString str = new HashedString(text);
            if (!nont.TryGetValue(str, out int result))
            {
                result = list.Count.ToNonterminal();
                list.Add(default);
                nont.Add(str, result);
            }

            return result;
        }

        public bool ContainsNT(string text) =>
            nont.TryGetValue(new HashedString(text), out int index) &&
            list[index.ToIndex()].end > 0;

        public void BeginParseGrammar(List<Range> list) => this.list = list;

        public void EndParseGrammar()
        {
            list = null;
            nont.Clear();
        }

        public void Dispose()
        {
            if (nont != null)
            {
                nont.Clear();
                nont = null;
            }
            GC.SuppressFinalize(this);
        }
    }
}
