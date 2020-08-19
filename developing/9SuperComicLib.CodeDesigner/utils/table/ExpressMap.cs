using System;
using System.Collections.Generic;

namespace SuperComicLib.CodeDesigner
{
    public abstract class ExpressMap : IExpressMap
    {
        private Dictionary<string, TokenType> table;

        protected ExpressMap() => table = Initalize();

        public virtual TokenType Terminal(string text) => table[text];

        public virtual bool Terminal(string text, out TokenType result) => table.TryGetValue(text, out result);

        public virtual int Nonterminal(string text) => ParseNonterminalDefault(text);

        protected abstract Dictionary<string, TokenType> Initalize();

        internal static int ParseNonterminalDefault(string str) =>
            str == "e!!"
            ? ExpressInt.epsilon
            : str == "S"
            ? ExpressInt.start_symbol
            : str.ToNonterminal();

        #region disposable
        protected virtual void Dispose(bool disposing)
        {
            if (table != null)
            {
                table.Clear();
                table = null;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
