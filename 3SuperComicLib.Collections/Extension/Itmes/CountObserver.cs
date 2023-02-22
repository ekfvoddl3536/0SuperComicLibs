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

using System.Collections;

namespace SuperComicLib.Collections
{
    internal sealed class CountObserver : ICountObserver
    {
        private ICollection collection;
        private int prev_cnt;

        public CountObserver(ICollection collection)
        {
            this.collection = collection;
            prev_cnt = collection.Count;
        }

        public bool IsUpdated => prev_cnt != collection.Count;
        public bool IsAdded => prev_cnt < collection.Count;
        public bool IsRemoved => prev_cnt > collection.Count;
        public int Count => collection.Count;

        public void ChangeObserveTarget(object target)
        {
            if (target is ICollection col)
            {
                collection = col;
                prev_cnt = col.Count;
            }
        }

        public void UpdateToLatest() => prev_cnt = collection.Count;
    }
}
