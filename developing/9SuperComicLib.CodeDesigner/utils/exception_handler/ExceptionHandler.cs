namespace SuperComicLib.CodeDesigner
{
    internal sealed class ExceptionHandler : IExceptionHandler
    {
        public static readonly IExceptionHandler Default = new ExceptionHandler();

        private int fc;
        private int wc;
        private int mc;

        private ExceptionHandler() { }

        public int FailCount => fc;
        public int WarnCount => wc;
        public int MsgCount => mc;

        public void Fail(string value) => fc++;

        public void Msg(string value) => mc++;

        public void Warn(string value) => wc++;

        public void Reset()
        {
            fc = 0;
            wc = 0;
            mc = 0;
        }
    }
}
