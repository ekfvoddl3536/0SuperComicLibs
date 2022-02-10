namespace SuperComicLib.CodeDesigner
{
    public static class SymbolMapFactory
    {
        public static ISymbolMap Default =>
#if DEBUG
            Debug;
#else
            new InternalSymbolMap();
#endif

#if DEBUG
        public static ISymbolMap Debug => new DebugSymbolMap();
#endif
    }
}
