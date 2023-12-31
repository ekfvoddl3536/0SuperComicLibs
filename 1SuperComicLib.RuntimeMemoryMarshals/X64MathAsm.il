// The MIT License (MIT)
//
// Copyright (c) 2023. SuperComic (ekfvoddl3535@naver.com)
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



.class private auto ansi sealed SuperComicLib.PrivateCore.X64ASM
       extends [mscorlib]System.Object
{
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 )

    // Nested Types
    //  Byte[10]
    .class nested assembly explicit ansi sealed __staticArraySize10
            extends [mscorlib]System.ValueType
    {
        .pack 1
        .size 10

        .field [0] public initonly uint8 element
    }
    //  Byte[13]
    .class nested assembly explicit ansi sealed __staticArraySize13
            extends [mscorlib]System.ValueType
    {
        .pack 1
        .size 13

        .field [0] public initonly uint8 element
    }

    // Fields
    .field assembly static initonly valuetype SuperComicLib.PrivateCore.X64ASM/__staticArraySize10 __mulh at G_BYTEARRAY_0001
    .field assembly static initonly valuetype SuperComicLib.PrivateCore.X64ASM/__staticArraySize10 __umulh at G_BYTEARRAY_0002

    .field assembly static initonly valuetype SuperComicLib.PrivateCore.X64ASM/__staticArraySize13 _mul128 at G_BYTEARRAY_0003
    .field assembly static initonly valuetype SuperComicLib.PrivateCore.X64ASM/__staticArraySize13 _umul128 at G_BYTEARRAY_0004

    .data cil G_BYTEARRAY_0001 = bytearray (
        48 89 c8    //  mov rax, rcx
        48 f7 ea    //  imul rdx
        48 89 d0    //  mov rax, rdx
        c3          //  ret
    )
    .data cil G_BYTEARRAY_0002 = bytearray (
        48 89 c8    //  mov rax, rcx
        48 f7 e2    //  mul rdx
        48 89 d0    //  mov rax, rdx
        c3          //  ret
    )

    .data cil G_BYTEARRAY_0003 = bytearray (
        48 89 c8    //  mov rax, rcx
        48 f7 ea    //  imul rdx
        49 89 00    //  mov [r8], rax
        48 89 d0    //  mov rax, rdx
        c3          //  ret
    )
    .data cil G_BYTEARRAY_0004 = bytearray (
        48 89 c8    //  mov rax, rcx
        48 f7 e2    //  mul rdx
        49 89 00    //  mov [r8], rax
        48 89 d0    //  mov rax, rdx
        c3          //  ret
    )
}