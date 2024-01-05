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


.class private auto ansi sealed SuperComicLib.PrivateCore._UnsafeMethods
        extends [mscorlib]System.Object
{
    // return: if true, success
    .method public static bool  _unprotect(void* target, int32 size) cil managed
    {
        .maxstack 8
        
        // NOTE(ko-kr)::
        //  .NET 7 부터 런타임 버전이 바뀌면서, 코드 영역에 보호가 들어감
        //  이를 해제해야함
        //  윈도우에서는 net6.0 net7.0 net8.0이 테스트됐고, net7.0부터 임을 확인
        //  리눅스(우분투 22.04)는 net6.0 net8.0이 테스트됐고, net8.0에서 메모리 권한 문제가 발생함을 확인
        //  ---> (RE-WRITE)
        //  .NET 5/.NET Core 3.0+ ReadyToRun을 포함해야함. ReadyToRun이 있는 경우, 메모리 보호 문제가 생길 수 있음
        //  .NET Core 3.0+의 Major 버전이 3으로 .NET Framework 4.7.1의 Major 버전인 4보다 작으므로
        //  이 부분을 판별할 필요가 없어짐
        
        // ldsfld int32 SuperComicLib.PrivateCore._Runtime::version__ 
        // ldc.i4.s 5
        // blt.un.s G_SUCCESS
        
        ldarg.1
        ldc.i4.0
        beq.s G_SUCCESS
        
        // is windows?
        ldsfld int32 SuperComicLib.PrivateCore._Runtime::platform__
        ldc.i4.2
        bne.un.s L_001
        
        ldarg.0
        ldarg.1
        call int32 SuperComicLib.PrivateCore._UnsafeMethods::impl__unprotectWinNT(void*, int32)
        
        //  ref.: https://learn.microsoft.com/en-us/windows/win32/api/memoryapi/nf-memoryapi-virtualprotect
        //  != 0
        ldc.i4.0
        cgt.un
        ret
    
    L_001:
        
        // is unix (Linux)?
        ldsfld int32 SuperComicLib.PrivateCore._Runtime::platform__
        ldc.i4.4
        bne.un.s G_FAIL
        
        ldarg.0
        ldarg.1
        call int32 SuperComicLib.PrivateCore._UnsafeMethods::impl__unprotectUnix(void*, int32)
        
        //  ref.: https://man7.org/linux/man-pages/man2/mprotect.2.html
        //  == 0
        ldc.i4.0
        ceq
        ret
        
    G_FAIL:
        //  -- FAIL --
        ldc.i4.0
        ret
        
    G_SUCCESS:
        //  -- SUCCESS --
        ldc.i4.1
        ret
    }
    
    .method public static int32  impl__unprotectWinNT(void* target, int32 size) cil managed
    {
        .maxstack 8
        
        .locals (
            [0] int32
        )
        
        ldarg.0
        // set 'dwSize'
        ldarg.1
        conv.i
        // PAGE_EXECUTE_READWRITE
        ldc.i4.s 0x40
        // pass 'dummy pointer'
        ldloca.s 0
        conv.u
        call int32 SuperComicLib.PrivateCore._UnsafeMethods::'<WinNT>__PInvoke'(void*, native int, int32, int32*)
        
        ret
    }
    
    .method public static int32  impl__unprotectUnix(void* target, int32 size) cil managed
    {
        .maxstack 8
        
        .locals (
            [0] native int,
            [1] void*,
            [2] int32
        )
        
        // align address AND, re-calc length
        call int32 [mscorlib]System.Environment::get_SystemPageSize()
        conv.u
        stloc.0
        
        //  align = (target & -pg)
        ldarg.0
        ldloc.0
        neg
        and
        stloc.1
        
        // set 'len' (re-calc len)
        //  target & (pg - 1)
        ldarg.0
            ldloc.0
            ldc.i4.1
            sub
        and
        //  + size
        ldarg.1
        conv.i
        add
        conv.i4
        stloc.2
        
        ldloc.1
        // set 'len'
        ldloc.2
        conv.i
        // PROT_READ | PROT_WRITE | PROT_EXECUTE
        ldc.i4.s 7
        call int32 SuperComicLib.PrivateCore._UnsafeMethods::'<GNU>mprotect__PInvoke'(void*, native int, int32)
        
        ret
    }
    
    .method public hidebysig static void*  _AOTmaster(int8* mem, int32 size) cil managed noinlining
    {
        .maxstack 8
        
        .locals (
            [0] native int,
            [1] int32
        )
        
        // 16-bytes align
        ldarg.1
        ldc.i4.s 15
        add
        ldc.i4.s -16
        and
        stloc.1
        
        ldarg.0
        ldc.i4.8
        call bool SuperComicLib.PrivateCore._UnsafeMethods::_unprotect(void*, int32)
        brfalse.s F_NOTSUPPORT
        
        ldloc.1
        call native int [mscorlib]System.Runtime.InteropServices.Marshal::AllocHGlobal(int32)
        stloc.0
        
        // modify address
        ldarg.0
        ldloc.0
        stind.i
        
        ldloc.0
        ldloc.1
        call bool SuperComicLib.PrivateCore._UnsafeMethods::_unprotect(void*, int32)
        // why fail???
        brfalse.s F_NOTSUPPORT
        
        // SUCCESS!
        ldloc.0
        call void* [mscorlib]System.IntPtr::op_Explicit(native int)
        ret
        
    F_NOTSUPPORT:
        newobj instance void [mscorlib]System.NotSupportedException::.ctor()
        throw
    }
    
    .method private hidebysig static void*  'jmp/4'(void* ptr) cil managed aggressiveinlining
    {
        .maxstack 3
        
        // jump!
        //  r = *(uint*)(x + 2) + 6
        //  x = *(long*)(x + r)        
        ldarg.0
            ldarg.0
            ldc.i4.2
            add
            ldind.u4
            ldc.i4.6
            add
        add
        
        ret
    }
    
    .method private hidebysig static void*  prologue_cplxAOT(void* ptr) cil managed
    {
        .maxstack 8
        
        .locals (
            [0] void*,
            [1] void*
        )
        
        ldarg.0
        call void* SuperComicLib.PrivateCore._UnsafeMethods::'jmp/4'(void*)
        // back-up
        dup
        stloc.1
        // read value
        ldind.i
        dup
        stloc.0
        
        // NOTE:: >= .NET 7.0 / PGO(?) jmp
        //  mov rax, [rip+m32]
        //  dec word [rax]
        //  je
        //  jmp /4
        ldind.i4
        ldc.i4 0xFFFFFF
        and
        ldc.i4 0x058B48
        bne.un.s G_RETURN
        
        ldloc.0
        ldc.i4.7
        add
        ldind.i8
        ldc.i8 0x00FFFFFFFFFFFFFF
        and
        ldc.i8 0x0025FF067408FF66
        bne.un.s G_RETURN
        
        ldloc.0
        ldc.i4.s 12
        add
        tail.
        call void* SuperComicLib.PrivateCore._UnsafeMethods::'jmp/4'(void*)
        ret
        
    G_RETURN:
        ldloc.1
        ret
    }
    
    .method private hidebysig static void*  '<>prologue/ff25'(void* ptr, int32 size) cil managed
    {
        .maxstack 8
        
        .locals (
            [0] int8*,
            [1] int8*
        )
        
        ldarg.0
        call void* SuperComicLib.PrivateCore._UnsafeMethods::prologue_cplxAOT(void*)
        dup
        stloc.1
        ldind.i
        dup
        stloc.0

        // unprotect MEMORY
        ldarg.1
        call bool SuperComicLib.PrivateCore._UnsafeMethods::_unprotect(void*, int32)
        brtrue.s G_RETURN
        
        //  AOT ERROR!!
        //  need to allocate a data area and use it
        ldloc.1
        ldarg.1
        tail.
        call void* SuperComicLib.PrivateCore._UnsafeMethods::_AOTmaster(int8*, int32)
        
        ret
        
    G_RETURN:
        ldloc.0
        ret
    }
    
    .method private hidebysig static void*  prologue_cplxRTR(void* ptr) cil managed
    {
        .maxstack 8
        
        // movabs rax, imm64
        ldarg.0
        ldind.u2
        ldc.i4 0xB848
        bne.un.s G_RETURN
        
        // jmp rax
        ldarg.0
        ldc.i4.s 10
        add
        ldind.u2
        ldc.i4 0xE0FF
        bne.un.s G_RETURN
        
        ldarg.0
        ldc.i4.2
        add
        ldind.i
        ret
        
    G_RETURN:
        ldarg.0
        ret
    }
    
    .method private hidebysig static void*  '<>prologue/e9'(void* ptr, int32 size) cil managed
    {
        .maxstack 8
        
        .locals (
            [0] int8*,
            [1] int8*
        )
        
        // jump!
        //  k = x + *(int*)(x + 1) + 5
        ldarg.0
            ldarg.0
            ldc.i4.1
            add
            // must be 'signed'. DON't UINT!
            ldind.i4
            conv.i
            add
        ldc.i4.5
        add
        // back-up
        dup
        stloc.1
        
        //  Ready-To-Run ERROR!!!
        call void* SuperComicLib.PrivateCore._UnsafeMethods::prologue_cplxRTR(void*)
        dup
        stloc.0
        
        // unprotect MEMORY
        ldarg.1
        call bool SuperComicLib.PrivateCore._UnsafeMethods::_unprotect(void*, int32)
        brtrue.s G_RETURN
        
        ldloc.0
        ldloc.1
        beq.s FAIL_NOTSUPPORT
        
        // AOT is supported starting from .NET 7, and .NET 7 does not seem to use this jmp method.
        //  Ready-To-Run ERROR!!!
        ldloc.1
        ldc.i4.2
        add
        ldarg.1
        tail.
        call void* SuperComicLib.PrivateCore._UnsafeMethods::_AOTmaster(int8*, int32)
        
        ret
        
    G_RETURN:
        ldloc.0
        ret
    
    FAIL_NOTSUPPORT:
        // ldstr "X"
        // call instance string [mscorlib]System.Int64::ToString(string)
        newobj instance void [mscorlib]System.NotSupportedException::.ctor()
        throw
    }
    
    // Methods
    .method assembly hidebysig static void*  prologue(native int tok, int32 size) cil managed
    {
        .maxstack 8
        
        .locals (
            [0] int8*
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
        bne.un.s L003
        
        ldloc.0
        ldarg.1
        tail.
        call void* SuperComicLib.PrivateCore._UnsafeMethods::'<>prologue/ff25'(void*, int32)
        
        ret
        
    L001:
        ldloc.0
        ldarg.1
        tail.
        call void* SuperComicLib.PrivateCore._UnsafeMethods::'<>prologue/e9'(void*, int32)
        
        ret
        
    L003:
        ldloc.0
        ldarg.1
        call bool SuperComicLib.PrivateCore._UnsafeMethods::_unprotect(void*, int32)
        pop
        
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
            [1] int32
        )
        
        ldarga.s 0
        call instance native int [mscorlib]System.RuntimeMethodHandle::GetFunctionPointer()
        ldarg.2
        call void* SuperComicLib.PrivateCore._UnsafeMethods::prologue(native int, int32)
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
    
    
    
    
    // P/Invoke methods
    // WinNT
    .method private hidebysig static pinvokeimpl("kernel32.dll" as "VirtualProtect" nomangle winapi) 
        int32 '<WinNT>__PInvoke' (
            void* lpAddress,
            native int dwSize,
            int32 flNewProtect,
            int32* lpflOldProtect
        ) cil managed preservesig
    {
    }
    
    // Unix (Linux)
    .method private hidebysig static pinvokeimpl("libc.so.6" as "mprotect" nomangle winapi) 
        int32 '<GNU>mprotect__PInvoke' (
            void* addr,
            native int len,
            int32 prot
        ) cil managed preservesig
    {
    }
}