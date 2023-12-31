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



.class public abstract auto ansi sealed beforefieldinit SuperComicLib.X64ABI
       extends [mscorlib]System.Object
{
    .class nested public auto ansi sealed CALLCONV
           extends [mscorlib]System.Enum
    {
        .field public specialname rtspecialname uint32 value__
    
        .field public static literal valuetype SuperComicLib.X64ABI/CALLCONV INVALID   = uint32(0)
        .field public static literal valuetype SuperComicLib.X64ABI/CALLCONV MS_ABI    = uint32(1)
        .field public static literal valuetype SuperComicLib.X64ABI/CALLCONV SYS_V_ABI = uint32(2)
    }
    
    .field public static initonly valuetype SuperComicLib.X64ABI/CALLCONV CallingConvention
    
    .method private hidebysig specialname rtspecialname static void  .cctor () cil managed 
    {
        .maxstack 8
    
        call valuetype SuperComicLib.X64ABI/CALLCONV SuperComicLib.X64ABI::_GetCallConv()
        stsfld valuetype SuperComicLib.X64ABI/CALLCONV SuperComicLib.X64ABI::CallingConvention
        ret
    }

    .method private hidebysig static valuetype SuperComicLib.X64ABI/CALLCONV  _GetCallConv() cil managed
    {
        .maxstack 8

        .locals (
            [0] method int32 *(int64, int64, int64, int64, int64, int64)
        )

        ldtoken method int32 SuperComicLib.X64ABI::__abi_test(int64, int64, int64, int64, int64, int64)
        call void [mscorlib]System.Runtime.CompilerServices.RuntimeHelpers::PrepareMethod(valuetype [mscorlib]System.RuntimeMethodHandle)
        
        ldtoken method int32 SuperComicLib.X64ABI::__abi_test(int64, int64, int64, int64, int64, int64)
        ldsflda valuetype SuperComicLib.PrivateCore.X64ASM/__staticArraySize74 SuperComicLib.PrivateCore.X64ASM::_testabi
        ldflda uint8 SuperComicLib.PrivateCore.X64ASM/__staticArraySize74::element
        ldc.i4.s 74
        call void SuperComicLib.PrivateCore.X64ASM::patch(valuetype [mscorlib]System.RuntimeMethodHandle, uint8&, int32)

        // Don't tail call (DIRTY 'rcx, rdx')
        ldftn int32 SuperComicLib.X64ABI::__abi_test(int64, int64, int64, int64, int64, int64)
        stloc.0
        ldc.i4.1
        conv.i8
        ldc.i4.2
        conv.i8
        ldc.i4.3
        conv.i8
        ldc.i4.4
        conv.i8
        ldc.i4.5
        conv.i8
        ldc.i4.6
        conv.i8
        // call rax (.NET)
        // call r11 (MONO)
        ldloc.0
        calli int32(int64, int64, int64, int64, int64, int64)

        ret
    }

    .method private hidebysig static int32  __abi_test(
            int64 t0, 
            int64 t1,
            int64 t2,
            int64 t3,
            int64 t4,
            int64 t5) cil managed noinlining nooptimization
    {
        .maxstack 8

        .locals init (
            [0] int32
        )

        ldarg.0
        ldarg.1
        mul.ovf

        ldarg.2
        mul.ovf

        ldarg.3
        mul.ovf

        ldarg.s 4
        mul.ovf

        conv.i4
        stloc.0

        ldloc.0
        ret
    }
}