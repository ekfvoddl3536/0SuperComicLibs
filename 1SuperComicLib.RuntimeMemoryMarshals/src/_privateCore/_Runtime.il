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



.class private abstract auto ansi sealed beforefieldinit SuperComicLib.PrivateCore._Runtime
       extends [mscorlib]System.Object
{
    // Runtime major version
    .field public static initonly int32 version__
    .field public static initonly int32 platform__
    
    .method private hidebysig specialname rtspecialname static void  .cctor () cil managed 
    {
        .maxstack 1
        
        call int32 SuperComicLib.PrivateCore._Runtime::_GetVersion()
        stsfld int32 SuperComicLib.PrivateCore._Runtime::version__
        
        call int32 SuperComicLib.PrivateCore._Runtime::_GetPlatfromID()
        stsfld int32 SuperComicLib.PrivateCore._Runtime::platform__
        
        ret
    }
    
    .method private hidebysig static int32  _GetVersion() cil managed
    {
        .maxstack 1
        
        call class [mscorlib]System.Version [mscorlib]System.Environment::get_Version()
        callvirt instance int32 [mscorlib]System.Version::get_Major()
        ret
    }
    
    .method private hidebysig static int32  _GetPlatfromID() cil managed
    {
        .maxstack 2
        
        call class [mscorlib]System.OperatingSystem [mscorlib]System.Environment::get_OSVersion()
        callvirt instance valuetype [mscorlib]System.PlatformID [mscorlib]System.OperatingSystem::get_Platform()
        ret
    }
    
    .method public hidebysig static void*  _AOTmaster(void* ptr, int8* mem, int32 size) cil managed noinlining
    {
        .maxstack 8
        
        .locals (
            [0] native int,
            [1] int32
        )
        
        // 16-bytes align
        ldarg.2
        ldc.i4.s 15
        add
        ldc.i4.s -16
        and
        stloc.1
        
        ldarg.0
        ldc.i4.7
        call bool SuperComicLib.PrivateCore._Runtime::_unprotect(void*, int32)
        brfalse.s F_NOTSUPPORT
        
        ldloc.1
        call native int [mscorlib]System.Runtime.InteropServices.Marshal::AllocHGlobal(int32)
        stloc.0
        
        // modify address
        ldarg.1
        ldloc.0
        stind.i
        
        ldloc.0
        ldloc.1
        call bool SuperComicLib.PrivateCore._Runtime::_unprotect(void*, int32)
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
    
    // return: if true, success
    .method public hidebysig static bool  _unprotect(void* target, int32 size) cil managed
    {
        .maxstack 8
        
        // NOTE(ko-kr)::
        //  .NET 7 부터 런타임 버전이 바뀌면서, 코드 영역에 보호가 들어감
        //  이를 해제해야함
        //  윈도우에서는 net6.0 net7.0 net8.0이 테스트됐고, net7.0부터 임을 확인
        //  리눅스(우분투 22.04)는 net6.0 net8.0이 테스트됐고, net8.0에서 메모리 권한 문제가 발생함을 확인
        ldsfld int32 SuperComicLib.PrivateCore._Runtime::version__ 
        ldc.i4.s 7
        blt.un.s G_SUCCESS
        
        // is windows?
        ldsfld int32 SuperComicLib.PrivateCore._Runtime::platform__
        ldc.i4.2
        bne.un.s L_001
        
        ldarg.0
        ldarg.1
        call int32 SuperComicLib.PrivateCore._Runtime::impl__unprotectWinNT(void*, int32)
        
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
        call int32 SuperComicLib.PrivateCore._Runtime::impl__unprotectUnix(void*, int32)
        
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
    
    .method public hidebysig static int32  impl__unprotectWinNT(void* target, int32 size) cil managed noinlining
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
        tail.
        call int32 SuperComicLib.PrivateCore._Runtime::'<WinNT>__PInvoke'(void*, native int, int32, int32*)
        
        ret
    }
    
    .method public hidebysig static int32  impl__unprotectUnix(void* target, int32 size) cil managed noinlining
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
        tail.
        call int32 SuperComicLib.PrivateCore._Runtime::'<GNU>mprotect__PInvoke'(void*, native int, int32)
        
        ret
    }
    
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