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



.class public abstract auto ansi sealed beforefieldinit SuperComicLib.X64Math
       extends [mscorlib]System.Object
{
    .field public static initonly bool IsCompiled

    .method private hidebysig specialname rtspecialname static void  .cctor() cil managed
    {
        .maxstack  8

        call bool SuperComicLib.X64Math::Init()
        stsfld bool SuperComicLib.X64Math::IsCompiled
        ret
    }

    .method private hidebysig static bool  Init() cil managed
    {
        .maxstack 8

        // step 1.
        //  -- prepare methods --
        ldtoken method int64 SuperComicLib.X64Math::Mulhi(int64, int64)
        call void [mscorlib]System.Runtime.CompilerServices.RuntimeHelpers::PrepareMethod(valuetype [mscorlib]System.RuntimeMethodHandle)

        ldtoken method uint64 SuperComicLib.X64Math::Mulhi(uint64, uint64)
        call void [mscorlib]System.Runtime.CompilerServices.RuntimeHelpers::PrepareMethod(valuetype [mscorlib]System.RuntimeMethodHandle)

        ldtoken method int64 SuperComicLib.X64Math::BigMul(int64, int64, int64&)
        call void [mscorlib]System.Runtime.CompilerServices.RuntimeHelpers::PrepareMethod(valuetype [mscorlib]System.RuntimeMethodHandle)

        ldtoken method uint64 SuperComicLib.X64Math::BigMul(uint64, uint64, uint64&)
        call void [mscorlib]System.Runtime.CompilerServices.RuntimeHelpers::PrepareMethod(valuetype [mscorlib]System.RuntimeMethodHandle)


        // step 2.
        //  -- inject codes --
        ldtoken method int64 SuperComicLib.X64Math::Mulhi(int64, int64)
        ldsflda valuetype SuperComicLib.PrivateCore.X64ASM/__staticArraySize10 SuperComicLib.PrivateCore.X64ASM::__mulh
        ldflda uint8 SuperComicLib.PrivateCore.X64ASM/__staticArraySize10::element
        ldc.i4.s 10
        call void SuperComicLib.X64Math::patch(valuetype [mscorlib]System.RuntimeMethodHandle, uint8&, int32)

        ldtoken method uint64 SuperComicLib.X64Math::Mulhi(uint64, uint64)
        ldsflda valuetype SuperComicLib.PrivateCore.X64ASM/__staticArraySize10 SuperComicLib.PrivateCore.X64ASM::__umulh
        ldflda uint8 SuperComicLib.PrivateCore.X64ASM/__staticArraySize10::element
        ldc.i4.s 10
        call void SuperComicLib.X64Math::patch(valuetype [mscorlib]System.RuntimeMethodHandle, uint8&, int32)

        ldtoken method int64 SuperComicLib.X64Math::BigMul(int64, int64, int64&)
        ldsflda valuetype SuperComicLib.PrivateCore.X64ASM/__staticArraySize13 SuperComicLib.PrivateCore.X64ASM::_mul128
        ldflda uint8 SuperComicLib.PrivateCore.X64ASM/__staticArraySize10::element
        ldc.i4.s 13
        call void SuperComicLib.X64Math::patch(valuetype [mscorlib]System.RuntimeMethodHandle, uint8&, int32)

        ldtoken method uint64 SuperComicLib.X64Math::BigMul(uint64, uint64, uint64&)
        ldsflda valuetype SuperComicLib.PrivateCore.X64ASM/__staticArraySize13 SuperComicLib.PrivateCore.X64ASM::_umul128
        ldflda uint8 SuperComicLib.PrivateCore.X64ASM/__staticArraySize10::element
        ldc.i4.s 13
        call void SuperComicLib.X64Math::patch(valuetype [mscorlib]System.RuntimeMethodHandle, uint8&, int32)

        // step 3.
        //  -- return 'true' --
        ldc.i4.1
        ret
    }

    .method public hidebysig static int64  Mulhi(int64 a, int64 b) cil managed noinlining nooptimization
    {
        .maxstack 8

        ldsfld bool SuperComicLib.X64Math::IsCompiled
        brfalse.s L_001

        ldarg.0
        ldarg.1
        tail.
        call int64 SuperComicLib.X64Math::Mulhi(int64, int64)
        ret

    L_001:
        ldc.i4.0
        conv.i8
        ret
    }

    .method public hidebysig static uint64  Mulhi(uint64 a, uint64 b) cil managed noinlining nooptimization
    {
        .maxstack 8

        ldsfld bool SuperComicLib.X64Math::IsCompiled
        brfalse.s L_001

        ldarg.0
        ldarg.1
        tail.
        call uint64 SuperComicLib.X64Math::Mulhi(uint64, uint64)
        ret

    L_001:
        ldc.i4.0
        conv.u8
        ret
    }

    .method public hidebysig static int64  BigMul(int64 a, int64 b, [out] int64& low) cil managed noinlining nooptimization
    {
        .maxstack 8

        ldsfld bool SuperComicLib.X64Math::IsCompiled
        brfalse.s L_001

        ldarg.0
        ldarg.1
        ldarg.2
        tail.
        call int64 SuperComicLib.X64Math::BigMul(int64, int64, int64&)
        ret

    L_001:
        ldc.i4.0
        conv.i8
        ret
    }

    .method public hidebysig static uint64  BigMul(uint64 a, uint64 b, [out] uint64& low) cil managed noinlining nooptimization
    {
        .maxstack 8

        ldsfld bool SuperComicLib.X64Math::IsCompiled
        brfalse.s L_001

        ldarg.0
        ldarg.1
        ldarg.2
        tail.
        call uint64 SuperComicLib.X64Math::BigMul(uint64, uint64, uint64&)
        ret

    L_001:
        ldc.i4.0
        conv.u8
        ret
    }




    
    .method private hidebysig static void  patch(valuetype [mscorlib]System.RuntimeMethodHandle target, [in] uint8& asm, int32 size) cil managed noinlining
    {
        .param [2]
        .custom instance void [mscorlib]System.Runtime.CompilerServices.IsReadOnlyAttribute::.ctor() = ( 01 00 00 00 )

        .maxstack 8

        .locals (
            [0] uint8*  ,
            [1] int32   ,
            [2] valuetype [mscorlib]System.RuntimeMethodHandle
        )

        ldarg.0
        stloc.2
        ldloca.s 2
        call instance native int [mscorlib]System.RuntimeMethodHandle::GetFunctionPointer()
        call void* [mscorlib]System.IntPtr::op_Explicit(native int)
        stloc.0
        ldc.i4.0
        stloc.1

        // loop
        br.s L001

        L002:
            ldloc.0
            ldloc.1
            add
            ldarg.1
            ldloc.1
            add
            ldind.u1
            stind.i1

            // ++i
            ldloc.1
            ldc.i4.1
            add
            stloc.1

        L001:
            ldloc.1
            ldarg.2
            blt.s L002

        // end loop
        ret
    }
}