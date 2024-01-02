// The MIT License (MIT)
//
// Copyright (c) 2023-2024. SuperComic (ekfvoddl3535@naver.com)
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
    /// Provides native runtime functionality targeting x64 machines.
    /// </summary>
    /// <remarks>
    /// This type is designed for professional memory and machine language manipulation scenarios.<br/>
    /// It is presumed that the caller has a thorough understanding of the .NET runtime, .NET runtime implementations, 
    /// and JIT compiler, and is well-versed in x86_64 machine language programming.
    /// <para/>
    /// This type can unpredictably alter the behavior of the runtime and requires a comprehensive understanding of 
    /// the typical runtime environments for its use.<br/>
    /// Improper utilization of these APIs can lead to corruption of process memory or destabilization of the.NET runtime.
    /// </remarks>
    public static class NativeRuntimeSupports
    {
        /// <summary>
        /// Prepares the method and modifies the code of the compiled method with the provided raw image code.
        /// </summary>
        /// <remarks>
        /// Make sure the compiled method's code is sufficient to accommodate all of the provided raw image code.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CompileMethod(RuntimeMethodHandle handle, in byte lpNativeImageCode, int dwNativeImageCodeSize) =>
            throw new PlatformNotSupportedException();

        /// <summary>
        /// Assuming the provided method is ready and compiled, modify the method's code with the provided native image code.
        /// <para/>
        /// This method is a simplified variant of <see cref="CompileMethod(RuntimeMethodHandle, in byte, int)"/>.
        /// </summary>
        /// <remarks>
        /// Make sure the compiled method's code is sufficient to accommodate all of the provided raw image code.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Inject(RuntimeMethodHandle handle, in byte lpNativeImageCode, int dwNativeImageCodeSize) =>
            throw new PlatformNotSupportedException();

        /// <summary>
        /// Gets the starting address of the prolog of the provided method procedure.
        /// </summary>
        /// <remarks>
        /// This method does not prepare the provided method, so prepare the method before performing this method.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IntPtr GetActualFunctionAddress(RuntimeMethodHandle handle) =>
            throw new PlatformNotSupportedException();

        /// <summary>
        /// Removes memory protection for the specified address and marks it as READ/WRITE/EXECUTE.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void MemUnprotect(IntPtr ptr) => 
            throw new PlatformNotSupportedException();
    }
}
