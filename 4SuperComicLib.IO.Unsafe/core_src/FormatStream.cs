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

using System.IO;
using System.Text;
using SuperComicLib.CodeContracts;

namespace SuperComicLib.IO
{
    public static unsafe class FormatStream
    {
        [return: NotNull]
        public static FormatStreamReader<TResult, TLocalBuf> UnsafeFastNew<TResult, TLocalBuf>(
            [DisallowNull] IFormatParseResolver<TResult, TLocalBuf> resolver,
            [DisallowNull] Stream stream,
            [AllowNull] Encoding encoding = null,
            int bufferSize = 0)
            where TResult : unmanaged
            where TLocalBuf : unmanaged
        {
            FastContract.Requires(stream != null);
            FastContract.Requires(resolver != null);
            FastContract.Requires(bufferSize > 16);

            if (encoding == null)
                encoding = Encoding.Default;

            bufferSize = (int)CMath.Max((uint)resolver.EncodingBufferSize, 128u);

            return
                new FormatStreamReader<TResult, TLocalBuf>(
                    stream,
                    encoding,
                    resolver,
                    bufferSize);
        }
    }
}
