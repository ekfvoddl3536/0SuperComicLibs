// MIT License
//
// Copyright (c) 2019-2022 SuperComic (ekfvoddl3535@naver.com)
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

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
