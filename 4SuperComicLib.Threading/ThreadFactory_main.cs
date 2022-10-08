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
using System.Collections;

namespace SuperComicLib.Threading
{
    public static partial class ThreadFactory
    {
        public static IEnumerator Create(Action<IWorker> taskMethod) => new E_Single(taskMethod);

        public static IEnumerator Create(int threadCount, params Action<int, IMultiWorker>[] taskMethods) =>
            taskMethods == null
            ? throw new ArgumentNullException(nameof(taskMethods))
            : taskMethods.Length < 2
            ? throw new ArgumentOutOfRangeException(nameof(taskMethods))
            : threadCount < 2 || threadCount >= Environment.ProcessorCount
            ? throw new ArgumentOutOfRangeException($"out of range: {nameof(threadCount)}. min: 2, max: {Environment.ProcessorCount - 1}")
            : new E_Multi(taskMethods, threadCount);
    }
}
