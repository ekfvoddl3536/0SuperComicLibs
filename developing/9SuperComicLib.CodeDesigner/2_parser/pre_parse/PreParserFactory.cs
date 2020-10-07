namespace SuperComicLib.CodeDesigner
{
    public static class PreParserFactory
    {
        public static ITokenEnumerator Default(ITokenEnumerator source) => Default(source, ExceptionHandler.Default);

        public static ITokenEnumerator Default(ITokenEnumerator source, IExceptionHandler handler) => new DefPPAE(source, handler);
    }
}
