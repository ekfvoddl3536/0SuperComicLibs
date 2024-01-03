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
        
        // CHECK ABI
        ldsfld valuetype SuperComicLib.X64ABI/CALLCONV SuperComicLib.X64ABI::CallingConvention
        ldc.i4.1
        bne.un.s M_SYSVABI
        
        //  -- MS ABI --
        ldftn int64 SuperComicLib.X64Math::Mulhi(int64, int64)
        ldsflda valuetype SuperComicLib.PrivateCore._NativeImg/__staticArraySize10 SuperComicLib.PrivateCore._NativeImg::__mulh
        ldflda uint8 SuperComicLib.PrivateCore._NativeImg/__staticArraySize10::element
        ldc.i4.s 10
        call void SuperComicLib.PrivateCore._NativeImg::patch(native int, uint8&, int32)
        
        ldftn uint64 SuperComicLib.X64Math::Mulhi(uint64, uint64)
        ldsflda valuetype SuperComicLib.PrivateCore._NativeImg/__staticArraySize10 SuperComicLib.PrivateCore._NativeImg::__umulh
        ldflda uint8 SuperComicLib.PrivateCore._NativeImg/__staticArraySize10::element
        ldc.i4.s 10
        call void SuperComicLib.PrivateCore._NativeImg::patch(native int, uint8&, int32)
        
        ldftn int64 SuperComicLib.X64Math::BigMul(int64, int64, int64&)
        ldsflda valuetype SuperComicLib.PrivateCore._NativeImg/__staticArraySize13 SuperComicLib.PrivateCore._NativeImg::_mul128
        ldflda uint8 SuperComicLib.PrivateCore._NativeImg/__staticArraySize13::element
        ldc.i4.s 13
        call void SuperComicLib.PrivateCore._NativeImg::patch(native int, uint8&, int32)
        
        ldftn uint64 SuperComicLib.X64Math::BigMul(uint64, uint64, uint64&)
        ldsflda valuetype SuperComicLib.PrivateCore._NativeImg/__staticArraySize13 SuperComicLib.PrivateCore._NativeImg::_umul128
        ldflda uint8 SuperComicLib.PrivateCore._NativeImg/__staticArraySize13::element
        ldc.i4.s 13
        call void SuperComicLib.PrivateCore._NativeImg::patch(native int, uint8&, int32)
        
        br.s G_RETURN
        
    M_SYSVABI:
        ldsfld valuetype SuperComicLib.X64ABI/CALLCONV SuperComicLib.X64ABI::CallingConvention
        ldc.i4.2
        bne.un.s M_INVALID
        
        //  -- SYS V ABI --
        ldftn int64 SuperComicLib.X64Math::Mulhi(int64, int64)
        ldsflda valuetype SuperComicLib.PrivateCore._NativeImg/__staticArraySize10 SuperComicLib.PrivateCore._NativeImg::__mulhV
        ldflda uint8 SuperComicLib.PrivateCore._NativeImg/__staticArraySize10::element
        ldc.i4.s 10
        call void SuperComicLib.PrivateCore._NativeImg::patch(native int, uint8&, int32)
        
        ldftn uint64 SuperComicLib.X64Math::Mulhi(uint64, uint64)
        ldsflda valuetype SuperComicLib.PrivateCore._NativeImg/__staticArraySize10 SuperComicLib.PrivateCore._NativeImg::__umulhV
        ldflda uint8 SuperComicLib.PrivateCore._NativeImg/__staticArraySize10::element
        ldc.i4.s 10
        call void SuperComicLib.PrivateCore._NativeImg::patch(native int, uint8&, int32)
        
        ldftn int64 SuperComicLib.X64Math::BigMul(int64, int64, int64&)
        ldsflda valuetype SuperComicLib.PrivateCore._NativeImg/__staticArraySize16 SuperComicLib.PrivateCore._NativeImg::_mul128V
        ldflda uint8 SuperComicLib.PrivateCore._NativeImg/__staticArraySize16::element
        ldc.i4.s 16
        call void SuperComicLib.PrivateCore._NativeImg::patch(native int, uint8&, int32)
        
        ldftn uint64 SuperComicLib.X64Math::BigMul(uint64, uint64, uint64&)
        ldsflda valuetype SuperComicLib.PrivateCore._NativeImg/__staticArraySize16 SuperComicLib.PrivateCore._NativeImg::_umul128V
        ldflda uint8 SuperComicLib.PrivateCore._NativeImg/__staticArraySize16::element
        ldc.i4.s 16
        call void SuperComicLib.PrivateCore._NativeImg::patch(native int, uint8&, int32)
        
    G_RETURN:
        // step 3.
        //  -- return 'true' --
        ldc.i4.1
        ret
        
    M_INVALID:
        //  -- FAIL --
        ldc.i4.0
        ret
    }
    
    .method public hidebysig static int64  Mulhi(int64 a, int64 b) cil managed preservesig noinlining nooptimization
    {
        .maxstack 8
        
        ldsfld bool SuperComicLib.X64Math::IsCompiled
        brfalse.s L_001
        
        ldarg.0
        ldarg.1
        mul
        ret
        
    L_001:
        ldc.i4.0
        conv.i8
        ret
    }
    
    .method public hidebysig static uint64  Mulhi(uint64 a, uint64 b) cil managed preservesig noinlining nooptimization
    {
        .maxstack 8
        
        ldsfld bool SuperComicLib.X64Math::IsCompiled
        brfalse.s L_001
        
        ldarg.0
        ldarg.1
        mul
        ret
        
    L_001:
        ldc.i4.0
        conv.u8
        ret
    }
    
    .method public hidebysig static int64  BigMul(int64 a, int64 b, [out] int64& low) cil managed preservesig noinlining nooptimization
    {
        .maxstack 8
        
        ldsfld bool SuperComicLib.X64Math::IsCompiled
        brfalse.s L_001
        
        ldarg.0
        ldarg.1
        mul.ovf
        ret
        
    L_001:
        ldc.i4.0
        conv.i8
        ret
    }
    
    .method public hidebysig static uint64  BigMul(uint64 a, uint64 b, [out] uint64& low) cil managed preservesig noinlining nooptimization
    {
        .maxstack 8
        
        ldsfld bool SuperComicLib.X64Math::IsCompiled
        brfalse.s L_001
        
        ldarg.0
        ldarg.1
        mul.ovf
        ret
        
    L_001:
        ldc.i4.0
        conv.u8
        ret
    }
}