// MIT License
//
// Copyright (c) 2020-2023. SuperComic (ekfvoddl3535@naver.com)
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
using System.Runtime.CompilerServices;
using SuperComicLib.RuntimeMemoryMarshals;

namespace SuperComicLib.IO
{
    public static unsafe class GLOBAL_SuperComicLib_IO_StreamExtension_MemoryExtension
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Read(this Stream stream, in arrayref<byte> buffer) => 
            stream.Read(buffer.AsManaged(), 0, buffer.Length);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Read(this Stream stream, in arrayrefSegment<byte> bufferSegment) => 
            stream.Read(bufferSegment._source.AsManaged(), bufferSegment._start, bufferSegment.Length);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Write(this Stream stream, in arrayref<byte> buffer) =>
            stream.Write(buffer.AsManaged(), 0, buffer.Length);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Write(this Stream stream, in arrayrefSegment<byte> bufferSegment) =>
            stream.Write(bufferSegment._source.AsManaged(), bufferSegment._start, bufferSegment.Length);
    }
}
