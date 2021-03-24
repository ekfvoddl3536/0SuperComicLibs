using System;
using System.Collections;
using System.Collections.Generic;

namespace SuperComicLib.Text
{
    internal sealed class FCharEnumerator : IEnumerator<char>
    {
        private string str;
        private int sidx;
        private int eidx;
        private int cidx;

        public FCharEnumerator(in FRngString value) : this(value.str, value.sidx, value.eidx)
        {
        }

        public FCharEnumerator(string str, int sidx, int eidx)
        {
            this.str = str;
            this.eidx = eidx;

            cidx = this.sidx = sidx - 1;
        }

        object IEnumerator.Current => Current;
        public char Current => str[cidx];

        public bool MoveNext() => ++cidx < eidx;
        public void Reset() => cidx = sidx;

        public void Dispose()
        {
            str = null;
            sidx = 0;
            eidx = 0;
            cidx = 0;

            GC.SuppressFinalize(this);
        }
    }
}
