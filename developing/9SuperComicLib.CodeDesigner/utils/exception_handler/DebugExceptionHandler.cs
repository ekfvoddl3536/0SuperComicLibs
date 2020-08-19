namespace SuperComicLib.CodeDesigner
{
    internal sealed class DebugExceptionHandler : IExceptionHandler
    {
        public static readonly IExceptionHandler Default = new DebugExceptionHandler();

        private int fc;
        private int wc;
        private int mc;

        private DebugExceptionHandler() { }

        public int FailCount => fc;
        public int WarnCount => wc;
        public int MsgCount => mc;

        public void Fail(string value)
        {
            System.Diagnostics.Debug.Fail(value);
            fc++;
        }

        public void Msg(string value)
        {
            System.Diagnostics.Debug.WriteLine(value);
            mc++;
        }

        public void Warn(string value)
        {
            System.Diagnostics.Debug.WriteLine(value);
            wc++;
        }

        public void Reset()
        {
            fc = 0;
            wc = 0;
            mc = 0;
        }
    }
}
