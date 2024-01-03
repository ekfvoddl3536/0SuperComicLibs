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



.class private auto ansi sealed SuperComicLib.PrivateCore._NativeImg
        extends [mscorlib]System.Object
{
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 )

    // Nested Types
    //  Byte[6]
    .class nested assembly explicit ansi sealed __staticArraySize6
            extends [mscorlib]System.ValueType
    {
        .pack 1
        .size 6

        .field [0] public initonly uint8 element
    }
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
    //  Byte[16]
    .class nested assembly explicit ansi sealed __staticArraySize16
            extends [mscorlib]System.ValueType
    {
        .pack 1
        .size 16

        .field [0] public initonly uint8 element
    }
    //  Byte[25]
    .class nested assembly explicit ansi sealed __staticArraySize25
            extends [mscorlib]System.ValueType
    {
        .pack 1
        .size 25

        .field [0] public initonly uint8 element
    }
    //  Byte[27]
    .class nested assembly explicit ansi sealed __staticArraySize27
            extends [mscorlib]System.ValueType
    {
        .pack 1
        .size 27

        .field [0] public initonly uint8 element
    }
    //  Byte[74]
    .class nested assembly explicit ansi sealed __staticArraySize74
            extends [mscorlib]System.ValueType
    {
        .pack 1
        .size 74

        .field [0] public initonly uint8 element
    }

    // Fields
    .field assembly static initonly valuetype SuperComicLib.PrivateCore._NativeImg/__staticArraySize10 __mulh at G_BYTEARRAY_0001
    .field assembly static initonly valuetype SuperComicLib.PrivateCore._NativeImg/__staticArraySize10 __umulh at G_BYTEARRAY_0002

    .field assembly static initonly valuetype SuperComicLib.PrivateCore._NativeImg/__staticArraySize13 _mul128 at G_BYTEARRAY_0003
    .field assembly static initonly valuetype SuperComicLib.PrivateCore._NativeImg/__staticArraySize13 _umul128 at G_BYTEARRAY_0004

    .field assembly static initonly valuetype SuperComicLib.PrivateCore._NativeImg/__staticArraySize10 __mulhV at VG_BYTEARRAY_0001
    .field assembly static initonly valuetype SuperComicLib.PrivateCore._NativeImg/__staticArraySize10 __umulhV at VG_BYTEARRAY_0002

    .field assembly static initonly valuetype SuperComicLib.PrivateCore._NativeImg/__staticArraySize16 _mul128V at VG_BYTEARRAY_0003
    .field assembly static initonly valuetype SuperComicLib.PrivateCore._NativeImg/__staticArraySize16 _umul128V at VG_BYTEARRAY_0004

    .field assembly static initonly valuetype SuperComicLib.PrivateCore._NativeImg/__staticArraySize74 _testabi at GG_0001
    
    .field assembly static initonly valuetype SuperComicLib.PrivateCore._NativeImg/__staticArraySize25 _cpuid at MD_0001
    .field assembly static initonly valuetype SuperComicLib.PrivateCore._NativeImg/__staticArraySize25 _cpuidV at MD_0002
    
    .field assembly static initonly valuetype SuperComicLib.PrivateCore._NativeImg/__staticArraySize27 _cpuidEx at MD_0003
    .field assembly static initonly valuetype SuperComicLib.PrivateCore._NativeImg/__staticArraySize27 _cpuidVEx at MD_0004
    
    .field assembly static initonly valuetype SuperComicLib.PrivateCore._NativeImg/__staticArraySize6 _sp at XE_0001


    // Datas
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

    .data cil VG_BYTEARRAY_0001 = bytearray (
        48 89 f0    //  mov rax, rsi
        48 f7 ef    //  imul rdi
        48 89 d0    //  mov rax, rdx
        c3          //  ret
    )
    .data cil VG_BYTEARRAY_0002 = bytearray (
        48 89 f0    //  mov rax, rsi
        48 f7 e7    //  mul rdi
        48 89 d0    //  mov rax, rdx
        c3          //  ret
    )

    .data cil VG_BYTEARRAY_0003 = bytearray (
        48 89 f0    //  mov rax, rsi
        48 89 d1    //  mov rcx, rdx
        48 f7 ef    //  imul rdi
        48 89 01    //  mov [rcx], rax
        48 89 d0    //  mov rax, rdx
        c3          //  ret
    )
    .data cil VG_BYTEARRAY_0004 = bytearray (
        48 89 f0    //  mov rax, rsi
        48 89 d1    //  mov rcx, rdx
        48 f7 e7    //  mul rdi
        48 89 01    //  mov [rcx], rax
        48 89 d0    //  mov rax, rdx
        c3          //  ret
    )

    .data cil GG_0001 = bytearray (
        48 83 f9 01     //  cmp rcx, 1
        75 18           //  jne L2
        48 83 fa 02     //  cmp rdx, 2
        75 12           //  jne L2
        49 83 f8 03     //  cmp r8, 3
        75 0c           //  jne L2
        49 83 f9 04     //  cmp r9, 4
        75 06           //  jne L2

        b8 01 00 00 00  //  mov eax, 1
        c3              //  ret

        // L2:
        48 83 ff 01     //  cmp rdi, 1
        75 24           //  jne L3
        48 83 fe 02     //  cmp rsi, 2
        75 1e           //  jne L3
        48 83 fa 03     //  cmp rdx, 3
        75 18           //  jne L3
        48 83 f9 04     //  cmp rcx, 4
        75 12           //  jne L3
        49 83 f8 05     //  cmp r8, 5
        75 0c           //  jne L3
        49 83 f9 06     //  cmp r9, 6
        75 06           //  jne L3

        b8 02 00 00 00  //  mov eax, 2
        c3              //  ret

        // L3:
        31 c0           //  xor eax, eax
        c3              //  ret
    )
    
    .data cil MD_0001 = bytearray (
        53              // push   rbx
        89 c8           // mov    eax,ecx
        49 89 d0        // mov    r8,rdx
        0f a2           // cpuid
        49 89 00        // mov    [r8],rax
        49 89 58 08     // mov    [r8+0x8],rbx
        49 89 48 10     // mov    [r8+0x10],rcx
        49 89 50 18     // mov    [r8+0x18],rdx
        5b              // pop    rbx
        c3              // ret
    )
    .data cil MD_0002 = bytearray (
        53              //  push   rbx
        89 f8           //  mov    eax,edi
        0f a2           //  cpuid
        48 89 06        //  mov    [rsi],rax
        48 89 5e 08     //  mov    [rsi+0x8],rbx
        48 89 4e 10     //  mov    [rsi+0x10],rcx
        48 89 56 18     //  mov    [rsi+0x18],rdx
        5b              //  pop    rbx
        c3              //  ret
        00 00 00        //  -- padding --
    )
    
    .data cil MD_0003 = bytearray (
        53              //  push   rbx
        89 c8           //  mov    eax,ecx
        89 d1           //  mov    ecx,edx
        0f a2           //  cpuid
        49 89 00        //  mov    [r8],rax
        49 89 58 08     //  mov    [r8+0x8],rbx
        49 89 48 10     //  mov    [r8+0x10],rcx
        49 89 50 18     //  mov    [r8+0x18],rdx
        5b              //  pop    rbx
        c3              //  ret
        00 00 00        //  -- padding --
    )
    .data cil MD_0004 = bytearray (
        53              //  push   rbx
        89 f8           //  mov    eax,edi
        89 f1           //  mov    ecx,esi
        49 89 d0        //  mov    r8,rdx
        0f a2           //  cpuid
        49 89 00        //  mov    [r8],rax
        49 89 58 08     //  mov    [r8+0x8],rbx
        49 89 48 10     //  mov    [r8+0x10],rcx
        49 89 50 18     //  mov    [r8+0x18],rdx
        5b              //  pop    rbx
        c3              //  ret
    )
    
    .data cil XE_0001 = bytearray (
        48 8d 44 24 10  //  lea    rax,[rsp+0x10]
        c3              //  ret
    )
    
    
    
    // Methods
    .method assembly hidebysig static void*  prologue(valuetype [mscorlib]System.RuntimeMethodHandle target, int32 size) cil managed
    {
        .maxstack 8
        
        .locals (
            [0] uint8*,
            [1] valuetype [mscorlib]System.RuntimeMethodHandle
        )
        
        ldarg.0
        stloc.1
        
        ldloca.s 1
        call instance native int [mscorlib]System.RuntimeMethodHandle::GetFunctionPointer()
        tail.
        call void* SuperComicLib.PrivateCore._NativeImg::prologue(native int, int32)
        ret
    }
    
    .method assembly hidebysig static void*  prologue(native int tok, int32 size) cil managed
    {
        .maxstack 8
        
        .locals (
            [0] int8*,
            [1] int8*
        )
        
        ldarg.0
        call void* [mscorlib]System.IntPtr::op_Explicit(native int)
        stloc.0
        
        // NOTE:: .NET Framework, <= .NET 6, Mono
        // test 'jmp' code
        ldloc.0
        ldind.u1
        ldc.i4 0xE9
        beq.s L001
        
        // NOTE:: >= .NET 7
        // test 'jmp (ff /4)' code
        ldloc.0
        ldind.u2
        ldc.i4 0x25FF
        bne.un.s G_RETURN
        
    L002:
        // jump!
        //  r = *(uint*)(x + 2) + 6
        //  x = *(long*)(x + r)        
        ldloc.0
        ldloc.0
        ldc.i4.2
        add
        ldind.u4
        ldc.i4.6
        add
        conv.u
        add
        
        // back-up
        dup
        stloc.1
        
        ldind.i8
        conv.u
        stloc.0
        
        // unprotect MEMORY
        ldloc.0
        ldarg.1
        call bool SuperComicLib.PrivateCore._Runtime::_unprotect(void*, int32)
        brtrue.s G_RETURN
        
        //  AOT ERROR!!
        //  need to allocate a data area and use it
        ldarg.0
        ldloc.1
        ldarg.1
        call void* SuperComicLib.PrivateCore._Runtime::_AOTmaster(void*, int8*, int32)
        stloc.0
        
        br.s G_RETURN
        
    L001:
        // jump!
        //  x += *(uint*)(x + 1) + 5
        ldloc.0
        ldloc.0
        ldc.i4.1
        add
        ldind.u4
        ldc.i4.5
        add
        conv.u
        add
        stloc.0
        
        // unprotect MEMORY
        ldloc.0
        ldarg.1
        call bool SuperComicLib.PrivateCore._Runtime::_unprotect(void*, int32)
        pop
        
        // AOT is supported starting from .NET 7, and .NET 7 does not seem to use this jmp method.
        
    G_RETURN:
        ldloc.0
        ret
    }
    
    .method assembly hidebysig static void  patch(valuetype [mscorlib]System.RuntimeMethodHandle tok, [in] uint8& asm, int32 size) cil managed noinlining
    {
        .param [2]
        .custom instance void [mscorlib]System.Runtime.CompilerServices.IsReadOnlyAttribute::.ctor() = ( 01 00 00 00 )

        .maxstack 8

        .locals (
            [0] uint8*,
            [1] int32,
            [2] valuetype [mscorlib]System.RuntimeMethodHandle
        )
        
        ldarg.0
        stloc.2
        
        ldloca.s 2
        call instance native int [mscorlib]System.RuntimeMethodHandle::GetFunctionPointer()
        ldarg.2
        call void* SuperComicLib.PrivateCore._NativeImg::prologue(native int, int32)
        stloc.0
        
        ldc.i4.0
        stloc.1
        
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