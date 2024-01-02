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
    
    .method public hidebysig static void  _unprotect(void* target) cil managed
    {
        .maxstack 8
        
        ldsfld int32 SuperComicLib.PrivateCore._Runtime::version__ 
        ldc.i4.s 7
        blt.un.s G_RETURN
        
        // is windows?
        ldsfld int32 SuperComicLib.PrivateCore._Runtime::platform__
        ldc.i4.2
        bne.un.s L_001
        
        ldarg.0
        call void SuperComicLib.PrivateCore._Runtime::impl__unprotectWinNT(void*)
        
        br.s G_RETURN
    
    L_001:
        
        // is unix (Linux)?
        ldsfld int32 SuperComicLib.PrivateCore._Runtime::platform__
        ldc.i4.4
        bne.un.s G_RETURN
        
        ldarg.0
        call void SuperComicLib.PrivateCore._Runtime::impl__unprotectUnix(void*)
        
    G_RETURN:
        ret
    }
    
    .method public hidebysig static void  impl__unprotectWinNT(void* target) cil managed noinlining
    {
        .maxstack 8
        
        .locals (
            [0] int32
        )
        
        ldarg.0
        // dwSize = 1 byte
        ldc.i4.1
        conv.i
        // PAGE_EXECUTE_READWRITE
        ldc.i4.s 0x40
        ldloca.s 0
        conv.u
        call int32 SuperComicLib.PrivateCore._Runtime::'<WinNT>__PInvoke'(void*, native int, int32, int32*)
        pop
        
        ret
    }
    
    .method public hidebysig static void  impl__unprotectUnix(void* target) cil managed noinlining
    {
        .maxstack 8
        
        .locals (
            [0] native int
        )
        
        // -(nint)getpagesize
        call int32 SuperComicLib.PrivateCore._Runtime::'<GNU>getpagesize__PInvoke'()
        conv.i
        neg
        stloc.0
        
        // align address
        ldarg.0
        ldloc.0
        and
        // len = 1 byte
        ldc.i4.1
        conv.i
        // PROT_READ | PROT_WRITE | PROT_EXECUTE
        ldc.i4.s 7
        call int32 SuperComicLib.PrivateCore._Runtime::'<GNU>mprotect__PInvoke'(void*, native int, int32)
        pop
        
        ret
    }
    
    // WinNT
    .method private hidebysig static pinvokeimpl("kernel32.dll" as "VirtualProtect" nomangle winapi) 
        int32 '<WinNT>__PInvoke' (
            [in] void* lpAddress,
            [in] native int dwSize,
            [in] int32 flNewProtect,
            [out] int32* lpflOldProtect
        ) cil managed preservesig
    {
    }
    
    // Unix (Linux)
    .method private hidebysig static pinvokeimpl("libc.so.6" as "getpagesize" nomangle winapi) 
        int32 '<GNU>getpagesize__PInvoke' () cil managed preservesig
    {
    }
    
    .method private hidebysig static pinvokeimpl("libc.so.6" as "mprotect" nomangle winapi) 
        int32 '<GNU>mprotect__PInvoke' (
            [in] void* addr,
            [in] native int len,
            [in] int32 prot
        ) cil managed preservesig
    {
    }
}