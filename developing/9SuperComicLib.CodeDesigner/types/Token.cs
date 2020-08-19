namespace SuperComicLib.CodeDesigner
{
    public sealed class Token
    {
        public readonly string text;
        public readonly TokenType type;
        public readonly int line;
        public readonly int row;

        public Token(string text, TokenType type, int line, int row)
        {
            this.text = text;
            this.type = type;
            this.line = line;
            this.row = row;
        }

        public override string ToString() =>
            $"'{text}' ({type}) [{line}, {row}]";
    }
}
