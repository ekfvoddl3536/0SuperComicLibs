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



.class public abstract auto ansi sealed beforefieldinit SuperComicLib.Runtime.NativeRuntimeSupports
       extends [mscorlib]System.Object
{
    // Methods
    .method public hidebysig static void  CompileMethod(valuetype [mscorlib]System.RuntimeMethodHandle handle, [in] uint8& lpNativeImageCode, int32 dwNativeImageCodeSize) cil managed aggressiveinlining
    {
        .param [2]
		.custom instance void [mscorlib]System.Runtime.CompilerServices.IsReadOnlyAttribute::.ctor() = ( 01 00 00 00 )
		
        .maxstack 3
        
        ldarg.0
        call void [mscorlib]System.Runtime.CompilerServices.RuntimeHelpers::PrepareMethod(valuetype [mscorlib]System.RuntimeMethodHandle)
        
        ldarg.0
        ldarg.1
        ldarg.2
        tail.
        call void SuperComicLib.PrivateCore._UnsafeMethods::patch(valuetype [mscorlib]System.RuntimeMethodHandle, uint8&, int32)
        
        ret
    }
    
    .method public hidebysig static void  Inject(valuetype [mscorlib]System.RuntimeMethodHandle handle, [in] uint8& lpNativeImageCode, int32 dwNativeImageCodeSize) cil managed aggressiveinlining
    {
        .param [2]
		.custom instance void [mscorlib]System.Runtime.CompilerServices.IsReadOnlyAttribute::.ctor() = ( 01 00 00 00 )
        
        .maxstack 3
        
        ldarg.0
        ldarg.1
        ldarg.2
        tail.
        call void SuperComicLib.PrivateCore._UnsafeMethods::patch(valuetype [mscorlib]System.RuntimeMethodHandle, uint8&, int32)
        
        ret
    }
    
    .method public hidebysig static native int  GetActualFunctionAddress(valuetype [mscorlib]System.RuntimeMethodHandle handle) cil managed aggressiveinlining
    {
        .maxstack 3
        
        ldarga.s 0
        call instance native int [mscorlib]System.RuntimeMethodHandle::GetFunctionPointer()
        ldc.i4.0
        tail.
        call void* SuperComicLib.PrivateCore._UnsafeMethods::prologue(native int, int32)
        
        ret
    }
    
    .method public hidebysig static void*  GetActualFunctionAddress(void* in_functionPointer) cil managed aggressiveinlining
    {
        .maxstack 3
        
        ldarg.0
        ldc.i4.0
        tail.
        call void* SuperComicLib.PrivateCore._UnsafeMethods::prologue(native int, int32)
        
        ret
    }
    
    .method public hidebysig static bool  MemUnprotect(native int ptr) cil managed aggressiveinlining
    {
        .maxstack 3
        
        ldarg.0
        call void* [mscorlib]System.IntPtr::op_Explicit(native int)
        ldc.i4.1
        tail.
        call bool SuperComicLib.PrivateCore._UnsafeMethods::_unprotect(void*, int32)
        
        ret
    }
    
    .method public hidebysig static bool  MemUnprotect(native int ptr, int32 size) cil managed aggressiveinlining
    {
        .maxstack 3
        
        ldarg.0
        call void* [mscorlib]System.IntPtr::op_Explicit(native int)
        ldarg.1
        tail.
        call bool SuperComicLib.PrivateCore._UnsafeMethods::_unprotect(void*, int32)
        
        ret
    }
}