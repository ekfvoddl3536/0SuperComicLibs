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


	// 
	//				+object, AsClass, AsPointer, GetDataRef, GetDataPtr
	// 
	.method public hidebysig static !!T  AsClass<class T>(void* source) cil managed aggressiveinlining
	{
		.maxstack 1
		.locals (
			[0] !!0
		)

		ldarg.0
		conv.i
		stloc.0
		ldloc.0
		ret
	}

	.method public hidebysig static !!T  AsClass<class T>(native int source) cil managed aggressiveinlining
	{
		.maxstack 1
		.locals (
			// NOTE::
			//		A variable that assists in producing code as concise as possible 
			//		when data containing pointer operations is input.
			//		
			//		byte* SOURCE_VALUE = ...;
			//		
			//		[CASE 1 | (without 'int8&')]
			//		x = (System.IntPtr)(SOURCE_VALUE)
			//		(JIT) -->
			//			mov rbx, rcx	; OK
			//
			//		[CASE 2 | (without 'int8&')]
			//		y = (System.IntPtr)(SOURCE_VALUE + 8)
			//		(JIT) -->
			//			lea rax, [rcx+8]
			//			mov rbx, rax
			//			mov rax, rbx
			//			mov rbx, rax	; What?
			//
			//		[CASE 3 | (with 'int8&')]
			//		z = (System.IntPtr)(SOURCE_VALUE + 8)
			//		(JIT) -->
			//			lea rax, [rcx+8]
			//			mov rbx, rax	; Okay, not bad. (best = 'lea rbx, [rcx+8]')
			[0] int8&,
			[1] !!0
		)

		// load 'native int' and store to 'int8&'
		ldarg.0
		stloc.0
		
		// load 'int8&' and store to '!!0'
		ldloc.0
		stloc.1

		// load '!!0' and return
		ldloc.1
		ret
	}

	.method public hidebysig static !!TTo  AsClass<TFrom, class TTo>(!!TFrom& source) cil managed aggressiveinlining
	{
		.maxstack 1
		.locals (
			[0] !!1
		)

		ldarg.0
		// Since '&' is treated as a managed pointer, 'conv.i' does not seem to be necessary.
		stloc.0
		ldloc.0
		ret
	}

	.method public hidebysig static void*  AsPointer(object reference) cil managed aggressiveinlining
	{
		.maxstack 1

		ldarg.0
		conv.i
		ret
	}

	.method public hidebysig static void*  AsPointer(object reference, int64 byteOffset) cil managed aggressiveinlining
	{
		.maxstack 2

		ldarg.0
		conv.i
		ldarg.1
		add
		ret
	}

	.method public hidebysig static !!TField&  GetDataRef<TField>(object reference) cil managed aggressiveinlining
	{
		.maxstack 1
		
		ldarg.0
		call !!0 SuperComicLib.RuntimeMemoryMarshals.ILUnsafe::As<class SuperComicLib.RuntimeMemoryMarshals.iff__asref`1<!!TField>>(object)
		ldflda !0 class SuperComicLib.RuntimeMemoryMarshals.iff__asref`1<!!TField>::item
		ret
	}

	.method public hidebysig static !!TField&  GetDataRef<TField>(object reference, int64 byteOffset) cil managed aggressiveinlining
	{
		.maxstack 2
		
		ldarg.0
		call !!0 SuperComicLib.RuntimeMemoryMarshals.ILUnsafe::As<class SuperComicLib.RuntimeMemoryMarshals.iff__asref`1<!!TField>>(object)
		ldflda !0 class SuperComicLib.RuntimeMemoryMarshals.iff__asref`1<!!TField>::item
		ldarg.1
		add
		ret
	}

	.method public hidebysig static !!TField*  GetDataPtr<valuetype .ctor (class [mscorlib]System.ValueType modreq([mscorlib]System.Runtime.InteropServices.UnmanagedType)) TField>(object reference) cil managed aggressiveinlining
	{
		.param type TField
        .custom instance void System.Runtime.CompilerServices.IsUnmanagedAttribute::.ctor() = ( 01 00 00 00 )

		.maxstack 1
		
		ldarg.0
		call !!0 SuperComicLib.RuntimeMemoryMarshals.ILUnsafe::As<class SuperComicLib.RuntimeMemoryMarshals.iff__asref`1<!!TField>>(object)
		ldflda !0 class SuperComicLib.RuntimeMemoryMarshals.iff__asref`1<!!TField>::item
		conv.u
		ret
	}

	.method public hidebysig static !!TField*  GetDataPtr<valuetype .ctor (class [mscorlib]System.ValueType modreq([mscorlib]System.Runtime.InteropServices.UnmanagedType)) TField>(object reference, int64 byteOffset) cil managed aggressiveinlining
	{
		.param type TField
        .custom instance void System.Runtime.CompilerServices.IsUnmanagedAttribute::.ctor() = ( 01 00 00 00 )

		.maxstack 2
		
		ldarg.0
		call !!0 SuperComicLib.RuntimeMemoryMarshals.ILUnsafe::As<class SuperComicLib.RuntimeMemoryMarshals.iff__asref`1<!!TField>>(object)
		ldflda !0 class SuperComicLib.RuntimeMemoryMarshals.iff__asref`1<!!TField>::item
		ldarg.1
		add
		conv.u
		ret
	}




	// 
	//				'readonly', As, Add, Subtract
	// 
	.method public hidebysig static !!TTo& modreq([mscorlib]System.Runtime.InteropServices.InAttribute) ReadOnlyAs<TFrom, TTo>([in] !!TFrom& source) cil managed aggressiveinlining
	{
		.param [0] 
		.custom instance void [mscorlib]System.Runtime.CompilerServices.IsReadOnlyAttribute::.ctor() = ( 01 00 00 00 )

        .param [1]
		.custom instance void [mscorlib]System.Runtime.CompilerServices.IsReadOnlyAttribute::.ctor() = ( 01 00 00 00 )

		.maxstack 1
		
		ldarg.0
		ret
	}


	//											Add
	.method public hidebysig static !!T& modreq([mscorlib]System.Runtime.InteropServices.InAttribute) ReadOnlyAdd<T>(
			[in] !!T& source, int32 offset) cil managed aggressiveinlining
	{
		.param [0] 
		.custom instance void [mscorlib]System.Runtime.CompilerServices.IsReadOnlyAttribute::.ctor() = ( 01 00 00 00 )

        .param [1]
		.custom instance void [mscorlib]System.Runtime.CompilerServices.IsReadOnlyAttribute::.ctor() = ( 01 00 00 00 )

		.maxstack 3
		
		ldarg.0
		ldarg.1
		sizeof !!0
		conv.i
		mul
		add
		ret
	}

	.method public hidebysig static !!T& modreq([mscorlib]System.Runtime.InteropServices.InAttribute) ReadOnlyAdd<T>(
			[in] !!T& source, int64 offset) cil managed aggressiveinlining
	{
		.param [0] 
		.custom instance void [mscorlib]System.Runtime.CompilerServices.IsReadOnlyAttribute::.ctor() = ( 01 00 00 00 )

        .param [1]
		.custom instance void [mscorlib]System.Runtime.CompilerServices.IsReadOnlyAttribute::.ctor() = ( 01 00 00 00 )

		.maxstack 3
		
		ldarg.0
		ldarg.1
		sizeof !!0
		conv.i
		mul
		add
		ret
	}

	.method public hidebysig static !!TTo& modreq([mscorlib]System.Runtime.InteropServices.InAttribute)  ReadOnlyAdd<TFrom, TTo>(
			[in] !!TFrom& source, int64 offset) cil managed aggressiveinlining
	{
		.param [0] 
		.custom instance void [mscorlib]System.Runtime.CompilerServices.IsReadOnlyAttribute::.ctor() = ( 01 00 00 00 )

        .param [1]
		.custom instance void [mscorlib]System.Runtime.CompilerServices.IsReadOnlyAttribute::.ctor() = ( 01 00 00 00 )

		ldarg.0
		ldarg.1
		sizeof !!0
		conv.i
		mul
		add
		ret
	}

	.method public hidebysig static !!TTo& modreq([mscorlib]System.Runtime.InteropServices.InAttribute)  ReadOnlyAdd<TFrom, TTo>(
			[in] !!TFrom& source, int64 offset, int32 displacement) cil managed aggressiveinlining
	{
		.param [0] 
		.custom instance void [mscorlib]System.Runtime.CompilerServices.IsReadOnlyAttribute::.ctor() = ( 01 00 00 00 )

        .param [1]
		.custom instance void [mscorlib]System.Runtime.CompilerServices.IsReadOnlyAttribute::.ctor() = ( 01 00 00 00 )

		ldarg.0
		ldarg.1
		sizeof !!0
		conv.i
		mul
		add
		ldarg.2
		add
		ret
	}

	.method public hidebysig static !!TTo& modreq([mscorlib]System.Runtime.InteropServices.InAttribute)  ReadOnlyAdd<TFrom, TSize, TTo>(
			[in] !!TFrom& source, int64 offset) cil managed aggressiveinlining
	{
		.param [0] 
		.custom instance void [mscorlib]System.Runtime.CompilerServices.IsReadOnlyAttribute::.ctor() = ( 01 00 00 00 )

        .param [1]
		.custom instance void [mscorlib]System.Runtime.CompilerServices.IsReadOnlyAttribute::.ctor() = ( 01 00 00 00 )

		ldarg.0
		ldarg.1
		sizeof !!1
		conv.i
		mul
		add
		ret
	}

	.method public hidebysig static !!TTo& modreq([mscorlib]System.Runtime.InteropServices.InAttribute)  ReadOnlyAdd<TFrom, TSize, TTo>(
			[in] !!TFrom& source, int64 offset, int32 displacement) cil managed aggressiveinlining
	{
		.param [0] 
		.custom instance void [mscorlib]System.Runtime.CompilerServices.IsReadOnlyAttribute::.ctor() = ( 01 00 00 00 )

        .param [1]
		.custom instance void [mscorlib]System.Runtime.CompilerServices.IsReadOnlyAttribute::.ctor() = ( 01 00 00 00 )

		ldarg.0
		ldarg.1
		sizeof !!1
		conv.i
		mul
		add
		ldarg.2
		add
		ret
	}


	//											Subtract
	.method public hidebysig static !!T& modreq([mscorlib]System.Runtime.InteropServices.InAttribute) ReadOnlySubtract<T>(
			[in] !!T& source, int32 offset) cil managed aggressiveinlining
	{
		.param [0] 
		.custom instance void [mscorlib]System.Runtime.CompilerServices.IsReadOnlyAttribute::.ctor() = ( 01 00 00 00 )

        .param [1]
		.custom instance void [mscorlib]System.Runtime.CompilerServices.IsReadOnlyAttribute::.ctor() = ( 01 00 00 00 )

		.maxstack 3
		
		ldarg.0
		ldarg.1
		sizeof !!0
		conv.i
		mul
		sub
		ret
	}

	.method public hidebysig static !!T& modreq([mscorlib]System.Runtime.InteropServices.InAttribute) ReadOnlySubtract<T>(
			[in] !!T& source, int64 offset) cil managed aggressiveinlining
	{
		.param [0] 
		.custom instance void [mscorlib]System.Runtime.CompilerServices.IsReadOnlyAttribute::.ctor() = ( 01 00 00 00 )

        .param [1]
		.custom instance void [mscorlib]System.Runtime.CompilerServices.IsReadOnlyAttribute::.ctor() = ( 01 00 00 00 )

		.maxstack 3
		
		ldarg.0
		ldarg.1
		sizeof !!0
		conv.i
		mul
		sub
		ret
	}

	.method public hidebysig static !!TTo& modreq([mscorlib]System.Runtime.InteropServices.InAttribute)  ReadOnlySubtract<TFrom, TTo>(
			[in] !!TFrom& source, int64 offset) cil managed aggressiveinlining
	{
		.param [0] 
		.custom instance void [mscorlib]System.Runtime.CompilerServices.IsReadOnlyAttribute::.ctor() = ( 01 00 00 00 )

        .param [1]
		.custom instance void [mscorlib]System.Runtime.CompilerServices.IsReadOnlyAttribute::.ctor() = ( 01 00 00 00 )

		ldarg.0
		ldarg.1
		sizeof !!0
		conv.i
		mul
		sub
		ret
	}

	.method public hidebysig static !!TTo& modreq([mscorlib]System.Runtime.InteropServices.InAttribute)  ReadOnlySubtract<TFrom, TSize, TTo>(
			[in] !!TFrom& source, int64 offset) cil managed aggressiveinlining
	{
		.param [0] 
		.custom instance void [mscorlib]System.Runtime.CompilerServices.IsReadOnlyAttribute::.ctor() = ( 01 00 00 00 )

        .param [1]
		.custom instance void [mscorlib]System.Runtime.CompilerServices.IsReadOnlyAttribute::.ctor() = ( 01 00 00 00 )

		ldarg.0
		ldarg.1
		sizeof !!1
		conv.i
		mul
		sub
		ret
	}




	// 
	//				Group 2. (CUSTOM, void*, bool->int)
	// 
	.method public hidebysig static !!T&  AddByteOffset<T>(void* source, int64 byteOffset) cil managed aggressiveinlining
	{
		.maxstack 2

		ldarg.0
		ldarg.1
		conv.u
		add
		call !!0& SuperComicLib.RuntimeMemoryMarshals.ILUnsafe::AsRef<!!T>(void*)
		ret
	}


	//											Add
	.method public hidebysig static !!T&  Add<T>(void* source, int32 offset, int32 displacement) cil managed aggressiveinlining
	{
		.maxstack 3
		
		ldarg.0
		ldarg.1
		sizeof !!0
		conv.u
		mul
		add
		ldarg.2
		add
		call !!0& SuperComicLib.RuntimeMemoryMarshals.ILUnsafe::AsRef<!!T>(void*)
		ret
	}

	.method public hidebysig static !!T&  Add<T>(void* source, int64 offset, int32 displacement) cil managed aggressiveinlining
	{
		.maxstack 3
		
		ldarg.0
		ldarg.1
		sizeof !!0
		conv.u
		mul
		add
		ldarg.2
		add
		call !!0& SuperComicLib.RuntimeMemoryMarshals.ILUnsafe::AsRef<!!T>(void*)
		ret
	}

	.method public hidebysig static !!T&  Add<T>(void* source, int32 offset) cil managed aggressiveinlining
	{
		.maxstack 3
		
		ldarg.0
		ldarg.1
		sizeof !!0
		conv.u
		mul
		add
		call !!0& SuperComicLib.RuntimeMemoryMarshals.ILUnsafe::AsRef<!!T>(void*)
		ret
	}

	.method public hidebysig static !!T&  Add<T>(void* source, int64 offset) cil managed aggressiveinlining
	{
		.maxstack 3
		
		ldarg.0
		ldarg.1
		sizeof !!0
		conv.u
		mul
		add
		call !!0& SuperComicLib.RuntimeMemoryMarshals.ILUnsafe::AsRef<!!T>(void*)
		ret
	}


	//											Subtract
	.method public hidebysig static !!T&  Subtract<T>(void* source, int32 offset) cil managed aggressiveinlining
	{
		.maxstack 3
		
		ldarg.0
		ldarg.1
		sizeof !!0
		conv.u
		mul
		sub
		call !!0& SuperComicLib.RuntimeMemoryMarshals.ILUnsafe::AsRef<!!T>(void*)
		ret
	}

	.method public hidebysig static !!T&  Subtract<T>(void* source, int64 offset) cil managed aggressiveinlining
	{
		.maxstack 3
		
		ldarg.0
		ldarg.1
		sizeof !!0
		conv.u
		mul
		sub
		call !!0& SuperComicLib.RuntimeMemoryMarshals.ILUnsafe::AsRef<!!T>(void*)
		ret
	}

	.method public hidebysig static int32  ConvI4(bool 'value') cil managed aggressiveinlining
	{
		.maxstack 1

		ldarg.0
		ret
	}




	// 
	//				Group 3. (CUSTOM, +Increment, +Decrement)
	// 
	.method public hidebysig static !!T&  Increment<T>(!!T& source) cil managed aggressiveinlining
	{
		.maxstack 2

		ldarg.0
		sizeof !!0
		add
		ret
	}

	.method public hidebysig static !!TFrom&  Increment<TFrom, TSize>(!!TFrom& source) cil managed aggressiveinlining
	{
		.maxstack 2

		ldarg.0
		sizeof !!1
		add
		ret
	}

	.method public hidebysig static !!TTo&  Increment<TFrom, TSize, TTo>(!!TFrom& source) cil managed aggressiveinlining
	{
		.maxstack 2

		ldarg.0
		sizeof !!1
		add
		ret
	}

	.method public hidebysig static !!T&  Decrement<T>(!!T& source) cil managed aggressiveinlining
	{
		.maxstack 2

		ldarg.0
		sizeof !!0
		sub
		ret
	}

	.method public hidebysig static !!TFrom&  Decrement<TFrom, TSize>(!!TFrom& source) cil managed aggressiveinlining
	{
		.maxstack 2

		ldarg.0
		sizeof !!1
		sub
		ret
	}

	.method public hidebysig static !!TTo&  Decrement<TFrom, TSize, TTo>(!!TFrom& source) cil managed aggressiveinlining
	{
		.maxstack 2

		ldarg.0
		sizeof !!1
		sub
		ret
	}




	// 
	//				Group 4. (CUSTOM, +refpoint<>)
	// 
	.method public hidebysig static valuetype SuperComicLib.RuntimeMemoryMarshals.Refpoint`1<!!T>&  AsRefpoint<T>(void* source) cil managed aggressiveinlining
	{
		.maxstack 1

		ldarg.0
		conv.i
		ret
	}

	.method public hidebysig static valuetype SuperComicLib.RuntimeMemoryMarshals.Refpoint`1<!!T>&  AsRefpoint<T>(!!T& source) cil managed aggressiveinlining
	{
		.maxstack 1

		ldarg.0
		ret
	}

	.method public hidebysig static valuetype SuperComicLib.RuntimeMemoryMarshals.Refpoint`1<!!TTo>&  AsRefpoint<TFrom, TTo>(!!TFrom& source) cil managed aggressiveinlining
	{
		.maxstack 1

		ldarg.0
		ret
	}




	// 
	//				Group 5. (EXTEND, +Add, +Subtract, +AsRef)
	// 
	//											Add
	.method public hidebysig static !!TTo&  Add<TFrom, TTo>(!!TFrom& source, int64 offset) cil managed aggressiveinlining
	{
		.maxstack 3

		ldarg.0
		ldarg.1
		sizeof !!0
		conv.i
		mul
		add
		ret
	}

	.method public hidebysig static !!TTo&  Add<TFrom, TTo>(!!TFrom& source, int64 offset, int32 displacement) cil managed aggressiveinlining
	{
		.maxstack 3

		ldarg.0
		ldarg.1
		sizeof !!0
		conv.i
		mul
		add
		ldarg.2
		add
		ret
	}

	.method public hidebysig static !!TTo&  Add<TFrom, TSize, TTo>(!!TFrom& source, int64 offset) cil managed aggressiveinlining
	{
		.maxstack 3

		ldarg.0
		ldarg.1
		sizeof !!1
		conv.i
		mul
		add
		ret
	}

	.method public hidebysig static !!TTo&  Add<TFrom, TSize, TTo>(!!TFrom& source, int64 offset, int32 displacement) cil managed aggressiveinlining
	{
		.maxstack 3

		ldarg.0
		ldarg.1
		sizeof !!1
		conv.i
		mul
		add
		ldarg.2
		add
		ret
	}

	.method public hidebysig static !!T&  Add<T>(!!T& source, uint64 offset) cil managed aggressiveinlining
	{
		.maxstack 3

		ldarg.0
		ldarg.1
		sizeof !!0
		conv.i
		mul
		add
		ret
	}

	.method public hidebysig static !!T&  Add<T>(!!T& source, uint64 offset, int32 displacement) cil managed aggressiveinlining
	{
		.maxstack 3

		ldarg.0
		ldarg.1
		sizeof !!0
		conv.i
		mul
		add
		ldarg.2
		add
		ret
	}

	.method public hidebysig static !!T&  Add<T>(!!T& source, int64 offset, int32 displacement) cil managed aggressiveinlining
	{
		.maxstack 3

		ldarg.0
		ldarg.1
		sizeof !!0
		conv.i
		mul
		add
		ldarg.2
		add
		ret
	}


	//											Subtract
	.method public hidebysig static !!TTo&  Subtract<TFrom, TTo>(!!TFrom& source, int64 offset) cil managed aggressiveinlining
	{
		.maxstack 3

		ldarg.0
		ldarg.1
		sizeof !!0
		conv.i
		mul
		sub
		ret
	}

	.method public hidebysig static !!TTo&  Subtract<TFrom, TSize, TTo>(!!TFrom& source, int64 offset) cil managed aggressiveinlining
	{
		.maxstack 3

		ldarg.0
		ldarg.1
		sizeof !!1
		conv.i
		mul
		sub
		ret
	}


	//											AsRef
	.method public hidebysig static !!TTo&  AsRef<TFrom, TTo>([in] !!TFrom& source) cil managed aggressiveinlining
	{
		.param [1]
		.custom instance void [mscorlib]System.Runtime.CompilerServices.IsReadOnlyAttribute::.ctor() = ( 01 00 00 00 ) 

		.maxstack 1

		ldarg.0
		ret
	}
	




	// refer: https://github.com/dotnet/corert/blob/master/src/System.Private.CoreLib/shared/Internal/Runtime/CompilerServices/Unsafe.cs
	//
	// !================== System.Runtime.CompilerServices.Unsafe (features) ==================!
	// 
	.method public hidebysig static void*  AsPointer<T>(!!T& source) cil managed aggressiveinlining
	{
		.maxstack 1

		ldarg.0
		conv.u
		ret
	}

	.method public hidebysig static int32  SizeOf<T>() cil managed aggressiveinlining
	{
		.maxstack 1

		sizeof !!0
		ret
	}

	.method public hidebysig static !!T  As<class T>(object o) cil managed aggressiveinlining
	{
		.maxstack 1

		ldarg.0
		ret
	}

	.method public hidebysig static !!TTo& As<TFrom,TTo>(!!TFrom& source) cil managed aggressiveinlining
	{
		.maxstack 1

		ldarg.0
		ret
	}

	.method public hidebysig static !!T&  Add<T>(!!T& source, int32 elementOffset) cil managed aggressiveinlining
	{
		.maxstack 3

		ldarg.0
		ldarg.1
		sizeof !!0
		conv.i
		mul
		add
		ret
	}

	.method public hidebysig static !!T&  Add<T>(!!T& source, int64 elementOffset) cil managed aggressiveinlining
	{
		.maxstack 3

		ldarg.0
		ldarg.1
		sizeof !!0
		mul
		add
		ret
	}

	.method public hidebysig static bool  AreSame<T>([in] !!T& left, [in] !!T& right) cil managed aggressiveinlining
	{
		.param [1]
		.custom instance void [mscorlib]System.Runtime.CompilerServices.IsReadOnlyAttribute::.ctor() = ( 01 00 00 00 ) 

		.param [2]
		.custom instance void [mscorlib]System.Runtime.CompilerServices.IsReadOnlyAttribute::.ctor() = ( 01 00 00 00 ) 

		.maxstack 2

		ldarg.0
		ldarg.1
		ceq
		ret
	}

	.method public hidebysig static int64  ByteOffset<T>([in] !!T& origin, [in] !!T& target) cil managed aggressiveinlining
	{
		.param [1]
		.custom instance void [mscorlib]System.Runtime.CompilerServices.IsReadOnlyAttribute::.ctor() = ( 01 00 00 00 ) 

		.param [2]
		.custom instance void [mscorlib]System.Runtime.CompilerServices.IsReadOnlyAttribute::.ctor() = ( 01 00 00 00 ) 

		.maxstack 2

		ldarg.0
		ldarg.1
		sub
		conv.i8
		ret
	}

	.method public hidebysig static bool  IsAddressGreaterThan<T>([in] !!T& left, [in] !!T& right) cil managed aggressiveinlining
	{
		.param [1]
		.custom instance void [mscorlib]System.Runtime.CompilerServices.IsReadOnlyAttribute::.ctor() = ( 01 00 00 00 ) 

		.param [2]
		.custom instance void [mscorlib]System.Runtime.CompilerServices.IsReadOnlyAttribute::.ctor() = ( 01 00 00 00 ) 

		.maxstack 2

		ldarg.0
		ldarg.1
		cgt.un
		ret
	}

	.method public hidebysig static bool  IsAddressLessThan<T>([in] !!T& left, [in] !!T& right) cil managed aggressiveinlining
	{
		.param [1]
		.custom instance void [mscorlib]System.Runtime.CompilerServices.IsReadOnlyAttribute::.ctor() = ( 01 00 00 00 ) 

		.param [2]
		.custom instance void [mscorlib]System.Runtime.CompilerServices.IsReadOnlyAttribute::.ctor() = ( 01 00 00 00 ) 

		.maxstack 2

		ldarg.0
		ldarg.1
		clt.un
		ret
	}

	.method public hidebysig static void  Copy<T>(void* destination, [in] !!T& source) cil managed aggressiveinlining
	{
		.param [2]
		.custom instance void [mscorlib]System.Runtime.CompilerServices.IsReadOnlyAttribute::.ctor() = ( 01 00 00 00 ) 

		.maxstack 2

		ldarg.0
		ldarg.1
		ldobj !!0
		stobj !!0
		ret
	}

	.method public hidebysig static void  Copy<T>(!!T& destination, void* source) cil managed aggressiveinlining
	{
		.maxstack 2

		ldarg.0
		ldarg.1
		ldobj !!0
		stobj !!0
		ret
	}

	.method public hidebysig static void  CopyBlock(void* destination, void* source, uint32 byteCount) cil managed aggressiveinlining
	{
		.maxstack 3

		ldarg.0
		ldarg.1
		ldarg.2
		cpblk
		ret
	}

	.method public hidebysig static void  CopyBlock(uint8& destination, [in] uint8& source, uint32 byteCount) cil managed aggressiveinlining
	{
		.param [2]
		.custom instance void [mscorlib]System.Runtime.CompilerServices.IsReadOnlyAttribute::.ctor() = ( 01 00 00 00 ) 

		.maxstack 3

		ldarg.0
		ldarg.1
		ldarg.2
		cpblk
		ret
	}

	.method public hidebysig static void  CopyBlockUnaligned(void* destination, void* source, uint32 byteCount) cil managed aggressiveinlining
	{
		.maxstack 4

		ldarg.0
        ldarg.1
        ldarg.2
        unaligned. 0x1
        cpblk
        ret
	}

	.method public hidebysig static void  CopyBlockUnaligned(uint8& destination, [in] uint8& source, uint32 byteCount) cil managed aggressiveinlining
	{
		.param [2]
		.custom instance void [mscorlib]System.Runtime.CompilerServices.IsReadOnlyAttribute::.ctor() = ( 01 00 00 00 )

		.maxstack 4

		ldarg.0
        ldarg.1
        ldarg.2
        unaligned. 0x1
        cpblk
        ret
	}

	.method public hidebysig static void  InitBlock(void* startAddress, uint8 'value', uint32 byteCount) cil managed aggressiveinlining
	{
		.maxstack 3

		ldarg.0
		ldarg.1
		ldarg.2
		initblk
		ret
	}

	.method public hidebysig static void  InitBlock(uint8& startAddress, uint8 'value', uint32 byteCount) cil managed aggressiveinlining
	{
		.maxstack 3

		ldarg.0
		ldarg.1
		ldarg.2
		initblk
		ret
	}

	.method public hidebysig static void  InitBlockUnaligned(void* startAddress, uint8 'value', uint32 byteCount) cil managed aggressiveinlining
	{
		.maxstack 4

		ldarg.0
		ldarg.1
		ldarg.2
		unaligned. 0x1
		initblk
		ret
	}

	.method public hidebysig static void  InitBlockUnaligned(uint8& startAddress, uint8 'value', uint32 byteCount) cil managed aggressiveinlining
	{
		.maxstack 4

		ldarg.0
		ldarg.1
		ldarg.2
		unaligned. 0x1
		initblk
		ret
	}

	.method public hidebysig static !!T  ReadUnaligned<T>(void* source) cil managed aggressiveinlining
	{
		.maxstack 4

		ldarg.0
		unaligned. 0x1
		ldobj !!0
		ret
	}

	.method public hidebysig static !!T  ReadUnaligned<T>([in] uint8& source) cil managed aggressiveinlining
	{
		.param [1]
		.custom instance void [mscorlib]System.Runtime.CompilerServices.IsReadOnlyAttribute::.ctor() = ( 01 00 00 00 ) 

		.maxstack 4

		ldarg.0
		unaligned. 0x1
		ldobj !!0
		ret
	}

	.method public hidebysig static void  WriteUnaligned<T>(void* destination, !!T 'value') cil managed aggressiveinlining
	{
		.maxstack 4

		ldarg.0
		ldarg.1
		unaligned. 0x1
		stobj !!0
		ret
	}

	.method public hidebysig static void  WriteUnaligned<T>(uint8& destination, !!T 'value') cil managed aggressiveinlining
	{
		.maxstack 4

		ldarg.0
		ldarg.1
		unaligned. 0x1
		stobj !!0
		ret
	}

	.method public hidebysig static !!T&  AddByteOffset<T>(!!T& source, int32 byteOffset) cil managed aggressiveinlining
	{
		.maxstack 2

		ldarg.0
		ldarg.1
		conv.i
		add
		ret
	}

	.method public hidebysig static !!T&  AddByteOffset<T>(!!T& source, int64 byteOffset) cil managed aggressiveinlining
	{
		.maxstack 2

		ldarg.0
		ldarg.1
		add
		ret
	}

	.method public hidebysig static !!T&  AsRef<T>(void* source) cil managed aggressiveinlining
	{
		.maxstack 1

		ldarg.0
		call !!1& SuperComicLib.RuntimeMemoryMarshals.ILUnsafe::As<int8, !!T>(!!0&)
		ret
	}

	.method public hidebysig static !!T&  AsRef<T>([in] !!T& source) cil managed aggressiveinlining
	{
		.param [1]
		.custom instance void [mscorlib]System.Runtime.CompilerServices.IsReadOnlyAttribute::.ctor() = ( 01 00 00 00 ) 

		.maxstack 1

		ldarg.0
		ret
	}

	.method public hidebysig static !!T  Read<T>(void* source) cil managed aggressiveinlining
	{
		.maxstack 2

		ldarg.0
		ldobj !!0
		ret
	}

	.method public hidebysig static void  Write<T>(void* destination, !!T 'value') cil managed aggressiveinlining
	{
		.maxstack 4

		ldarg.0
		ldarg.1
		stobj !!0
		ret
	}

	.method public hidebysig static !!T&  NullRef<T>() cil managed aggressiveinlining
	{
		.maxstack 1

		ldc.i4.0
		conv.u
		ret
	}

	.method public hidebysig static bool  IsNullRef<T>([in] !!T& source) cil managed aggressiveinlining
	{
		.param [1]
		.custom instance void [mscorlib]System.Runtime.CompilerServices.IsReadOnlyAttribute::.ctor() = ( 01 00 00 00 ) 

		.maxstack 2

		ldarg.0
		ldc.i4.0
		conv.u
		ceq
		ret
	}

	.method public hidebysig static void  SkipInit<T>([out] !!T& source) cil managed aggressiveinlining
	{
		.maxstack 0

		ret
	}

	.method public hidebysig static !!T&  Subtract<T>(!!T& source, int32 elementOffset) cil managed aggressiveinlining
	{
		.maxstack 4

		ldarg.0
		ldarg.1
		sizeof !!0
		conv.i
		mul
		sub
		ret
	}

	.method public hidebysig static !!T&  Subtract<T>(!!T& source, int64 elementOffset) cil managed aggressiveinlining
	{
		.maxstack 4

		ldarg.0
		ldarg.1
		sizeof !!0
		mul
		sub
		ret
	}

	.method public hidebysig static !!T&  SubtractByteOffset<T>(!!T& source, int64 byteOffset) cil managed aggressiveinlining
	{
		.maxstack 2

		ldarg.0
		ldarg.1
		sub
		ret
	}

	.method public hidebysig static !!T&  Unbox<valuetype .ctor ([mscorlib]System.ValueType) T>(object 'box') cil managed aggressiveinlining
	{
		.maxstack 2

		ldarg.0
		unbox !!0
		ret
	}
} // end of 'ILUnsafe'