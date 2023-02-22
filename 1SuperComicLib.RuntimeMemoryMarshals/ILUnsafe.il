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


// ==================================================================================================================
//
//
//												2. ILUnsafe CLASS
//
//
// ==================================================================================================================

.class public abstract auto ansi sealed beforefieldinit SuperComicLib.RuntimeMemoryMarshals.ILUnsafe
       extends [mscorlib]System.Object
{
	.method public hidebysig static !!T  AsClass<class T>(void* 'value') cil managed aggressiveinlining
	{
		.maxstack 1

		ldarg.0
		newobj instance void valuetype SuperComicLib.RuntimeMemoryMarshals.iff__asclass`1<!!T>::.ctor(void*)
		ldfld !0 valuetype SuperComicLib.RuntimeMemoryMarshals.iff__asclass`1<!!T>::item
		ret
	}

	.method public hidebysig static !!T  AsClass<class T>(uint8& 'value') cil managed aggressiveinlining
	{
		.maxstack 1

		ldarg.0
		conv.i
		newobj instance void valuetype SuperComicLib.RuntimeMemoryMarshals.iff__asclass`1<!!T>::.ctor(void*)
		ldfld !0 valuetype SuperComicLib.RuntimeMemoryMarshals.iff__asclass`1<!!T>::item
		ret
	}

	.method public hidebysig static void*  AsPointer<class T>(!!T 'value') cil managed aggressiveinlining
	{
		.maxstack 1
		
		ldarg.0
		conv.i
		ret
	}

	.method public hidebysig static void*  AsPointer<class T>(!!T 'value', native int byteOffset) cil managed aggressiveinlining
	{
		.maxstack 2
		
		ldarg.0
		conv.i
		ldarg.1
		add
		ret
	}

	.method public hidebysig static uint8&  AsRef<class T>(!!T 'value') cil managed aggressiveinlining
	{
		.maxstack 2
		
		ldarg.0
		call !!0 SuperComicLib.RuntimeMemoryMarshals.ILUnsafe::icall__SystemRuntimeCompilerServicesUnsafeAs<class SuperComicLib.RuntimeMemoryMarshals.iff__asref`1<uint8>>(object)
		ldflda !0 class SuperComicLib.RuntimeMemoryMarshals.iff__asref`1<uint8>::item
		sizeof void*
		conv.i
		sub
		ret
	}

	.method public hidebysig static uint8&  AsRef<class T>(!!T 'value', native int byteOffset) cil managed aggressiveinlining
	{
		.maxstack 3
		
		ldarg.0
		call !!0 SuperComicLib.RuntimeMemoryMarshals.ILUnsafe::icall__SystemRuntimeCompilerServicesUnsafeAs<class SuperComicLib.RuntimeMemoryMarshals.iff__asref`1<uint8>>(object)
		ldflda !0 class SuperComicLib.RuntimeMemoryMarshals.iff__asref`1<uint8>::item
		ldarg.1
		sizeof void*
		conv.i
		sub
		add
		ret
	}

	.method public hidebysig static !!TField&  GetDataRef<class TClass, TField>(!!TClass 'value') cil managed aggressiveinlining
	{
		.maxstack 1
		
		ldarg.0
		call !!0 SuperComicLib.RuntimeMemoryMarshals.ILUnsafe::icall__SystemRuntimeCompilerServicesUnsafeAs<class SuperComicLib.RuntimeMemoryMarshals.iff__asref`1<!!TField>>(object)
		ldflda !0 class SuperComicLib.RuntimeMemoryMarshals.iff__asref`1<!!TField>::item
		ret
	}

	.method public hidebysig static !!TField&  GetDataRef<class TClass, TField>(!!TClass 'value', native int byteOffset) cil managed aggressiveinlining
	{
		.maxstack 2
		
		ldarg.0
		call !!0 SuperComicLib.RuntimeMemoryMarshals.ILUnsafe::icall__SystemRuntimeCompilerServicesUnsafeAs<class SuperComicLib.RuntimeMemoryMarshals.iff__asref`1<!!TField>>(object)
		ldflda !0 class SuperComicLib.RuntimeMemoryMarshals.iff__asref`1<!!TField>::item
		ldarg.1
		add
		ret
	}

	.method public hidebysig static !!TTo& modreq([mscorlib]System.Runtime.InteropServices.InAttribute) ConstAs<TFrom, TTo>([in] !!TFrom& 'value') cil managed aggressiveinlining
	{
		.param [0] 
		.custom instance void [mscorlib]System.Runtime.CompilerServices.IsReadOnlyAttribute::.ctor() = ( 01 00 00 00 )

        .param [1]
		.custom instance void [mscorlib]System.Runtime.CompilerServices.IsReadOnlyAttribute::.ctor() = ( 01 00 00 00 )

		.maxstack 1
		
		ldarg.0
		ret
	}

	.method public hidebysig static int32  Compare(object left, object right) cil managed aggressiveinlining
	{
		.maxstack 2
		
		ldarg.0
		conv.i
		ldarg.1
		conv.i
		sub
		conv.i4
		ret
	}

	.method public hidebysig static !!T&  AddByteOffset<T>(void* source, int32 byteOffset) cil managed aggressiveinlining
	{
		.maxstack 2

		ldarg.0
		ldarg.1
		conv.i
		add
		call !!1& SuperComicLib.RuntimeMemoryMarshals.ILUnsafe::icall__SystemRuntimeCompilerServicesUnsafeAs<int8, !!T>(!!0&)
		ret
	}

	.method public hidebysig static !!T&  Add<T>(void* source, int32 elementOffset, int32 addByteOffset) cil managed aggressiveinlining
	{
		.maxstack 3
		
		ldarg.0
		ldarg.1
		conv.i
		sizeof !!0
		conv.i
		mul
		add
		ldarg.2
		conv.i
		add
		call !!1& SuperComicLib.RuntimeMemoryMarshals.ILUnsafe::icall__SystemRuntimeCompilerServicesUnsafeAs<int8, !!T>(!!0&)
		ret
	}

	.method public hidebysig static !!T& modreq([mscorlib]System.Runtime.InteropServices.InAttribute) Add<T>([in] !!T& 'value', int32 elementOffset) cil managed aggressiveinlining
	{
		.param [0] 
		.custom instance void [mscorlib]System.Runtime.CompilerServices.IsReadOnlyAttribute::.ctor() = ( 01 00 00 00 )

        .param [1]
		.custom instance void [mscorlib]System.Runtime.CompilerServices.IsReadOnlyAttribute::.ctor() = ( 01 00 00 00 )

		.maxstack 2
		
		ldarg.0
		ldarg.1
		add
		ret
	}

	.method public hidebysig static int32  ConvI4(bool 'value') cil managed aggressiveinlining
	{
		.maxstack 1

		ldarg.0
		conv.i4
		ret
	}



	// refer: https://github.com/dotnet/corert/blob/master/src/System.Private.CoreLib/shared/Internal/Runtime/CompilerServices/Unsafe.cs#L77
	// do not expose this method 'public'
	.method assembly hidebysig static !!T  icall__SystemRuntimeCompilerServicesUnsafeAs<class T>(object 'value') cil managed aggressiveinlining
	{
		.maxstack 1

		ldarg.0
		ret
	}

	.method assembly hidebysig static !!TTo&  icall__SystemRuntimeCompilerServicesUnsafeAs<TFrom, TTo>(!!TFrom& source) cil managed aggressiveinlining
	{
		.maxstack 1

		ldarg.0
		ret
	}
} // end of 'ILUnsafe'