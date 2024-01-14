// MIT License
//
// Copyright (c) 2019-2024. SuperComic (ekfvoddl3535@naver.com)
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

namespace SuperComicLib.IO
{
    public sealed class ETxtReader : IDisposable
    {
        private const string ERROR_FORMAT = "Unexpected format encountered in the data.";
        private const string ERROR_EOF = "Reached the end of the file unexpectedly while processing data.";

        private StreamReader _reader;
        private StringBuilder _builder;

        public ETxtReader(string filePath) : this(new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read), Encoding.UTF8)
        {
        }

        public ETxtReader(Stream stream) : this(stream, Encoding.UTF8)
        {
        }

        public ETxtReader(Stream stream, Encoding encoding)
        {
            _reader = new StreamReader(stream, encoding, false);

            _builder = new StringBuilder(0x1_0000); // 64KiB
        }

        public bool EndOfStream => _reader.EndOfStream;

        public KeyValuePair<HeaderID, object> Read()
        {
            var r = _reader;
            int c = NextChar(r);

            if (c < 0)
                return default;

            // get id
            var sb = _builder;
            
            sb.Clear();

            sb.Append((char)c);

            HeaderID id = ParseId(sb, r);

            object value = ParseValue(sb, r);

            return new KeyValuePair<HeaderID, object>(id, value);
        }

        private object ParseValue(StringBuilder sb, StreamReader r)
        {
            sb.Clear();

            int c = SkipWhitespaces(r);
            if (c < 0)
                throw new EndOfStreamException(ERROR_EOF);

            if (c == '"')
                return ParseStringValue(sb, r);
            else if (c == '[')
                return ParseListValue(sb, r);
            else
                throw new InvalidOperationException(ERROR_FORMAT);
        }

        private List<string> ParseListValue(StringBuilder sb, StreamReader r)
        {
            var list = new List<string>();

            int c = SkipWhitespaces(r);
            if (c == ']')
                return list;

            for (; ; )
            {
                if (c != '"')
                    throw new InvalidOperationException(ERROR_FORMAT);

                sb.Clear();
                list.Add(ParseStringValue(sb, r));

                c = SkipWhitespaces(r);
                if (c == ']')
                    return list;
                else if (c != ',')
                    throw new InvalidOperationException(ERROR_FORMAT);

                // ,
                c = SkipWhitespaces(r);
            }
        }

        private string ParseStringValue(StringBuilder sb, StreamReader r)
        {
            int c = r.Read();
            if (c < 0)
                throw new EndOfStreamException(ERROR_EOF);

            if (c == '"')
                return string.Empty;

            var nl = Environment.NewLine;
            for (; ; )
            {
                if (c == '"')
                    break;
                else if (c == '\\')
                    sb.Append(GetEscapeSequence(r.Read()));
                else if (c == '\r')
                {
                    if (r.Peek() == '\n') // CR LF
                        r.Read();

                    sb.Append(nl); // CR
                }
                else if (c == '\n')
                    sb.Append(nl); // LF
                else
                    sb.Append((char)c);

                c = r.Read();
                if (c < 0)
                    throw new EndOfStreamException(ERROR_EOF);
            }

            return sb.ToString();
        }

        private char GetEscapeSequence(int c)
        {
            if (c < 0)
                throw new EndOfStreamException(ERROR_EOF);

            switch (c)
            {
                case 'a':
                    return '\a';

                case 'b':
                    return '\b';

                case 'f':
                    return '\f';

                case 'n':
                    return '\n';

                case 'r':
                    return '\r';

                case 't':
                    return '\t';

                case 'v':
                    return '\v';

                default:
                    return (char)c;
            }
        }

        private HeaderID ParseId(StringBuilder sb, StreamReader r)
        {
            for (int c; ;)
            {
                if ((c = r.Read()) < 0)
                    throw new EndOfStreamException(ERROR_EOF);

                if (sb.Length > ushort.MaxValue)
                    throw new OutOfMemoryException();

                if (c == ':')
                    break;

                sb.Append((char)c);
            }

            return new HeaderID(sb.ToString().TrimEnd());
        }

        private static int NextChar(StreamReader r)
        {
            for (; ; )
            {
                int c = SkipWhitespaces(r);

                if (c == '#')
                    NextLine(r);
                else
                    return c;
            }
        }

        private static int SkipWhitespaces(StreamReader r)
        {
            int c;
            do
                if ((c = r.Read()) < 0)
                    return -1;
            while (char.IsWhiteSpace((char)c));

            return c;
        }

        private static void NextLine(StreamReader r)
        {
            for (; ; )
            {
                int c = r.Read();
                if (c < 0)
                    return;
                else if (c == '\r')
                {
                    if (r.Peek() == '\n')
                        r.Read();

                    break;
                }
                else if (c == '\n')
                    break;
            }
        }

        public void Dispose()
        {
            if (_reader != null)
            {
                _reader.Dispose();
                _reader = null;

                _builder = null;
            }

            GC.SuppressFinalize(this);
        }
    }
}
