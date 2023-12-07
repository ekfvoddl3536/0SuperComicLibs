﻿// MIT License
//
// Copyright (c) 2023. SuperComic (ekfvoddl3535@naver.com)
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

using System.Diagnostics;

namespace SuperComicLib.Tensor.Debugging;

internal sealed unsafe class InternalVectorDebugView<T> where T : unmanaged
{
    private readonly T[] _array;

    public InternalVectorDebugView(ReadOnlyVector1D<T> vector) : this(vector.v)
    { 
    }

    public InternalVectorDebugView(Vector1D<T> vector)
    {
        if (vector.ptr == null || vector.length == 0)
            _array = [];
        else
        {
            var len = Math.Min(vector.length, int.MaxValue);

            var arr = new T[len];
            _array = arr;

            new Span<T>(vector.ptr, (int)len).CopyTo(arr.AsSpan());
        }
    }

    [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
    public T[] Items => _array;
}
