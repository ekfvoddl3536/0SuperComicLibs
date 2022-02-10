namespace SuperComicLib.CodeDesigner
{
    public class TypeNameFixPP : PreParserBase
    {
        protected Token prev;

        public TypeNameFixPP(ITokenEnumerator source, bool ignoreEOL) : base(source, ignoreEOL)
        {
        }

        public override bool MoveNext()
        {
            prev = current;
            bool result = base.MoveNext();
            if (result && 
                prev.type == TokenType.type &&
                current.type == TokenType.type)
                current = new Token(current.text, TokenType.id, current.line, Current.row);

            return result;
        }
    }
}
