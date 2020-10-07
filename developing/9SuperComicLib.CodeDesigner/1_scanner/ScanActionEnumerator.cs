using System;
using System.Collections;
using System.IO;
using System.Text;
using SuperComicLib.Collections;
using SuperComicLib.IO;
using SuperComicLib.Runtime;

namespace SuperComicLib.CodeDesigner
{
    internal sealed class ScanActionEnumerator : ITokenEnumerator
    {
        private static readonly Token endtoken = new Token("$", (TokenType)ExpressInt.end_symbol, -1, -1);

        private CStreamReader sr;
        private IEnumerablePair<string, TokenType> sys;
        private IEnumerablePair<string, TokenType> ids;
        private Token current;
        private int line;
        private int row;
        private IExceptionHandler handler;
        private ITypeMap typeMap;

        public ScanActionEnumerator(
            CStreamReader sr,
            IEnumerablePair<string, TokenType> sys,
            IEnumerablePair<string, TokenType> ids,
            IExceptionHandler handler,
            ITypeMap typeMap)
        {
            this.sys = sys;
            this.ids = ids;
            this.sr = sr;
            this.handler = handler;
            this.typeMap = typeMap;

            line = 1;
            row = 1;
        }

        #region interface
        public bool IsEnd => sr.EndOfStream && current == endtoken;

        public Token Current => current;
        object IEnumerator.Current => current;

        #region method - 0
        public bool MoveNext()
        {
            CStreamReader sr = this.sr;

        loop:
            if (sr.EndOfStream)
            {
                current = endtoken;
                return true;
            }

            if (sr.EndOfLine()) // newline
            {
                if (row == 1)
                {
                    line++;
                    goto loop;
                }

                current = new Token(Environment.NewLine, TokenType.EOL, line, row);

                line++;
                row = 1;

                return true;
                // goto loop;
            }

            char read = (char)sr.Read();
            if (char.IsWhiteSpace(read)) // not-allowed
            {
                row++;
                goto loop;
            }

            string temp;
            int move;
            StringBuilder sb;

            if (IsStartIdentity(read)) // start
                TestKeyword(LexIdentity(read, out temp), temp);
            else if (read == '\'')
            {
                move = LexCharater(sr.Read(), out int result);
                if (move < 0)
                    return false;

                current = new Token(result.ToString(), TokenType.literal_int_4, line, row);
                row += move;
            }
            else if (read == '\"')
                return LexString();
            else if (read == '.' && char.IsDigit((char)sr.Peek()))
            {
                sb = new StringBuilder();
                sb.Append("0.");
                return LexRealNumber(sb, (char)sr.Read());
            }
            else if (char.IsDigit(read))
                return LexNumber(read, char.MinValue);
            else if ((read == '-' || read == '+') && (sr.Peek() == '.' || sr.Peek().IsRngIn('0', '9')))
            {
                move = sr.Read();
                if (move == '.')
                {
                    if (char.IsDigit((char)sr.Peek()))
                    {
                        sb = new StringBuilder();
                        sb.Append(read);
                        sb.Append('.');
                        return LexRealNumber(sb, (char)sr.Read());
                    }
                }
                else
                    return LexNumber((char)move, read);
            }
            else
            {
                move = LexSymbol(read, out temp, out TokenType type);
                if (move <= 0)
                    return false;

                current = new Token(temp, type, line, row);
                row += move;
            }

            return handler.FailCount == 0;
        }

        public void Reset()
        {
            sr.BaseStream.Seek(0, SeekOrigin.Begin);
            sys.Reset();

            current = null;
        }
        #endregion

        #region method
        private unsafe int LexSymbol(char read, out string temp, out TokenType type)
        {
            // step 1. 한 문자 읽기
            // step 2. StartsWith이 일치하는 모든 심볼 찾기
            // step 3-true. 단 1개의 심볼만 일치하는 경우 반환
            // step 3-false. 실패
            CStreamReader sr = this.sr;

            char* ptr = stackalloc char[4]; // 최대 4개의 심볼을 인식한다
            ptr[0] = read;

            IEnumerablePair<string, TokenType> e1 = sys;

            string key = null;
            TokenType tt = 0;

            int ptrcnt = 1;
            bool found = false;

        loop:
            int swcnt = 0;

            while (e1.MoveNext())
                if (e1.Item1.StartsWith(ptr, ptrcnt))
                {
                    swcnt++;

                    if (FnCHAR.Equals(ptr, ptrcnt, e1.Item1))
                    {
                        found = true;

                        key = e1.Item1;
                        tt = e1.Item2;
                    }
                }

            e1.Reset();
            if (swcnt == 0 && !found)
            {
                handler.Fail(FMSG.L_C2);
                temp = null;
                type = 0;
                return -1;
            }
            else if (swcnt > 1)
            {
                ptr[ptrcnt] = (char)sr.Peek(ptrcnt - 1);
                ptrcnt++;
                goto loop;
            }
            else if (swcnt == 1 && ptrcnt > 1)
                sr.Move(ptrcnt - 1);

            temp = key;
            type = tt;

            return key.Length;
        }

        private bool LexRealNumber(StringBuilder start, char read)
        {
            CStreamReader sr = this.sr;

            StringBuilder sb = start ?? new StringBuilder();
            sb.Append(read);

        loop:
            read = (char)sr.Peek();
            if (char.IsDigit(read))
            {
                sr.Read();
                sb.Append(read);
                goto loop;
            }

            read = char.ToLower(read);
            if (read == 'e')
            {
                sr.Read();
                return LexRealNumberExponent(sb);
            }
            else if (read == 'f') // float
            {
                sr.Read();
                return PrsFLOAT(sb.Length, sb.ToString());
            }
            else if (read == 'd')
                sr.Read();

            return PrsDOUBLE(sb.Length, sb.ToString());
        }

        private bool LexRealNumberExponent(StringBuilder sb)
        {
            CStreamReader sr = this.sr;
            int read = sr.Peek();
            if (read.IsRngIn('0', '9'))
            {
                sr.Read();
                sb.Append(read);
            }
            if (read == '+' || read == '-') // first
            {
                sr.Read();
                sb.Append(read);
            }
            else
            {
                // invalid
                handler.Fail(FMSG.L_C2);
                return false;
            }

        loop:
            read = sr.Peek();
            if (read.IsRngIn('0', '9'))
            {
                sr.Read();
                sb.Append(read);
                goto loop;
            }

            read &= 0xFFDF; // to lower
            if (read == 'f') // float
            {
                sr.Read();
                return PrsFLOAT(sb.Length, sb.ToString());
            }
            else if (read == 'd')
                sr.Read();

            return PrsDOUBLE(sb.Length, sb.ToString());
        }

        private bool LexNumber(char read, char sign)
        {
            CStreamReader sr = this.sr;

            StringBuilder sb = new StringBuilder();
            if (sign != 0)
                sb.Append(sign);

            sb.Append(read);

        loop:
            read = (char)sr.Peek();
            if (char.IsDigit(read))
            {
                sr.Read();
                sb.Append(read);
                goto loop;
            }

            if (read == '.') // oh its real number
            {
                sr.Read();
                return LexRealNumber(sb, read);
            }
            else if (char.ToLower(read) == 'e')
            {
                sr.Read();
                return LexRealNumberExponent(sb);
            }
            else
            {
                read = char.ToLower(read);
                if (read == 'u') // uint
                {
                    sr.Read();
                    return 
                        sr.Peek() == 'l' 
                        ? PrsULONG(sb.Length, sb.ToString()) 
                        : PrsUINT(sb.Length, sb.ToString());
                }
                else if (read == 'l')
                {
                    sr.Read();
                    if (long.TryParse(sb.ToString(), out long v1))
                    {
                        current = new Token(TokenType.literal_int_8, line, row, v1);
                        row += sb.Length;
                        return true;
                    }
                }
                else if (int.TryParse(sb.ToString(), out int v2))
                {
                    current = new Token(TokenType.literal_int_4, line, row, v2);
                    row += sb.Length;
                    return true;
                }
                else
                    handler.Fail(FMSG.L_NumOver);
            }

            return false;
        }

        private bool PrsDOUBLE(int len, string text)
        {
            if (double.TryParse(text, out double result))
            {
                current = new Token(TokenType.literal_real_8, line, row, result);
                row += len;
                return true;
            }
            return false;
        }

        private bool PrsFLOAT(int len, string text)
        {
            if (float.TryParse(text, out float result))
            {
                current = new Token(TokenType.literal_real_4, line, row, result);
                row += len;
                return true;
            }
            return false;
        }

        private bool PrsUINT(int len, string text)
        {
            if (uint.TryParse(text, out uint result))
            {
                current = new Token(TokenType.literal_int_4, line, row, (int)result);
                row += len;
                return true;
            }
            return false;
        }

        private bool PrsULONG(int len, string text)
        {
            if (ulong.TryParse(text, out ulong result))
            {
                current = new Token(TokenType.literal_int_8, line, row, (long)result);
                row += len;
                return true;
            }
            return false;
        }

        private bool LexString()
        {
            CStreamReader sr = this.sr;

            StringBuilder sb = new StringBuilder();
            int numread = 2;

        loop:
            int read = sr.Read();
            if (read == '\\') // unicode
            {
                int pos = LexUnicodeCHAR(sr, out read);
                if (pos < 0)
                    return false;

                numread += pos;
                sb.Append((char)read);
            }
            else if (read == '\"') // end
            {
                current = new Token(sb.ToString(), TokenType.literal_str, line, row);
                row += numread;
                return true;
            }
            else
            {
                numread++;
                sb.Append((char)read);
            }
            goto loop;
        }

        private void TestKeyword(int readed, string text)
        {
            if (readed == 0)
            {
                handler.Fail(FMSG.F1);
                return;
            }
            else if (typeMap.Contains(text))
            {
                ParseType(text);
                goto ex2;
            }

            IEnumerablePair<string, TokenType> e1 = ids;
            while (e1.MoveNext())
                if (e1.Item1 == text)
                {
                    current = new Token(text, e1.Item2, line, row);
                    goto exit;
                }

            current = new Token(text, TokenType.id, line, row);

        exit:
            e1.Reset();

        ex2:
            row += readed;
        }

        private bool ParseType(string text)
        {
            CStreamReader sr = this.sr;

            if (sr.Peek() == '[' && sr.Peek(1) == ']')
            {
                StringBuilder sbLR = new StringBuilder();
                sbLR.Append("[]");

                int idx = 2;

            loop2:
                if (sr.Peek(idx) == '[')
                    if (sr.Peek(idx + 1) == ']')
                    {
                        idx += 2;
                        sbLR.Append("[]");
                        goto loop2;
                    }
                    else
                    {
                        handler.Fail(FMSG.F1);
                        return true; // 강제 반환
                    }

                sr.Move(idx);

                current = new Token(text, TokenType.type_array, line, row, idx / 2);
                row += idx;

                return true;
            }
            
            current = new Token(text, TokenType.type, line, row);
            return false;
        }

        private int LexIdentity(char read, out string result)
        {
            StringBuilder sb = new StringBuilder();
            int tmprow;
            if (read != '@')
            {
                tmprow = 1;
                sb.Append(read);
            }
            else
                tmprow = 0;

            CStreamReader sr = this.sr;

        loop:
            read = (char)sr.Peek();
            if (char.IsLetterOrDigit(read) || read == '_')
            {
                sr.Read();

                sb.Append(read);
                tmprow++;
                goto loop;
            }

            result = sb.ToString();
            return tmprow;
        }

        private unsafe int LexCharater(int read, out int result)
        {
            CStreamReader sr = this.sr;

            int read2 = sr.Read();
            int numread = 2; // read, read2

            if (read2 == '\'') // end char
            {
                result = read;
                return numread + 1; // 'X'
            }
            else if (read != '\\')
            {
                // invalid + too long char
                result = 0;
                handler.Fail(FMSG.L_C1);
                return -1;
            }
            else if (read2 != 'u')
            {
                if (sr.Read() != '\'') // read last '
                {
                    // invalid char
                    result = 0;
                    handler.Fail(FMSG.L_C1);
                    return -1;
                }

                result = read2;
                return numread + 2; // '\X'
            }

            int pos = LexUnicodeCHAR(sr, out result);
            if (pos < 0)
                return -1;

            return numread + pos + 2; // '\u(1..4)'
        }

        private unsafe int LexUnicodeCHAR(CStreamReader sr, out int result)
        {
            int read2 = sr.Read();
            if (read2 != 'u')
            {
                result = read2;
                return 1;
            }

            // 유니코드
            int* ptr = stackalloc int[4];
            int pos = 0;

        loop:
            int tmp = ParseHex(sr.Read());
            if (tmp < 0)
            {
                // invalid char
                result = 0;
                handler.Fail(FMSG.L_C1);
                return -1;
            }

            ptr[pos++] = tmp;
            if (pos < 4)
                goto loop;

            tmp = 0;
            for (int x = 0; x < pos; x++)
                tmp = tmp * 16 + ptr[x];

            result = tmp;
            return 5;
        }

        private static int ParseHex(int value) =>
            value.IsRngIn('A', 'F') || value.IsRngIn('a', 'f')
            ? (value & 7) + 9
            : value.IsRngIn('0', '9')
            ? value - '0'
            : -1;

        private static bool IsStartIdentity(char c) => char.IsLetter(c) || c == '@' || c == '_';
        #endregion
        #endregion

        #region dispose
        public void Dispose()
        {
            if (sr != null)
            {
                sr.Close();
                sr = null;
                
                sys.Dispose();
                sys = null;

                ids.Dispose();
                ids = null;

                line = 0;
                row = 0;

                current = null;
                handler = null;
                typeMap = null;
            }
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
