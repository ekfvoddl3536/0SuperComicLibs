using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SuperComicLib.Collections;
using SuperComicLib.Runtime;

namespace SuperComicLib.CodeDesigner
{
    internal sealed class ScanActionEnumerator : IEnumerable<Token>, IEnumerator<Token>
    {
        private StreamReader sr;
        private IEnumerablePair<string, TokenType> sys;
        private IEnumerablePair<string, TokenType> ids;
        private Token current;
        private int line;
        private int row;
        private IExceptionHandler handler;

        public ScanActionEnumerator(StreamReader sr, IEnumerablePair<string, TokenType> sys, IEnumerablePair<string, TokenType> ids, IExceptionHandler handler)
        {
            this.sys = sys;
            this.ids = ids;
            this.sr = sr;
            this.handler = handler;

            line = 1;
            row = 1;
        }

        #region interface
        public IEnumerator<Token> GetEnumerator() => this;
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public Token Current => current;
        object IEnumerator.Current => current;

        public bool MoveNext()
        {
            StreamReader sr = this.sr;
        loop:
            if (sr.EndOfStream)
                return false;

            if (sr.EndOfLine()) // newline
            {
                line++;
                row = 1;
                goto loop;
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

            if (char.IsLetter(read)) // start
                TestKeyword(LexIdentity(read, out temp), temp);
            else if (read == '_' || read == '@') // 무조건 identity
            {
                move = LexIdentity(read, out temp);

                current = new Token(temp, TokenType.id, line, row);
                row += move;
            }
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
                return LexNumber(read);
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
                    return LexNumber((char)move);
            }
            else
            {
                move = LexSymbol(read, out temp, out TokenType type);
                if (move < 0)
                    return false;

                current = new Token(temp, type, line, row);
                row += move;
            }

            return true;
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
            StreamReader sr = this.sr;

            char* ptr = stackalloc char[8]; // 최대 8개의 심볼을 인식한다
            ptr[0] = read;

            int ptrcnt = 1;

            IEnumerablePair<string, TokenType> e1 = sys;

            string key = null;
            TokenType tt = 0;

        loop:
            int eqcnt = 0;

            while (e1.MoveNext())
                if (FnCHAR.StartsWith(ptr, ptrcnt, e1.Item1))
                {
                    eqcnt++;

                    key = e1.Item1;
                    tt = e1.Item2;
                }

            e1.Reset();
            if (eqcnt == 0)
                goto error;
            else if (eqcnt > 1)
            {
                if (ptrcnt == 8)
                    goto error;

                ptr[ptrcnt++] = (char)sr.Read();
                goto loop;
            }

            temp = key;
            type = tt;
            return ptrcnt;

        error:
            handler.Fail(FMSG.L_C2);
            temp = null;
            type = 0;
            return -1;
        }

        private bool LexRealNumber(StringBuilder start, char read)
        {
            StreamReader sr = this.sr;

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
                return EmitFLOAT(sb.Length, sb.ToString());
            }
            else if (read == 'd')
                sr.Read();

            return EmitDOUBLE(sb.Length, sb.ToString());
        }

        private bool LexRealNumberExponent(StringBuilder sb)
        {
            StreamReader sr = this.sr;
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
                return EmitFLOAT(sb.Length, sb.ToString());
            }
            else if (read == 'd')
                sr.Read();

            return EmitDOUBLE(sb.Length, sb.ToString());
        }

        private bool LexNumber(char read)
        {
            StreamReader sr = this.sr;

            StringBuilder sb = new StringBuilder();
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
                        ? EmitULONG(sb.Length, sb.ToString()) 
                        : EmitUINT(sb.Length, sb.ToString());
                }
                else if (read == 'l')
                {
                    sr.Read();
                    if (long.TryParse(sb.ToString(), out long result))
                    {
                        current = new Token(result.ToString(), TokenType.literal_int_8, line, row);
                        row += sb.Length;
                        return true;
                    }
                }
                else if (int.TryParse(sb.ToString(), out int result))
                {
                    current = new Token(result.ToString(), TokenType.literal_int_4, line, row);
                    row += sb.Length;
                    return true;
                }
                else
                    handler.Fail(FMSG.L_NumOver);
            }

            return false;
        }

        private bool EmitDOUBLE(int len, string text)
        {
            if (double.TryParse(text, out double result))
            {
                current = new Token(result.ToString(), TokenType.literal_real_8, line, row);
                row += len;
                return true;
            }
            return false;
        }

        private bool EmitFLOAT(int len, string text)
        {
            if (float.TryParse(text, out float result))
            {
                current = new Token(result.ToString(), TokenType.literal_real_4, line, row);
                row += len;
                return true;
            }
            return false;
        }

        private bool EmitUINT(int len, string text)
        {
            if (uint.TryParse(text, out uint result))
            {
                current = new Token(((int)result).ToString(), TokenType.literal_int_4, line, row);
                row += len;
                return true;
            }
            return false;
        }

        private bool EmitULONG(int len, string text)
        {
            if (ulong.TryParse(text, out ulong result))
            {
                current = new Token(((long)result).ToString(), TokenType.literal_int_8, line, row);
                row += len;
                return true;
            }
            return false;
        }

        private bool LexString()
        {
            StreamReader sr = this.sr;

            StringBuilder sb = new StringBuilder();
            int numread = 2;

        loop:
            int read = sr.Read();
            if (read == '\"') // end
            {
                current = new Token(sb.ToString(), TokenType.literal_str, line, row);
                row += numread;
                return true;
            }

            int tmp = LexCharater(read, out read);
            if (tmp < 0)
                return false;

            numread += tmp;
            sb.Append((char)read);
            goto loop;
        }

        private void TestKeyword(int readed, string text)
        {
            IEnumerablePair<string, TokenType> e1 = ids;
            while (e1.MoveNext())
                if (e1.Item1 == text)
                {
                    current = new Token(text, e1.Item2, line, row);
                    row += readed;

                    e1.Reset();

                    return;
                }

            e1.Reset();

            current = new Token(text, TokenType.id, line, row);
            row += readed;
        }

        private int LexIdentity(char read, out string result)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(read);

            StreamReader sr = this.sr;

            int tmprow = 1;
        loop:
            read = (char)sr.Peek();
            if (char.IsLetterOrDigit(read) && (!char.IsSymbol(read) || read == '_'))
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
            StreamReader sr = this.sr;

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

            // 유니코드
            int* ptr = stackalloc int[4];
            int pos = 0;
            int tmp;

        loop:
            read2 = sr.Read();
            if (read2 != '\'') // end
            {
                if (pos == 4)
                {
                    // over
                    result = 0;
                    handler.Fail(FMSG.L_C1);
                    return -1;
                }

                tmp = ParseHex(read2);
                if (tmp < 0)
                {
                    // invalid char
                    result = 0;
                    handler.Fail(FMSG.L_C1);
                    return -1;
                }

                ptr[pos++] = tmp;
                goto loop;
            }
            if (pos == 0)
            {
                // invalid unicode
                result = 0;
                handler.Fail(FMSG.L_C1);
                return -1;
            }

            tmp = 0;
            for (int x = 0; x < pos; x++)
                tmp = tmp * 16 + ptr[x];

            result = tmp;
            return numread + pos + 2; // '\u(1..4)'
        }

        private static int ParseHex(int value) =>
            value.IsRngIn('A', 'F') || value.IsRngIn('a', 'f')
            ? (value & 7) + 9
            : value.IsRngIn('0', '9')
            ? value - '0'
            : -1;
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
            }
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
