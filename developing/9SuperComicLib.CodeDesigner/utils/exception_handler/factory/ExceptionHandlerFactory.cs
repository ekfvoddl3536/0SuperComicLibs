namespace SuperComicLib.CodeDesigner
{
    public static class ExceptionHandlerFactory
    {
        public static IExceptionHandler TNormal => ExceptionHandler.Default;

        public static IExceptionHandler TDebug => DebugExceptionHandler.Default;

        public static IExceptionHandler Default =>
#if DEBUG
            TDebug;
#else
            TNormal;
#endif
    }
}
