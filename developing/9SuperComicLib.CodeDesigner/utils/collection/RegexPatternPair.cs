using System;
using System.Collections;
using System.Collections.Generic;

namespace SuperComicLib.CodeDesigner
{
    public sealed class RegexPatternPair : IDisposable, IEnumerable<KeyValuePair<string, CreateTokenHandler>>
    {
        public string[] regex_patterns;
        public CreateTokenHandler[] token_handlers;
        private int idx;

        public RegexPatternPair(int size)
        {
            regex_patterns = new string[size];
            token_handlers = new CreateTokenHandler[size];
        }

        public void Add(string regex_pattern, CreateTokenHandler handler)
        {
#if DEBUG
            if (idx >= regex_patterns.Length)
                throw new IndexOutOfRangeException();
#endif
            ref int tmp = ref idx;
            regex_patterns[tmp] = regex_pattern;
            token_handlers[tmp] = handler;
            tmp++;
        }

        public void EnsureCapacity(int need_size)
        {
            if (idx + need_size > regex_patterns.Length)
            {
                int i = idx;
                int newmax = i + need_size;

                string[] nr = new string[newmax];
                CreateTokenHandler[] nh = new CreateTokenHandler[newmax];

                ref string[] old0 = ref regex_patterns;
                ref CreateTokenHandler[] old1 = ref token_handlers;

                Array.Copy(old0, nr, i);
                Array.Copy(old1, nh, i);

                old0 = nr;
                old1 = nh;
            }
        }

        public void Dispose()
        {
            ClsArray.DeleteAll(ref regex_patterns);
            ClsArray.DeleteAll(ref token_handlers);
            idx = 0;
        }

        public Enumerator GetEnumerator() => new Enumerator(this);

        IEnumerator<KeyValuePair<string, CreateTokenHandler>> IEnumerable<KeyValuePair<string, CreateTokenHandler>>.GetEnumerator() => GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public struct Enumerator : IEnumerator<KeyValuePair<string, CreateTokenHandler>>
        {
            private RegexPatternPair inst;
            private int idx;
            private int size;

            internal Enumerator(RegexPatternPair inst)
            {
                this.inst = inst;
                size = inst.idx;
                idx = -1;
            }

            public string Pattern => inst.regex_patterns[idx];

            public CreateTokenHandler Handler => inst.token_handlers[idx];

            public KeyValuePair<string, CreateTokenHandler> Current => new KeyValuePair<string, CreateTokenHandler>(Pattern, Handler);

            object IEnumerator.Current => Current;

            public void Dispose()
            {
                inst = null;
                idx = 0;
                size = 0;
            }

            public bool MoveNext()
            {
                idx++;
                return idx < size;
            }

            public void Reset() => idx = -1;
        }
    }
}
