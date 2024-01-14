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
    .field public static initonly int32 platform__
    
    .method private hidebysig specialname rtspecialname static void  .cctor () cil managed 
    {
        .maxstack 1
        
        call int32 SuperComicLib.PrivateCore._Runtime::_GetPlatfromID()
        stsfld int32 SuperComicLib.PrivateCore._Runtime::platform__
        
        ret
    }
    
    .method private hidebysig static int32  _GetPlatfromID() cil managed
    {
        .maxstack 2
        
        call class [mscorlib]System.OperatingSystem [mscorlib]System.Environment::get_OSVersion()
        callvirt instance valuetype [mscorlib]System.PlatformID [mscorlib]System.OperatingSystem::get_Platform()
        ret
    }
}