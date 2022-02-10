using System;
using System.Collections.Generic;

namespace SuperComicLib.CodeDesigner
{
    public abstract class SymbolMapBase : ISymbolMap
    {
        private Dictionary<string, TokenType> table;
        private Dictionary<HashedString, int> nont = new Dictionary<HashedString, int>();
        private List<Range> list;

        protected SymbolMapBase() => table = Initalize();

        public virtual TokenType Terminal(string text) => table[text];

        public virtual bool Terminal(string text, out TokenType result) => table.TryGetValue(text, out result);

        public virtual int NonTerminal(string text)
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

        protected abstract Dictionary<string, TokenType> Initalize();

        public virtual bool ContainsNT(string text) =>
            nont.TryGetValue(new HashedString(text), out int index) &&
            list[index.ToIndex()].end > 0;

        public virtual void BeginParseGrammar(List<Range> list) => this.list = list;

        public virtual void EndParseGrammar()
        {
            list = null;
            nont.Clear();
        }

        #region disposable
        protected virtual void Dispose(bool disposing)
        {
            if (table != null)
            {
                table.Clear();
                table = null;

                nont.Clear();
                nont = null;
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
