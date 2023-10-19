// MIT License
//
// Copyright (c) 2019-2023. SuperComic (ekfvoddl3535@naver.com)
// Copyright (c) .NET Foundation and Contributors
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
using System.Runtime.CompilerServices;

namespace SuperComicLib.RuntimeMemoryMarshals
{
    /// <summary>
    /// Helper class to determine if the currently running environment is a 'MonoRuntime'
    /// </summary>
    /// ref.: https://stackoverflow.com/questions/721161/how-to-detect-which-net-runtime-is-being-used-ms-vs-mono
    public static unsafe class JITPlatformEnvironment
    {
        private static int _isRunningOnMono;

        /// <summary>
        /// Gets whether the running on MonoRuntime.
        /// <br/>
        /// The result of this method is cached and returns the same result value until the cache is forcibly cleared.
        /// </summary>
        public static bool IsRunningOnMono
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if (_isRunningOnMono == 0)
                    _isRunningOnMono = ILUnsafe.ConvI4(Type.GetType("Mono.Runtime") != null) + 1;

                return _isRunningOnMono > 1;
            }
        }

        /// <summary>
        /// Discard all cached data.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DiscardCache() => _isRunningOnMono = 0;
    }
}
