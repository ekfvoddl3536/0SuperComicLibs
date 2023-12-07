// MIT License
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

using System.Collections;

namespace SuperComicLib.Tensor;

[StructLayout(LayoutKind.Sequential, Pack = 0x10)]
public readonly unsafe struct PointerEnumerable<T> : IEnumerable<Ptr<T>>
    where T : unmanaged
{
    private readonly T* _begin, _end;

    public PointerEnumerable(T* begin, T* end)
    {
        _begin = begin;
        _end = end;
    }

    public IEnumerator<Ptr<T>> GetEnumerator() => new Enumerator(_begin, _end);
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    [StructLayout(LayoutKind.Sequential, Pack = 0x10)]
    private struct Enumerator : IEnumerator<Ptr<T>>
    {
        private T* _last;
        private readonly T* _end;
        private readonly T* _first;

        public Enumerator(T* begin, T* end)
        {
            _last = begin - 1;
            _end = end;
            _first = begin;
        }

        public readonly Ptr<T> Current => _last;
        readonly object IEnumerator.Current => Current;

        public bool MoveNext() => ++_last != _end;
        public void Reset() => _last = _first - 1;

        public readonly void Dispose() { }
    }
}
