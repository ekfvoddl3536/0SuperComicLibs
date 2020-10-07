using System;
using SuperComicLib.Core;

namespace SuperComicLib.CodeDesigner
{
    public sealed class Token : IEquatable<Token>
    {
        public static Token Empty => new Token(string.Empty, 0, 0, 0);

        public readonly string text;
        public readonly TokenType type;
        public readonly int line;
        public readonly int row;
        public object state;

        public Token(string text, TokenType type, int line, int row)
        {
            this.text = text;
            this.type = type;
            this.line = line;
            this.row = row;
        }

        public Token(TokenType type, int line, int row, object state)
        {
            this.state = state;
            this.type = type;
            this.line = line;
            this.row = row;

            text = state.ToString();
        }

        public Token(string text, TokenType type, int line, int row, object state) : this(text, type, line, row) =>
            this.state = state;

        public bool Equals(Token other) =>
            FastEquals(other) &&
            other.text == text &&
            other.line == line &&
            other.row == row;

        public bool FastEquals(Token other) =>
            other != null &&
            other.type == type;

        public override int GetHashCode()
        {
            int result = 3;
            IntHash.Combine(ref result, (int)type, 0xFE6D); // 65133
            IntHash.CombineMOD(ref result, 0x1FFFF);
            IntHash.CombineMOD(ref result, line);
            IntHash.CombineMOD(ref result, row);

            return result;
        }

        public override string ToString() =>
            type == TokenType.EOL
            ? $"'EOL' ({type}) [{line}, {row}]"
            : $"'{text}' ({type}) [{line}, {row}]";
    }
}
