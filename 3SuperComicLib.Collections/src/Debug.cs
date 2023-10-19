﻿// MIT License
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

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SuperComicLib.Collections
{
    internal sealed class EnumerableView<T>
    {
        private readonly T[] arr;

        public EnumerableView(IEnumerable<T> inst) => arr = inst.ToArray();


        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public T[] Items => arr;
    }

    internal sealed unsafe class RawContainerView<T> where T : unmanaged
    {
        private readonly T[] _items;

        public RawContainerView(IReadOnlyRawContainer<T> inst) => _items = new NativeConstSpan<T>(inst.cbegin(), inst.cend()).ToArray();

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public T[] Items => _items;
    }
}