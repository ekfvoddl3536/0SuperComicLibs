using System.Collections.Generic;

namespace SuperComicLib.CodeDesigner
{
    internal sealed class DebugSymbolMap : InternalSymbolMap
    {
        private Dictionary<int, string> nonterminals = new Dictionary<int, string>();

        public override int NonTerminal(string text)
        {
            int result = base.NonTerminal(text);
            if (nonterminals.ContainsKey(result) == false)
                nonterminals.Add(result, text);

            return result;
        }

        public string Get(int hashcode) => nonterminals[hashcode];

        protected override void Dispose(bool disposing)
        {
            if (nonterminals != null)
            {
                nonterminals.Clear();
                nonterminals = null;
            }
            base.Dispose(disposing);
        }
    }
}
