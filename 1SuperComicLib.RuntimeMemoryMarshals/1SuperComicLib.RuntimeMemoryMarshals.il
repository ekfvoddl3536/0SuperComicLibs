// MIT License
//
// Copyright (c) 2023. SuperComic (ekfvoddl3535@naver.com)
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
  .ver 3:5:0:0
}
.assembly '1SuperComicLib.RuntimeMemoryMarshals'
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilationRelaxationsAttribute::.ctor(int32) = ( 01 00 08 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.RuntimeCompatibilityAttribute::.ctor() = ( 01 00 01 00 54 02 16 57 72 61 70 4E 6F 6E 45 78   // ....T..WrapNonEx
                                                                                                             63 65 70 74 69 6F 6E 54 68 72 6F 77 73 01 )       // ceptionThrows.

  .custom instance void [mscorlib]System.Reflection.AssemblyTitleAttribute::.ctor(string) = ( 01 00 24 31 53 75 70 65 72 43 6F 6D 69 63 4C 69   // ..$1SuperComicLi
                                                                                              62 2E 52 75 6E 74 69 6D 65 4D 65 6D 6F 72 79 4D   // b.RuntimeMemoryM
                                                                                              61 72 73 68 61 6C 73 00 00 )                      // arshals..
  .custom instance void [mscorlib]System.Reflection.AssemblyDescriptionAttribute::.ctor(string) = ( 01 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Reflection.AssemblyConfigurationAttribute::.ctor(string) = ( 01 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Reflection.AssemblyCompanyAttribute::.ctor(string) = ( 01 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Reflection.AssemblyProductAttribute::.ctor(string) = ( 01 00 24 31 53 75 70 65 72 43 6F 6D 69 63 4C 69   // ..$1SuperComicLi
                                                                                                62 2E 52 75 6E 74 69 6D 65 4D 65 6D 6F 72 79 4D   // b.RuntimeMemoryM
                                                                                                61 72 73 68 61 6C 73 00 00 )                      // arshals..
  .custom instance void [mscorlib]System.Reflection.AssemblyCopyrightAttribute::.ctor(string) = ( 01 00 4C 43 6F 70 79 72 69 67 68 74 20 C2 A9 20   // ..LCopyright .. 
                                                                                                  32 30 32 33 2E 20 53 75 70 65 72 43 6F 6D 69 63   // 2023. SuperComic
                                                                                                  20 28 65 6B 66 76 6F 64 64 6C 33 35 33 35 40 6E   //  (ekfvoddl3535@n
                                                                                                  61 76 65 72 2E 63 6F 6D 29 2E 20 41 6C 6C 20 72   // aver.com). All r
                                                                                                  69 67 68 74 73 20 72 65 73 65 72 76 65 64 2E 00   // ights reserved..
                                                                                                  00 ) 
  .custom instance void [mscorlib]System.Reflection.AssemblyTrademarkAttribute::.ctor(string) = ( 01 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.InteropServices.ComVisibleAttribute::.ctor(bool) = ( 01 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.InteropServices.GuidAttribute::.ctor(string) = ( 01 00 24 36 35 34 36 38 38 64 32 2D 65 61 38 32   // ..$654688d2-ea82
                                                                                                  2D 34 37 66 38 2D 39 35 65 39 2D 61 65 30 31 38   // -47f8-95e9-ae018
                                                                                                  35 34 32 38 37 31 34 00 00 )                      // 5428714..
  .custom instance void [mscorlib]System.Reflection.AssemblyFileVersionAttribute::.ctor(string) = ( 01 00 07 31 2E 30 2E 30 2E 30 00 00 )             // ...1.0.0.0..
  .custom instance void [mscorlib]System.Runtime.Versioning.TargetFrameworkAttribute::.ctor(string) = ( 01 00 1C 2E 4E 45 54 46 72 61 6D 65 77 6F 72 6B   // ....NETFramework
                                                                                                        2C 56 65 72 73 69 6F 6E 3D 76 34 2E 37 2E 31 01   // ,Version=v4.7.1.
                                                                                                        00 54 0E 14 46 72 61 6D 65 77 6F 72 6B 44 69 73   // .T..FrameworkDis
                                                                                                        70 6C 61 79 4E 61 6D 65 14 2E 4E 45 54 20 46 72   // playName..NET Fr
                                                                                                        61 6D 65 77 6F 72 6B 20 34 2E 37 2E 31 )          // amework 4.7.1
  .custom instance void [mscorlib]System.CLSCompliantAttribute::.ctor(bool) = ( 01 00 00 00 00 )

  .permissionset reqmin  = (
        2e 01 80 84 53 79 73 74 65 6d 2e 53 65 63 75 72
        69 74 79 2e 50 65 72 6d 69 73 73 69 6f 6e 73 2e
        53 65 63 75 72 69 74 79 50 65 72 6d 69 73 73 69
        6f 6e 41 74 74 72 69 62 75 74 65 2c 20 6d 73 63
        6f 72 6c 69 62 2c 20 56 65 72 73 69 6f 6e 3d 34
        2e 30 2e 30 2e 30 2c 20 43 75 6c 74 75 72 65 3d
        6e 65 75 74 72 61 6c 2c 20 50 75 62 6c 69 63 4b
        65 79 54 6f 6b 65 6e 3d 62 37 37 61 35 63 35 36
        31 39 33 34 65 30 38 39 15 01 54 02 10 53 6b 69
        70 56 65 72 69 66 69 63 61 74 69 6f 6e 01
  )

  .hash algorithm 0x00008004
  .ver 1:0:0:0
}
.module '1SuperComicLib.RuntimeMemoryMarshals.dll'
.custom instance void [mscorlib]System.Security.UnverifiableCodeAttribute::.ctor() = ( 01 00 00 00 )





// !============================================================== HELPERS ========================================================!
//
//
//
// !============================================================== CLASSES ========================================================!

.class private sequential ansi sealed beforefieldinit SuperComicLib.RuntimeMemoryMarshals.iff__asref`1<T>
    extends [mscorlib]System.Object
{
	.field public !T item
} // end of 'iff__asref'


// dotnet-coreclr array
.class private sequential ansi sealed beforefieldinit SuperComicLib.RuntimeMemoryMarshals.iffDotnet__asref`1<T>
    extends [mscorlib]System.Object
{
    .field public initonly native int _length
    .field public !T item
} // end of 'iffDotnet__asref'


// mono array (use constant value 0x20)
//
//  !======= DELETED ========!
//

// 레퍼런스 generic에 대한 share가 작용?? -> 어떻게 해결?
//	원인: 타입 불일치 (또는 예측 불가)에 따른 jit 인라인 불가
//	로컬 변수 사용하여 저장&로드 해서, 타입 일치 시켜서 해결
//     --> jit 인라인은 되나, 최적화된 소스가 생성이 안됨
//          --> 해결:   반환 타입을 일치시키는 방식을 사용하되, struct을 사용하는 방식을 취함
//                      자세한 내용은 blog에 게시된 글을 확인 --> https://blog.naver.com/ekfvoddl3535/223018679855
//          
//          --> 새로운 문제, 그리고 해결:
//              https://blog.naver.com/ekfvoddl3535/223043671536