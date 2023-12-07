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

internal static class SR
{
    public const string TENSOR_NONSEQUENTIAL = "Non-sequential tensor. Use `trimExcess` to arrange elements contiguously in memory.";

    public const string TENSOR_OVERFLOW = "Overflow. The number of elements in a dimension exceeds the uint32 range.";

    public const string MATRIX2D_OVERFLOW = "Overflow. '(width * height) > int.MaxValue'";

    public const string MISMATCH_SHAPE = "The shape of the tensor is different, thus the operation cannot be performed.";

    public const string OUTOFRANGE_SLICES = "Selects more dimensions than the target shape.";
}