namespace SuperComicLib.CodeDesigner
{
    internal sealed class DefPPAE : PreParseActionEnumerator
    {
        private IExceptionHandler handler;

        public DefPPAE(ITokenEnumerator source, IExceptionHandler handler) : base(source) => 
            this.handler = handler;

        protected override bool OnMoveNext() => handler.FailCount == 0;

        protected override bool OnPreParse(Token now)
        {
            TokenType tt = now.type;
            return
                tt == TokenType.EOL ||
                tt == TokenType._else;
        }

        /// <returns>want skip current, return true</returns>
        protected override bool OnPeeked(Token previous, Token current, Token peek) =>
            current.type == TokenType.EOL && 
            peek.type == TokenType._else;

        protected override void OnMODCurrent(Token prev2, Token previous, ref Token current)
        {
            if (prev2.type == TokenType.EOL &&
                previous.type == TokenType._else &&
                current.type == TokenType.EOL &&
                source.MoveNext())
                current = source.Current;
        }

        protected override void Dispose(bool disposing)
        {
            handler = null;
            base.Dispose(disposing);
        }
    }
}
