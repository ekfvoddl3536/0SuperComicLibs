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

.class public abstract auto ansi sealed beforefieldinit SuperComicLib.RuntimeMemoryMarshals.X86Function
        extends [mscorlib]System.Object
{
    .class nested public sequential ansi sealed REG
            extends [mscorlib]System.ValueType
    {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.IsReadOnlyAttribute::.ctor() = ( 01 00 00 00 )
        
        // Fields
        .field public initonly int64 rax
        .field public initonly int64 rbx
        .field public initonly int64 rcx
        .field public initonly int64 rdx
    }
    
    .field public static initonly bool IsCompiled
    
    .method private hidebysig specialname rtspecialname static void  .cctor() cil managed
    {
        .maxstack  8
        
        call bool SuperComicLib.RuntimeMemoryMarshals.X86Function::Init()
        stsfld bool SuperComicLib.RuntimeMemoryMarshals.X86Function::IsCompiled
        ret
    }
    
    .method private hidebysig static bool  Init() cil managed
    {
        .maxstack 8
        
        ldtoken method void SuperComicLib.RuntimeMemoryMarshals.X86Function::impl__cpuid(int32, void*)
        call void [mscorlib]System.Runtime.CompilerServices.RuntimeHelpers::PrepareMethod(valuetype [mscorlib]System.RuntimeMethodHandle)
        
        ldtoken method void SuperComicLib.RuntimeMemoryMarshals.X86Function::impl__cpuidEx(int32, int32, void*)
        call void [mscorlib]System.Runtime.CompilerServices.RuntimeHelpers::PrepareMethod(valuetype [mscorlib]System.RuntimeMethodHandle)
        
        ldtoken method native int SuperComicLib.RuntimeMemoryMarshals.X86Function::impl__stackptr()
        call void [mscorlib]System.Runtime.CompilerServices.RuntimeHelpers::PrepareMethod(valuetype [mscorlib]System.RuntimeMethodHandle)
        
        //  no abi dependency
        ldtoken method native int SuperComicLib.RuntimeMemoryMarshals.X86Function::impl__stackptr()
        ldsflda valuetype SuperComicLib.PrivateCore._NativeImg/__staticArraySize6 SuperComicLib.PrivateCore._NativeImg::_sp
        ldflda uint8 SuperComicLib.PrivateCore._NativeImg/__staticArraySize6::element
        ldc.i4.s 6
        call void SuperComicLib.PrivateCore._NativeImg::patch(valuetype [mscorlib]System.RuntimeMethodHandle, uint8&, int32)
        
        // CHECK ABI
        ldsfld valuetype SuperComicLib.X64ABI/CALLCONV SuperComicLib.X64ABI::CallingConvention
        ldc.i4.1
        bne.un.s M_SYSVABI
        
        //  -- MS ABI --
        ldtoken method void SuperComicLib.RuntimeMemoryMarshals.X86Function::impl__cpuid(int32, void*)
        ldsflda valuetype SuperComicLib.PrivateCore._NativeImg/__staticArraySize25 SuperComicLib.PrivateCore._NativeImg::_cpuid
        ldflda uint8 SuperComicLib.PrivateCore._NativeImg/__staticArraySize25::element
        ldc.i4.s 25
        call void SuperComicLib.PrivateCore._NativeImg::patch(valuetype [mscorlib]System.RuntimeMethodHandle, uint8&, int32)
        
        ldtoken method void SuperComicLib.RuntimeMemoryMarshals.X86Function::impl__cpuidEx(int32, int32, void*)
        ldsflda valuetype SuperComicLib.PrivateCore._NativeImg/__staticArraySize27 SuperComicLib.PrivateCore._NativeImg::_cpuidEx
        ldflda uint8 SuperComicLib.PrivateCore._NativeImg/__staticArraySize27::element
        ldc.i4.s 27
        call void SuperComicLib.PrivateCore._NativeImg::patch(valuetype [mscorlib]System.RuntimeMethodHandle, uint8&, int32)
        
        br.s G_RETURN
        
    M_SYSVABI:
        ldsfld valuetype SuperComicLib.X64ABI/CALLCONV SuperComicLib.X64ABI::CallingConvention
        ldc.i4.2
        bne.un.s M_INVALID
        
        //  -- SYS V ABI --
        ldtoken method void SuperComicLib.RuntimeMemoryMarshals.X86Function::impl__cpuid(int32, void*)
        ldsflda valuetype SuperComicLib.PrivateCore._NativeImg/__staticArraySize25 SuperComicLib.PrivateCore._NativeImg::_cpuidV
        ldflda uint8 SuperComicLib.PrivateCore._NativeImg/__staticArraySize25::element
        ldc.i4.s 25
        call void SuperComicLib.PrivateCore._NativeImg::patch(valuetype [mscorlib]System.RuntimeMethodHandle, uint8&, int32)
        
        ldtoken method void SuperComicLib.RuntimeMemoryMarshals.X86Function::impl__cpuidEx(int32, int32, void*)
        ldsflda valuetype SuperComicLib.PrivateCore._NativeImg/__staticArraySize27 SuperComicLib.PrivateCore._NativeImg::_cpuidVEx
        ldflda uint8 SuperComicLib.PrivateCore._NativeImg/__staticArraySize27::element
        ldc.i4.s 27
        call void SuperComicLib.PrivateCore._NativeImg::patch(valuetype [mscorlib]System.RuntimeMethodHandle, uint8&, int32)
        
    G_RETURN:
        //  -- DONE! --
        ldc.i4.1
        ret
        
    M_INVALID:
        //  -- FAIL --
        ldc.i4.0
        ret
    }
    
    
    .method public hidebysig static valuetype SuperComicLib.RuntimeMemoryMarshals.X86Function/REG  _CPUID(int32 eax) cil managed
    {
        .maxstack 8
        
        .locals (
            [0] valuetype SuperComicLib.RuntimeMemoryMarshals.X86Function/REG
        )
        
        ldsfld bool SuperComicLib.RuntimeMemoryMarshals.X86Function::IsCompiled
        brfalse.s L_001
        
        ldarg.0
        ldloca.s 0
        ldftn void SuperComicLib.RuntimeMemoryMarshals.X86Function::impl__cpuid(int32, void*) 
        calli void(int32, void*)
        
        br.s G_RETURN
        
    L_001:
        ldloca.s 0
        initobj SuperComicLib.RuntimeMemoryMarshals.X86Function/REG
        
    G_RETURN:
        ldloc.0
        ret
    }
    
    .method public hidebysig static valuetype SuperComicLib.RuntimeMemoryMarshals.X86Function/REG  _CPUID(int32 eax, int32 ecx) cil managed
    {
        .maxstack 8
        
        .locals (
            [0] valuetype SuperComicLib.RuntimeMemoryMarshals.X86Function/REG
        )
        
        ldsfld bool SuperComicLib.RuntimeMemoryMarshals.X86Function::IsCompiled
        brfalse.s L_001
        
        ldarg.0
        ldarg.1
        ldloca.s 0
        ldftn void SuperComicLib.RuntimeMemoryMarshals.X86Function::impl__cpuidEx(int32, int32, void*)
        calli void(int32, int32, void*)
        
        br.s G_RETURN
        
    L_001:
        ldloca.s 0
        initobj SuperComicLib.RuntimeMemoryMarshals.X86Function/REG
        
    G_RETURN:
        ldloc.0
        ret
    }
    
    .method public hidebysig static native int  _STACKPOINTER() cil managed aggressiveinlining
    {
        .maxstack 8
        
        ldsfld bool SuperComicLib.RuntimeMemoryMarshals.X86Function::IsCompiled
        brfalse.s L_001
        
        ldftn native int SuperComicLib.RuntimeMemoryMarshals.X86Function::impl__stackptr()
        tail.
        calli native int()
        ret
        
    L_001:
        ldc.i4.0
        conv.i
        ret
    }
    

    
    .method private static void  impl__cpuid(int32 a, void* p) cil managed noinlining nooptimization preservesig
    {
        .maxstack 8
        
        .try
        {
            ldarg.0
            conv.u8
            ldc.i8 100000000
            conv.u8
            mul.ovf
            pop
            
            ldarg.1
            ldc.i4.1
            conv.i8
            stind.i8
            
            leave.s G_RETURN
        }
        catch [mscorlib]System.Object
        {
            pop
            leave.s G_RETURN
        }
        
    G_RETURN:
        ret
    }
    
    .method private static void  impl__cpuidEx(int32 a, int32 b, void* p) cil managed noinlining nooptimization preservesig
    {
        .maxstack 8
        
        .try
        {
            ldarg.0
            conv.u8
            ldarg.1
            conv.u8
            mul.ovf
            pop
            
            ldarg.2
            ldc.i4.1
            conv.i8
            stind.i8
            
            leave.s G_RETURN
        }
        catch [mscorlib]System.Object
        {
            pop
            leave.s G_RETURN
        }
        
    G_RETURN:
        ret
    }
    
    .method private static native int  impl__stackptr() cil managed noinlining nooptimization preservesig
    {
        .maxstack 8
        
        call int32 [mscorlib]System.Environment::get_TickCount()
        conv.i
        ret
    }
}