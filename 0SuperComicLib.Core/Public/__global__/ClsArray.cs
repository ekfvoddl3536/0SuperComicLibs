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
using System.Collections.Generic;

namespace SuperComicLib
{
    public static class ClsArray
    {
        public static void DeleteAll<T>(ref T[] _arr) where T : class
        {
            for (int x = _arr.Length - 1; x >= 0; x--)
                _arr[x] = null;

            _arr = null;
        }

        public static void DisposeAll<T>(ref T[] _arr) where T : class, IDisposable
        {
            for (int x = _arr.Length - 1; x >= 0; x--)
            {
                ref T item = ref _arr[x];
                item.Dispose();
                item = null;
            }
            _arr = null;
        }

        public static void DisposeAll<T>(ref List<T> _list) where T : class, IDisposable
        {
            for (int i = _list.Count; --i >= 0;)
                _list[i].Dispose();

            _list.Clear();
            _list = null;
        }
    }
}