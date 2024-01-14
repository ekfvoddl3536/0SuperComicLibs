// MIT License
//
// Copyright (c) 2023-2024. SuperComic (ekfvoddl3535@naver.com)
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


// Metadata
.assembly extern mscorlib
{
    .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )
    .ver 4:0:0:0
}
.assembly extern '0SuperComicLib.Core'
{
   .ver 4:0:0:0
}

.assembly '1SuperComicLib.Runtime'
{
    .custom instance void [mscorlib]System.Runtime.CompilerServices.ExtensionAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilationRelaxationsAttribute::.ctor(int32) = ( 01 00 08 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.RuntimeCompatibilityAttribute::.ctor() = ( 
        01 00 01 00 54 02 16 57 72 61 70 4E 6F 6E 45 78   // ....T..WrapNonEx
        63 65 70 74 69 6F 6E 54 68 72 6F 77 73 01 )       // ceptionThrows.
    
    .custom instance void [mscorlib]System.Reflection.AssemblyTitleAttribute::.ctor(string) = ( 01 00 16 31 53 75 70 65 72 43 6F 6D 69 63 4C 69   // ...1SuperComicLi
                                                                                                62 2E 52 75 6E 74 69 6D 65 00 00 )                // b.Runtime..
    
    .custom instance void [mscorlib]System.Reflection.AssemblyDescriptionAttribute::.ctor(string) = ( 01 00 00 00 00 ) 
    
    .custom instance void [mscorlib]System.Reflection.AssemblyConfigurationAttribute::.ctor(string) = ( 01 00 00 00 00 ) 
    
    .custom instance void [mscorlib]System.Reflection.AssemblyCompanyAttribute::.ctor(string) = ( 01 00 00 00 00 ) 
    
    .custom instance void [mscorlib]System.Reflection.AssemblyProductAttribute::.ctor(string) = ( 01 00 16 31 53 75 70 65 72 43 6F 6D 69 63 4C 69   // ...1SuperComicLi
                                                                                                  62 2E 52 75 6E 74 69 6D 65 00 00 )                // b.Runtime..
    
    .custom instance void [mscorlib]System.Reflection.AssemblyCopyrightAttribute::.ctor(string) = ( 01 00 51 43 6F 70 79 72 69 67 68 74 20 C2 A9 20   // ..QCopyright .. 
                                                                                                    32 30 32 33 2D 32 30 32 34 2E 20 53 75 70 65 72   // 2023-2024. Super
                                                                                                    43 6F 6D 69 63 20 28 65 6B 66 76 6F 64 64 6C 33   // Comic (ekfvoddl3
                                                                                                    35 33 35 40 6E 61 76 65 72 2E 63 6F 6D 29 2E 20   // 535@naver.com). 
                                                                                                    41 6C 6C 20 72 69 67 68 74 73 20 72 65 73 65 72   // All rights reser
                                                                                                    76 65 64 2E 00 00 )                               // ved...
    
    .custom instance void [mscorlib]System.Reflection.AssemblyTrademarkAttribute::.ctor(string) = ( 01 00 00 00 00 ) 
    
    .custom instance void [mscorlib]System.Runtime.InteropServices.ComVisibleAttribute::.ctor(bool) = ( 01 00 00 00 00 ) 
    
    .custom instance void [mscorlib]System.Runtime.InteropServices.GuidAttribute::.ctor(string) = ( 01 00 24 39 65 30 61 61 37 37 62 2D 36 37 35 36   // ..$9e0aa77b-6756
                                                                                                    2D 34 37 63 34 2D 39 38 62 32 2D 32 36 38 65 34   // -47c4-98b2-268e4
                                                                                                    32 66 36 30 36 64 32 00 00 )                      // 2f606d2..
    
    .custom instance void [mscorlib]System.Reflection.AssemblyFileVersionAttribute::.ctor(string) = ( 01 00 07 34 2E 31 2E 30 2E 30 00 00 )             // ...4.1.0.0..
    
    .custom instance void [mscorlib]System.Runtime.Versioning.TargetFrameworkAttribute::.ctor(string) = ( 01 00 1C 2E 4E 45 54 46 72 61 6D 65 77 6F 72 6B   // ....NETFramework
                                                                                                          2C 56 65 72 73 69 6F 6E 3D 76 34 2E 37 2E 31 01   // ,Version=v4.7.1.
                                                                                                          00 54 0E 14 46 72 61 6D 65 77 6F 72 6B 44 69 73   // .T..FrameworkDis
                                                                                                          70 6C 61 79 4E 61 6D 65 14 2E 4E 45 54 20 46 72   // playName..NET Fr
                                                                                                          61 6D 65 77 6F 72 6B 20 34 2E 37 2E 31 )          // amework 4.7.1
    
    .permissionset reqmin = {[mscorlib]System.Security.Permissions.SecurityPermissionAttribute = {property bool 'SkipVerification' = bool(true)}}
    
    .hash algorithm 0x00008004
    .ver 4:1:0:0
}

.module '1SuperComicLib.Runtime.dll'
.custom instance void [mscorlib]System.Security.UnverifiableCodeAttribute::.ctor() = ( 01 00 00 00 )





// !============================================================== HELPERS ========================================================!
//
//
//
// !============================================================== CLASSES ========================================================!

.class private sequential ansi sealed beforefieldinit SuperComicLib.Runtime.iff__asref`1<T>
    extends [mscorlib]System.Object
{
	.field public !T item
} // end of 'iff__asref'


// coreclr array
.class private sequential ansi sealed beforefieldinit SuperComicLib.Runtime.iffCLR__asref`1<T>
    extends [mscorlib]System.Object
{
    .field public initonly native int _length
    .field public !T item
} // end of 'iffCLR__asref'

// 레퍼런스 generic에 대한 share가 작용?? -> 어떻게 해결?
//	원인: 타입 불일치 (또는 예측 불가)에 따른 jit 인라인 불가
//	로컬 변수 사용하여 저장&로드 해서, 타입 일치 시켜서 해결
//     --> jit 인라인은 되나, 최적화된 소스가 생성이 안됨
//          --> 해결:   반환 타입을 일치시키는 방식을 사용하되, struct을 사용하는 방식을 취함
//                      자세한 내용은 blog에 게시된 글을 확인 --> https://blog.naver.com/ekfvoddl3535/223018679855
//          
//          --> 새로운 문제, 그리고 해결:
//              https://blog.naver.com/ekfvoddl3535/223043671536