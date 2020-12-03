namespace SuperComicLib.CodeDesigner
{
    internal sealed class DefPPAE : PreParseActionEnumerator
    {
        public DefPPAE(ITokenEnumerator source) : base(source)
        {
        }

        // protected override bool OnMoveNext() => handler.FailCount == 0;

        protected override bool OnPreParse(Token now)
        {
            TokenType tt = now.type;
            return
                tt == TokenType.type ||
                tt == TokenType.EOL ||
                tt == TokenType._else;
        }

        /// <returns>want skip current, return true</returns>
        protected override bool OnPeeked(Token previous, Token current, Token peek)
        {
            TokenType pk1 = peek.type;
            return 
                current.type == TokenType.EOL &&
                (pk1 == TokenType._else || pk1 == (TokenType)ExpressInt.end_symbol);
        }

        protected override void OnMODCurrent(Token prev2, Token previous, ref Token current)
        {
            TokenType pv2tt = prev2.type;
            TokenType pv1tt = previous.type;
            if (pv2tt == TokenType.EOL && pv1tt == TokenType._else && current.type == TokenType.EOL && source.MoveNext())
                current = source.Current;
            else if (pv1tt == TokenType.type && current.type == TokenType.type)
                current = new Token(current.text, TokenType.id, current.line, current.row);
        }
    }
}
