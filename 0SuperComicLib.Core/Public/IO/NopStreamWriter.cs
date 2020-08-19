using System.IO;
using System.Threading.Tasks;

namespace SuperComicLib.Text
{

    public sealed class NopStreamWriter : StreamWriter
    {
        public static readonly NopStreamWriter INSTANCE = new NopStreamWriter();

        private NopStreamWriter() : base(NopStream.INSTANCE, NopEncoding.INSTANCE, 1, true) { }

        public override bool AutoFlush
        {
            get => false;
            set { }
        }

        public override void Flush() { }
        public override void Write(bool value) { }
        public override void Write(char value) { }
        public override void Write(char[] buffer) { }
        public override void Write(char[] buffer, int index, int count) { }
        public override void Write(decimal value) { }
        public override void Write(double value) { }
        public override void Write(float value) { }
        public override void Write(int value) { }
        public override void Write(long value) { }
        public override void Write(object value) { }
        public override void Write(string format, object arg0) { }
        public override void Write(string format, object arg0, object arg1) { }
        public override void Write(string format, object arg0, object arg1, object arg2) { }
        public override void Write(string format, params object[] arg) { }
        public override void Write(string value) { }
        public override void Write(uint value) { }
        public override void Write(ulong value) { }
        public override Task WriteAsync(char value) => null;
        public override Task WriteAsync(char[] buffer, int index, int count) => null;
        public override Task WriteAsync(string value) => null;
        public override Task WriteLineAsync() => null;
        public override Task WriteLineAsync(char value) => null;
        public override Task WriteLineAsync(char[] buffer, int index, int count) => null;
        public override Task WriteLineAsync(string value) => null;
        public override void WriteLine() { }

        public override void Close() { }
        public override Task FlushAsync() => null;
    }
}
