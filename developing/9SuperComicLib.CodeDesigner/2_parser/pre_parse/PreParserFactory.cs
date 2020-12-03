namespace SuperComicLib.CodeDesigner
{
    public static class PreParserFactory
    {
        public static ITokenEnumerator Default(ITokenEnumerator source) => new DefPPAE(source);
    }
}
