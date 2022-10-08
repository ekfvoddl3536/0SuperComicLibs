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

        public FCharEnumerator(FRngString value) : this(value.str, value.sidx, value.eidx)
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
