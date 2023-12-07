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

namespace SuperComicLib.Tensor;

internal static unsafe class SliceHelper
{
    public static ReadOnlySpan<Slice> _internal_build(ReadOnlySpan<Slice> slices, int ellipsis, ReadOnlySpan<uint> shape)
    {
        // [CHECKED]   ellipsis >= 0
        // [CHECKED]   slices.Length > 0
        // [POSSIBLE]  shape.Length == 0
        
        int padding = shape.Length - slices.Length + 1;
        if (padding <= 0)
            // don't return empty array. 'shape.Length == 0', possible.
            return slices;

        if (slices.Slice(ellipsis + 1).Contains(Slice.Ellipsis))
            // two (or, more) ellipsis find!
            return Array.Empty<Slice>();

        var array = new Slice[shape.Length];

        var dst = array.AsSpan();

        slices.Slice(0, ellipsis).CopyTo(dst);

        dst.Slice(ellipsis, padding).Fill(Slice.All);

        slices.Slice(ellipsis + 1).CopyTo(dst.Slice(ellipsis + padding));

        return dst;
    }
}
