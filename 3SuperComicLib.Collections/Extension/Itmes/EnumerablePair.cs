// MIT License
//
// Copyright (c) 2019-2023. SuperComic (ekfvoddl3535@naver.com)
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

namespace SuperComicLib.Collections
{
    internal sealed class EnumerablePair<T1, T2> : IEnumeratorPair<T1, T2>
    {
        private T1[] array;
        private T2[] other;
        private int idx;
        private int max;

        public EnumerablePair(T1[] array, T2[] other, int max)
        {
            this.array = array;
            this.other = other;

            this.max = max;
            idx = -1;
        }

        public T1 Item1 => array[idx];
        public T2 Item2 => other[idx];

        public bool MoveNext() => ++idx < max;

        public void Reset() => idx = -1;

        public void Dispose()
        {
            array = null;
            other = null;
            idx = 0;
            max = 0;

            GC.SuppressFinalize(this);
        }
    }
}