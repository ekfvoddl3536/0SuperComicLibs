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
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SuperComicLib.eTxt
{
    [StructLayout(LayoutKind.Sequential)]
    public readonly ref struct HierarchyName
    {
        public readonly string Value;
        internal readonly string _source;
        internal readonly Range _range;

        internal HierarchyName(string source, Range range)
        {
            Value = source.Substring(range.start, range.Length);
            _source = source;
            _range = range;
        }

        internal HierarchyName(string source)
        {
            Value = source;
            _source = source;
            _range = new Range(0, source.Length);
        }

        /// <summary>
        /// Move next or previous hierarchy name.
        /// </summary>
        /// <param name="index">negative = backward(previous)<para/>positive = forward(next)</param>
        /// <returns>When the end is reached, a struct with null value is returned.</returns>
        public HierarchyName this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if (index == 0)
                    return this;

                HierarchyName result;
                if (index > 0)
                    do
                        result = Next();
                    while (result.Value != null && --index != 0);
                else
                    do
                        result = Prev();
                    while (result.Value != null && ++index != 0);

                return result;
            }
        }

        /// <summary>
        /// Move next hierarchy name.
        /// </summary>
        /// <returns>When the end is reached, a struct with null value is returned.</returns>
        public HierarchyName Next()
        {
            var source = _source;
            var index = _range.end + 1;

            if (source.Length <= index)
                return default;

            var nidx = source.IndexOf('.', index);
            return 
                nidx >= 0 
                ? new HierarchyName(source, new Range(index, nidx)) 
                : new HierarchyName(source, new Range(index, source.Length));
        }

        /// <summary>
        /// Move previous hierarchy name.
        /// </summary>
        /// <returns>When the end is reached, a struct with null value is returned.</returns>
        public HierarchyName Prev()
        {
            var source = _source;
            var index = _range.start - 2;

            if (index <= 0)
                return default;

            var pidx = source.LastIndexOf('.', index) + 1;
            return
                pidx > 0
                ? new HierarchyName(source, new Range(pidx, index))
                : new HierarchyName(source, new Range(0, index));
        }
    }
}
