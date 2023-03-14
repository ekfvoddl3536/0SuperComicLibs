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
	.custom instance void [mscorlib]System.Security.SuppressUnmanagedCodeSecurityAttribute::.ctor() = ( 01 00 00 00 )
	.custom instance void [mscorlib]System.Security.SecurityCriticalAttribute::.ctor() = ( 01 00 00 00 )

	.method public hidebysig static !!T  AsClass<class T>(void* 'value') cil managed aggressiveinlining
	{
		.maxstack 1
		.locals (
            [0] !!T
        )

		ldarg.0
		stloc.0
		ldloc.0
		ret
	}

	.method public hidebysig static void*  AsPointer(object 'value') cil managed aggressiveinlining
	{
		.maxstack 1
		.locals (
			[0] native int
		)
		
		ldarg.0
		stloc.0
		ldloc.0
		ret
	}

	.method public hidebysig static void*  AsPointer(object 'value', native int byteOffset) cil managed aggressiveinlining
	{
		.maxstack 2
		.locals (
			[0] native int
		)
		
		ldarg.0
		stloc.0
		ldloc.0
		ldarg.1
		add
		ret
	}

	.method public hidebysig static !!TField&  GetDataRef<TField>(object class_references) cil managed aggressiveinlining
	{
		.maxstack 1
		
		ldarg.0
		call !!0 SuperComicLib.RuntimeMemoryMarshals.ILUnsafe::icall__SystemRuntimeCompilerServicesUnsafeAs<class SuperComicLib.RuntimeMemoryMarshals.iff__asref`1<!!TField>>(object)
		ldflda !0 class SuperComicLib.RuntimeMemoryMarshals.iff__asref`1<!!TField>::item
		ret
	}

	.method public hidebysig static !!TField&  GetDataRef<TField>(object class_references, native int byteOffset) cil managed aggressiveinlining
	{
		.maxstack 2
		
		ldarg.0
		call !!0 SuperComicLib.RuntimeMemoryMarshals.ILUnsafe::icall__SystemRuntimeCompilerServicesUnsafeAs<class SuperComicLib.RuntimeMemoryMarshals.iff__asref`1<!!TField>>(object)
		ldflda !0 class SuperComicLib.RuntimeMemoryMarshals.iff__asref`1<!!TField>::item
		ldarg.1
		add
		ret
	}

	.method public hidebysig static !!TField*  GetDataPtr<valuetype .ctor (class [mscorlib]System.ValueType modreq([mscorlib]System.Runtime.InteropServices.UnmanagedType)) TField>(object class_references) cil managed aggressiveinlining
	{
		.param type TField
        .custom instance void System.Runtime.CompilerServices.IsUnmanagedAttribute::.ctor() = ( 01 00 00 00 )

		.maxstack 1
		
		ldarg.0
		call !!0 SuperComicLib.RuntimeMemoryMarshals.ILUnsafe::icall__SystemRuntimeCompilerServicesUnsafeAs<class SuperComicLib.RuntimeMemoryMarshals.iff__asref`1<!!TField>>(object)
		ldflda !0 class SuperComicLib.RuntimeMemoryMarshals.iff__asref`1<!!TField>::item
		conv.u
		ret
	}

	.method public hidebysig static !!TField*  GetDataPtr<valuetype .ctor (class [mscorlib]System.ValueType modreq([mscorlib]System.Runtime.InteropServices.UnmanagedType)) TField>(object class_references, native int byteOffset) cil managed aggressiveinlining
	{
		.param type TField
        .custom instance void System.Runtime.CompilerServices.IsUnmanagedAttribute::.ctor() = ( 01 00 00 00 )

		.maxstack 2
		
		ldarg.0
		call !!0 SuperComicLib.RuntimeMemoryMarshals.ILUnsafe::icall__SystemRuntimeCompilerServicesUnsafeAs<class SuperComicLib.RuntimeMemoryMarshals.iff__asref`1<!!TField>>(object)
		ldflda !0 class SuperComicLib.RuntimeMemoryMarshals.iff__asref`1<!!TField>::item
		ldarg.1
		add
		conv.u
		ret
	}

	.method public hidebysig static !!TTo& modreq([mscorlib]System.Runtime.InteropServices.InAttribute) ReadOnlyAs<TFrom, TTo>([in] !!TFrom& 'value') cil managed aggressiveinlining
	{
		.param [0] 
		.custom instance void [mscorlib]System.Runtime.CompilerServices.IsReadOnlyAttribute::.ctor() = ( 01 00 00 00 )

        .param [1]
		.custom instance void [mscorlib]System.Runtime.CompilerServices.IsReadOnlyAttribute::.ctor() = ( 01 00 00 00 )

		.maxstack 1
		
		ldarg.0
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

	.method public hidebysig static !!T& modreq([mscorlib]System.Runtime.InteropServices.InAttribute) ReadOnlyAdd<T>([in] !!T& 'value', int32 elementOffset) cil managed aggressiveinlining
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