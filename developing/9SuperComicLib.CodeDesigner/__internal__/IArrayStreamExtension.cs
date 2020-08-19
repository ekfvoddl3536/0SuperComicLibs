using SuperComicLib.Collections;

namespace SuperComicLib.CodeDesigner
{
    internal static class IArrayStreamExtension
    {
        public static int NextEOL(this IArrayStream<Token> inst) => NextTT(inst, TokenType.EOL);

        public static int NextTT(this IArrayStream<Token> inst, TokenType value)
        {
            int pos = inst.Position;
            int cnt = 0;

            while (inst.EndOfStream == false && inst.Read().type != value)
                cnt++;

            inst.Position = pos;
            return cnt;
        }

        public static IArrayStream<Token> RangeTT(this IArrayStream<Token> inst, TokenType value) =>
            inst.Read(NextTT(inst, value));
    }
}
