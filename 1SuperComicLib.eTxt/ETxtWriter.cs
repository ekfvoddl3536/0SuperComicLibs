// MIT License
//
// Copyright (c) 2023. SuperComic (ekfvoddl3535@naver.com)
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

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SuperComicLib.eTxt
{
    public sealed class ETxtWriter : IDisposable
    {
        private StreamWriter _writer;

        public ETxtWriter(string filePath) : this(filePath, false)
        {
        }

        public ETxtWriter(string filePath, bool overwrite)
        {
            var fs = new FileStream(filePath, overwrite ? FileMode.Create : FileMode.CreateNew, FileAccess.Write);
            _writer = new StreamWriter(fs, Encoding.UTF8);
        }

        public ETxtWriter(Stream stream) : this(stream, Encoding.UTF8)
        {
        }

        public ETxtWriter(Stream stream, Encoding encoding) => 
            _writer = new StreamWriter(stream, encoding);

        public bool AutoFlush
        {
            get => _writer.AutoFlush;
            set => _writer.AutoFlush = value;
        }

        public void Flush() => _writer.Flush();

        public void WriteLine()
        {
            ThrowIfDispoed();

            _writer.WriteLine();
        }

        public void WriteLine(int count)
        {
            ThrowIfDispoed();

            while (--count >= 0)
                _writer.WriteLine();
        }

        public void Comment(string s)
        {
            ThrowIfDispoed();

            if (s == null)
                throw new ArgumentNullException(nameof(s));

            var w = _writer;
            var nl = Environment.NewLine;

            int si = 0,
                di = s.IndexOf(nl);

            while (di >= 0)
            {
                w.Write("# ");
                w.WriteLine(s.Substring(si, di));

                si = di + nl.Length;
                di = s.IndexOf(nl, si);
            }

            w.Write("# ");

            if (si == 0)
                w.WriteLine(s);
            else
                w.WriteLine(s.Substring(si));
        }

        public void Write(HeaderID id, string str_value)
        {
            ThrowIfDispoed();

            if (id.FullName == null)
                throw new ArgumentNullException(nameof(id));

            if (str_value == null)
                throw new ArgumentNullException(nameof(str_value));

            var w = _writer;

            w.Write(id.FullName);
            w.Write(": ");

            WriteSLCore(w, str_value);

            w.WriteLine();
        }

        private static void WriteSLCore(StreamWriter w, string str_value)
        {
            w.Write('"');

            WriteUnescaped(w, str_value);

            w.Write('"');
        }

        private static void WriteUnescaped(StreamWriter w, string str_value)
        {
            var rng = new Range();
            for (; rng.end < str_value.Length; ++rng.end)
            {
                int c = str_value[rng.end];
                if (c == '\\')
                {
                    w.Write(str_value.Substring(rng.start, rng.Length));
                    w.Write('\\');

                    rng.start = rng.end + 1;
                }
                else if (c == '"')
                {
                    w.Write(str_value.Substring(rng.start, rng.Length));

                    w.Write("\\\"");
                    rng.start = rng.end + 1;
                }
            }

            if (rng.start != rng.end)
                w.Write(str_value.Substring(rng.start, rng.Length));
        }
        public void Write(HeaderID id, IEnumerable<string> list)
        {
            ThrowIfDispoed();

            if (id.FullName == null)
                throw new ArgumentException("null or whitespace!", nameof(id));

            if (list == null)
                throw new ArgumentNullException(nameof(list));

            var w = _writer;

            w.Write(id.FullName);
            w.Write(": [");

            var iter = list.GetEnumerator();
            if (iter.MoveNext())
            {
                WriteSLCore(w, iter.Current);

                while (iter.MoveNext())
                {
                    w.Write(", ");
                    WriteSLCore(w, iter.Current);
                }
            }

            w.WriteLine(']');
        }

        private void ThrowIfDispoed()
        {
            if (_writer == null)
                throw new ObjectDisposedException(nameof(ETxtWriter));
        }

        public void Dispose()
        {
            if (_writer != null)
            {
                _writer.Dispose();
                _writer = null;
            }

            GC.SuppressFinalize(this);
        }
    }
}
