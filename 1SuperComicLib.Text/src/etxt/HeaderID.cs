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
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using SuperComicLib.CodeContracts;

namespace SuperComicLib.Text
{
    [StructLayout(LayoutKind.Sequential, Pack = sizeof(long) << 1)]
    public readonly unsafe struct HeaderID : IEquatable<HeaderID>
    {
        public readonly string FullName;
        public readonly uint ScopeLength;

        public HeaderID(string fullName)
        {
            if (string.IsNullOrEmpty(fullName))
                throw new ArgumentException("null or empty string", nameof(fullName));

            if (fullName.Length > ushort.MaxValue)
                throw new ArgumentException("Too long string.", nameof(fullName));

            FullName = fullName;
            ScopeLength = CheckID(fullName);
        }

        internal static uint CheckID([DisallowNull] string fullName)
        {
            var scope = 0u;
            fixed (char* str = fullName)
            {
                var si = 0;
                var di = fullName.IndexOf('.');
                while (di >= 0)
                {
                    if (IsValidateName(str + si, str + di) == false)
                        goto error1;

                    si = di + 1;
                    di = fullName.IndexOf('.', si);

                    ++scope;
                }

                if (IsValidateName(str + si, str + fullName.Length) == false)
                    goto error1;
            }

            return scope;

        error1:
            throw new ArgumentException("invalid character in string!", nameof(fullName));
        }

        public HierarchyName this[int index] => 
            index == 0
            ? First()
            : First()[index];

        public HierarchyName First()
        {
            var str = FullName;
            var idx = str.IndexOf('.');

            return
                idx >= 0
                ? new HierarchyName(str, new Range(0, idx))
                : new HierarchyName(str);
        }

        public string[] ToSeparatedNameArray()
        {
            var name = FullName;
            var res = new string[ScopeLength + 1];

            int si = 0, di;
            for (int i = 0; i < res.Length - 1; ++i)
            {
                di = name.IndexOf('.', si);

                res[i] = name.Substring(si, di - si);

                si = di + 1;
            }

            res[ScopeLength] =
                res.Length == 1
                ? name
                : name.Substring(si);

            return res;
        }

        public bool Equals(HeaderID other) => FullName == other.FullName;

        public override bool Equals(object obj) => FullName.Equals(obj);
        public override int GetHashCode() => FullName.GetHashCode();
        public override string ToString() => FullName;

        private static bool IsValidateName(char* ps, char* end)
        {
            // first (_, ascii letter)
            const uint LETTER_MAX = 'z' - 'a';
            const uint DIGIT_MAX = '9' - '0';

            if (ps == end || *ps != '_' && (*ps | 0x20u) - 'a' > LETTER_MAX)
                return false;

            while (++ps != end)
            {
                uint t = *ps;
                if ((t | 0x20u) - 'a' > LETTER_MAX &&
                    t - '0' > DIGIT_MAX &&
                    t != '_')
                    return false;
            }

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator HeaderID(string fullName) => new HeaderID(fullName);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(HeaderID left, HeaderID right) => left.FullName == right.FullName;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(HeaderID left, HeaderID right) => left.FullName != right.FullName;
    }

    // String, Number(
    //public sealed unsafe class ETxtReader : IDisposable
    //{
    //    private StreamReader _reader;
    //    private StringBuilder _builder;

    //    public ETxtNode Read()
    //    {
    //        ThrowIfDispoed();

    //        var r = _reader;
    //        var line = r.MoveNext();

    //        var idx = line.IndexOf(':');
    //        if (idx <= 0 || idx + 1 == line.Length) // not found -or- empty id -or- empty value
    //            throw new InvalidOperationException("Invalid format");

    //        var id = line.Substring(0, idx);
    //        var value = ParseValue(line, idx + 1, r, _builder);
    //    }

    //    private static object ParseValue(string s, int i, StreamReader r, StringBuilder b)
    //    {
    //        fixed (char* p = s)
    //        {
    //            var span = new NativeSpan<char>(p + i, s.Length - i).TrimStart();

    //            if (span[0] == '[') // list -or- map
    //            {

    //            }
    //            else if (span[0] == '"')
    //            {
    //                if (span.Length >= 3 &&
    //                    span[1] == '"' && span[2] == '"') // """
    //                {
    //                    span = span.Slice(3);
    //                    return ReadMLCore(span, r, b);
    //                }
    //            }
    //        }
    //    }

    //    private static string ReadMLCore(in NativeSpan<char> curr, StreamReader r, StringBuilder b)
    //    {
    //        b.Clear();

    //        b.Remove()
    //    }

    //    private static bool ReadLineMLCore(in NativeSpan<char> s, StringBuilder b)
    //    {
    //        const long
    //            STATE_NORMAL = 0,
    //            STATE_ES_SEQ = 1;

    //        long state = 0;
    //        var prev = s.Source;
    //        var iter = s.Source;
    //        var end = s.Source + (long)s.Length;
    //        for (; iter != end; ++iter)
    //            if (state == STATE_ES_SEQ)
    //            {
    //                b.Append(prev, (int)(iter - prev - 1));

    //                b.Append(GetEscapeCharacter(*iter));

    //                prev = iter + 1;

    //                state = STATE_NORMAL;
    //            }
    //            else if (*iter == '\\') // escape sequence
    //                state = STATE_ES_SEQ;
    //            else if (*iter == '"')
    //                break;

    //        if (state != STATE_NORMAL)
    //            throw new InvalidOperationException("invalid value");

    //        state = end - iter;
    //        if (state >= 3 && (*(int*)(iter - 3) | iter[-1]) == '"') // """
    //        {
    //            b.Append(prev, (int)(iter - prev - 3));
    //            return true;
    //        }
    //        else
    //        {
    //            b.Append(prev, (int)(iter - prev));
    //            return state != 0;
    //        }
    //    }

    //    private static char GetEscapeCharacter(char v)
    //    {
    //        switch (char.ToLower(v))
    //        {
    //            case 'a':
    //                return '\a';

    //            case 'b':
    //                return '\b';

    //            case 'f':
    //                return '\f';

    //            case 'n':
    //                return '\n';

    //            case 'r':
    //                return '\r';

    //            case 't':
    //                return '\t';

    //            case 'v':
    //                return '\v';

    //            default:
    //                return v;
    //        }
    //    }

    //    private void ThrowIfDispoed()
    //    {
    //        if (_reader == null)
    //            throw new ObjectDisposedException(nameof(ETxtReader));
    //    }

    //    public void Dispose()
    //    {
    //        _reader?.Dispose();
    //        _reader = null;

    //        GC.SuppressFinalize(this);
    //    }
    //}
}
