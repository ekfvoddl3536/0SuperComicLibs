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
//										1. ArrayUnsafeGetElementReferenceExtension CLASS
//
//
// ==================================================================================================================

.class public abstract auto ansi sealed beforefieldinit SuperComicLib.RuntimeMemoryMarshals.ArrayUnsafeGetElementReferenceExtension
       extends [mscorlib]System.Object
{
	.custom instance void [mscorlib]System.Runtime.CompilerServices.ExtensionAttribute::.ctor() = ( 01 00 00 00 )

	.method public hidebysig static !!T&  refdata_dotnet<T>(!!T[] 'array') cil managed aggressiveinlining
	{
		.custom instance void [mscorlib]System.Runtime.CompilerServices.ExtensionAttribute::.ctor() = ( 01 00 00 00 )
		.custom instance void ['0SuperComicLib.Core']SuperComicLib.CodeContracts.AssumeInputsValidAttribute::.ctor() = ( 01 00 00 00 )

		.param [1]
		.custom instance void ['0SuperComicLib.Core']SuperComicLib.CodeContracts.DisallowNullAttribute::.ctor() = ( 01 00 00 00 )
	
		.maxstack 8

		ldarg.0
		call !!0 SuperComicLib.RuntimeMemoryMarshals.ILUnsafe::icall__SystemRuntimeCompilerServicesUnsafeAs<class SuperComicLib.RuntimeMemoryMarshals.iff__asref`1<!!T>>(object)
		ldflda !0 class SuperComicLib.RuntimeMemoryMarshals.iff__asref`1<!!T>::item
		sizeof void*
		add
		ret
	}

	.method public hidebysig static !!T&  refdata_dotnet<T>(!!T[] 'array', native int index) cil managed aggressiveinlining
	{
		.custom instance void [mscorlib]System.Runtime.CompilerServices.ExtensionAttribute::.ctor() = ( 01 00 00 00 )
		.custom instance void ['0SuperComicLib.Core']SuperComicLib.CodeContracts.AssumeInputsValidAttribute::.ctor() = ( 01 00 00 00 )

		.param [1]
		.custom instance void ['0SuperComicLib.Core']SuperComicLib.CodeContracts.DisallowNullAttribute::.ctor() = ( 01 00 00 00 )

		.param [2]
		.custom instance void ['0SuperComicLib.Core']SuperComicLib.CodeContracts.ValidRangeAttribute::.ctor() = ( 01 00 00 00 )
	
		.maxstack 8

		ldarg.0
		call !!0 SuperComicLib.RuntimeMemoryMarshals.ILUnsafe::icall__SystemRuntimeCompilerServicesUnsafeAs<class SuperComicLib.RuntimeMemoryMarshals.iff__asref`1<!!T>>(object)
		ldflda !0 class SuperComicLib.RuntimeMemoryMarshals.iff__asref`1<!!T>::item
		ldarg.1
		sizeof !!T
		mul
		add
		sizeof void*
		add
		ret
	}

	.method public hidebysig static !!T&  refdata_dotnet<T>([in] valuetype ['0SuperComicLib.Core']SuperComicLib.Memory`1<!!T>& 'array') cil managed aggressiveinlining
	{
		.custom instance void [mscorlib]System.Runtime.CompilerServices.ExtensionAttribute::.ctor() = ( 01 00 00 00 )
		.custom instance void ['0SuperComicLib.Core']SuperComicLib.CodeContracts.AssumeInputsValidAttribute::.ctor() = ( 01 00 00 00 )

		.param [1]
		.custom instance void [mscorlib]System.Runtime.CompilerServices.IsReadOnlyAttribute::.ctor() = (01 00 00 00 )
		.custom instance void ['0SuperComicLib.Core']SuperComicLib.CodeContracts.DisallowNullAttribute::.ctor() = ( 01 00 00 00 )
		.custom instance void ['0SuperComicLib.Core']SuperComicLib.CodeContracts.ValidRangeAttribute::.ctor() = ( 01 00 00 00 )
	
		.maxstack 8

		ldarg.0
		ldfld !0[] valuetype ['0SuperComicLib.Core']SuperComicLib.Memory`1<!!T>::_source
		call !!0 SuperComicLib.RuntimeMemoryMarshals.ILUnsafe::icall__SystemRuntimeCompilerServicesUnsafeAs<class SuperComicLib.RuntimeMemoryMarshals.iff__asref`1<!!T>>(object)
		ldflda !0 class SuperComicLib.RuntimeMemoryMarshals.iff__asref`1<!!T>::item
		ldarg.0
		ldfld int32 valuetype ['0SuperComicLib.Core']SuperComicLib.Memory`1<!!T>::_start
		conv.u
		sizeof !!T
		mul
		conv.i
		add
		sizeof void*
		add
		ret
	}

	.method public hidebysig static !!T& modreq([mscorlib]System.Runtime.InteropServices.InAttribute) refdata_dotnet<T>([in] valuetype ['0SuperComicLib.Core']SuperComicLib.ConstMemory`1<!!T>& 'array') cil managed aggressiveinlining
	{
		.custom instance void [mscorlib]System.Runtime.CompilerServices.ExtensionAttribute::.ctor() = ( 01 00 00 00 )
		.custom instance void ['0SuperComicLib.Core']SuperComicLib.CodeContracts.AssumeInputsValidAttribute::.ctor() = ( 01 00 00 00 )

		.param [0]
		.custom instance void [mscorlib]System.Runtime.CompilerServices.IsReadOnlyAttribute::.ctor() = (01 00 00 00 )

		.param [1]
		.custom instance void [mscorlib]System.Runtime.CompilerServices.IsReadOnlyAttribute::.ctor() = (01 00 00 00 )
		.custom instance void ['0SuperComicLib.Core']SuperComicLib.CodeContracts.DisallowNullAttribute::.ctor() = ( 01 00 00 00 )
		.custom instance void ['0SuperComicLib.Core']SuperComicLib.CodeContracts.ValidRangeAttribute::.ctor() = ( 01 00 00 00 )
	
		.maxstack 8

		ldarg.0
		call !!1& modreq([mscorlib]System.Runtime.InteropServices.InAttribute) SuperComicLib.RuntimeMemoryMarshals.ILUnsafe::ConstAs<valuetype ['0SuperComicLib.Core']SuperComicLib.ConstMemory`1<!!T>, valuetype ['0SuperComicLib.Core']SuperComicLib.Memory`1<!!T>>(!!0&)
		call !!0& SuperComicLib.RuntimeMemoryMarshals.ArrayUnsafeGetElementReferenceExtension::refdata_dotnet<!!T>(valuetype ['0SuperComicLib.Core']SuperComicLib.Memory`1<!!0>&)
		ret
	}

	.method public hidebysig static !!T&  refdata_mono<T>(!!T[] 'array') cil managed aggressiveinlining
	{
		.custom instance void [mscorlib]System.Runtime.CompilerServices.ExtensionAttribute::.ctor() = ( 01 00 00 00 )
		.custom instance void ['0SuperComicLib.Core']SuperComicLib.CodeContracts.AssumeInputsValidAttribute::.ctor() = ( 01 00 00 00 )

		.param [1]
		.custom instance void ['0SuperComicLib.Core']SuperComicLib.CodeContracts.DisallowNullAttribute::.ctor() = ( 01 00 00 00 )
	
		.maxstack 8

		ldarg.0
		call !!0 SuperComicLib.RuntimeMemoryMarshals.ILUnsafe::icall__SystemRuntimeCompilerServicesUnsafeAs<class SuperComicLib.RuntimeMemoryMarshals.iffMono__asref`1<!!T>>(object)
		ldflda !0 class SuperComicLib.RuntimeMemoryMarshals.iffMono__asref`1<!!T>::item
		sizeof void*
		add
		ret
	}

	.method public hidebysig static !!T&  refdata_mono<T>(!!T[] 'array', native int index) cil managed aggressiveinlining
	{
		.custom instance void [mscorlib]System.Runtime.CompilerServices.ExtensionAttribute::.ctor() = ( 01 00 00 00 )
		.custom instance void ['0SuperComicLib.Core']SuperComicLib.CodeContracts.AssumeInputsValidAttribute::.ctor() = ( 01 00 00 00 )

		.param [1]
		.custom instance void ['0SuperComicLib.Core']SuperComicLib.CodeContracts.DisallowNullAttribute::.ctor() = ( 01 00 00 00 )

		.param [2]
		.custom instance void ['0SuperComicLib.Core']SuperComicLib.CodeContracts.ValidRangeAttribute::.ctor() = ( 01 00 00 00 )
	
		.maxstack 8

		ldarg.0
		call !!0 SuperComicLib.RuntimeMemoryMarshals.ILUnsafe::icall__SystemRuntimeCompilerServicesUnsafeAs<class SuperComicLib.RuntimeMemoryMarshals.iffMono__asref`1<!!T>>(object)
		ldflda !0 class SuperComicLib.RuntimeMemoryMarshals.iffMono__asref`1<!!T>::item
		ldarg.1
		sizeof !!T
		mul
		add
		sizeof void*
		add
		ret
	}

	.method public hidebysig static !!T&  refdata_mono<T>([in] valuetype ['0SuperComicLib.Core']SuperComicLib.Memory`1<!!T>& 'array') cil managed aggressiveinlining
	{
		.custom instance void [mscorlib]System.Runtime.CompilerServices.ExtensionAttribute::.ctor() = ( 01 00 00 00 )
		.custom instance void ['0SuperComicLib.Core']SuperComicLib.CodeContracts.AssumeInputsValidAttribute::.ctor() = ( 01 00 00 00 )

		.param [1]
		.custom instance void [mscorlib]System.Runtime.CompilerServices.IsReadOnlyAttribute::.ctor() = (01 00 00 00 )
		.custom instance void ['0SuperComicLib.Core']SuperComicLib.CodeContracts.DisallowNullAttribute::.ctor() = ( 01 00 00 00 )
		.custom instance void ['0SuperComicLib.Core']SuperComicLib.CodeContracts.ValidRangeAttribute::.ctor() = ( 01 00 00 00 )
		
		.maxstack 8

		ldarg.0
		ldfld !0[] valuetype ['0SuperComicLib.Core']SuperComicLib.Memory`1<!!T>::_source
		call !!0 SuperComicLib.RuntimeMemoryMarshals.ILUnsafe::icall__SystemRuntimeCompilerServicesUnsafeAs<class SuperComicLib.RuntimeMemoryMarshals.iffMono__asref`1<!!T>>(object)
		ldflda !0 class SuperComicLib.RuntimeMemoryMarshals.iffMono__asref`1<!!T>::item
		ldarg.0
		ldfld int32 valuetype ['0SuperComicLib.Core']SuperComicLib.Memory`1<!!T>::_start
		conv.u
		sizeof !!T
		mul
		conv.i
		add
		sizeof void*
		add
		ret
	}

	.method public hidebysig static !!T& modreq([mscorlib]System.Runtime.InteropServices.InAttribute) refdata_mono<T>([in] valuetype ['0SuperComicLib.Core']SuperComicLib.ConstMemory`1<!!T>& 'array') cil managed aggressiveinlining
	{
		.custom instance void [mscorlib]System.Runtime.CompilerServices.ExtensionAttribute::.ctor() = ( 01 00 00 00 )
		.custom instance void ['0SuperComicLib.Core']SuperComicLib.CodeContracts.AssumeInputsValidAttribute::.ctor() = ( 01 00 00 00 )

		.param [0]
		.custom instance void [mscorlib]System.Runtime.CompilerServices.IsReadOnlyAttribute::.ctor() = (01 00 00 00 )

		.param [1]
		.custom instance void [mscorlib]System.Runtime.CompilerServices.IsReadOnlyAttribute::.ctor() = (01 00 00 00 )
		.custom instance void ['0SuperComicLib.Core']SuperComicLib.CodeContracts.DisallowNullAttribute::.ctor() = ( 01 00 00 00 )
		.custom instance void ['0SuperComicLib.Core']SuperComicLib.CodeContracts.ValidRangeAttribute::.ctor() = ( 01 00 00 00 )
	
		.maxstack 8

		ldarg.0
		call !!1& modreq([mscorlib]System.Runtime.InteropServices.InAttribute) SuperComicLib.RuntimeMemoryMarshals.ILUnsafe::ConstAs<valuetype ['0SuperComicLib.Core']SuperComicLib.ConstMemory`1<!!T>, valuetype ['0SuperComicLib.Core']SuperComicLib.Memory`1<!!T>>(!!0&)
		call !!0& SuperComicLib.RuntimeMemoryMarshals.ArrayUnsafeGetElementReferenceExtension::refdata_mono<!!T>(valuetype ['0SuperComicLib.Core']SuperComicLib.Memory`1<!!0>&)
		ret
	}
} // end of 'ArrayUnsafeGetElementReferenceExtension'