using System;

namespace SuperComicLib.CodeDesigner
{
    internal sealed class EnumTM : IExpressMap
    {
        private static readonly IExpressMap inst = new EnumTM();

        public static IExpressMap Default => inst;

        private EnumTM() { }

        public TokenType Terminal(string text) => (TokenType)Enum.Parse(typeof(TokenType), text);

        public bool Terminal(string text, out TokenType result) => Enum.TryParse(text, out result);

        public int Nonterminal(string text) => ExpressMap.ParseNonterminalDefault(text);

        public void Dispose() { }
    }
}
