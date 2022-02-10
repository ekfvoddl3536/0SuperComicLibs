using System;
using System.Collections;
using System.Collections.Generic;

namespace SuperComicLib.CodeDesigner
{
    public sealed class GString : IEnumerator<string>
    {
        private string src;
        private string next;

        public GString(string src) => this.src = src.Trim();

        public int GetGroupCount()
        {
            int cnt = 0;

            string clone = new string(src.ToCharArray());
            while (MoveNext())
                cnt++;

            src = clone;

            return cnt;
        }

        public string Current => next;
        object IEnumerator.Current => next;

        public bool MoveNext()
        {
            if (string.IsNullOrWhiteSpace(src))
                return false;

            char first = src[0];
            return 
                first == '\'' 
                ? FindNext('\'')
                : first == '<' 
                ? FindNext('>')
                : FindNextWhitespace();
        }

        private bool FindNextWhitespace()
        {
            int x = 0;

        loop:
            if (char.IsWhiteSpace(src[x++]))
            {
                next = src.Substring(0, x).Trim();
                src = src.Substring(x).Trim();
            }
            else if (x < src.Length)
                goto loop;
            else
            {
                next = src;
                src = string.Empty;
            }

            return true;
        }

        private bool FindNext(char v)
        {
            if (src.Length < 3) // 최소 문자 수
                return false;

            int x = 0;
            int z = 2;

        loop:
            x++;
            if (src[z] == v)
            {
                if (x == 0)
                    return false;

                next = src.Substring(1, x).Trim();
                src = src.Substring(z + 1).Trim();

                return true;
            }

            if (++z < src.Length)
                goto loop;
            else
                return false;
        }

        public void Reset() { }

        public void Dispose()
        {
            src = null;
            next = null;

            GC.SuppressFinalize(this);
        }
    }
}
