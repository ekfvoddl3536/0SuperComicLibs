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

namespace SuperComicLib.IO.AdvancedParallel
{
    internal sealed unsafe class ParallelFileReadWorker
    {
        private readonly IParallelFileReader _owner;
        private readonly ParallelReadHandler _body;
        private readonly Range64 _offset;
        private readonly int _bufferSize;
        private readonly int _workerIndex;

        public ParallelFileReadWorker(
            IParallelFileReader owner,
            ParallelReadHandler body,
            in Range64 offset,
            int bufferSize,
            int workerIndex)
        {
            _owner = owner;
            _body = body;
            _offset = offset;
            _bufferSize = bufferSize;
            _workerIndex = workerIndex;
        }

        public void Run()
        {
            var ops = new OffsetParallelStream(_owner, _offset, _bufferSize);

            _body.Invoke(ref ops, (uint)_workerIndex);
        }
    }
}
