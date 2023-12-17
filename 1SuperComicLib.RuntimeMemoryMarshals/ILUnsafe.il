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
			[0] !!0
		)

		ldarg.0
		stloc.0
		ldloc.0
		ret
	}

	.method public hidebysig static void*  AsPointer(object source) cil managed aggressiveinlining
	{
		.maxstack 1

		ldarg.0
		conv.i
		ret
	}

	.method public hidebysig static void*  AsPointer(object source, int64 byteOffset) cil managed aggressiveinlining
	{
		.maxstack 2

		ldarg.0
		conv.i
		ldarg.1
		add
		ret
	}

	.method public hidebysig static !!TField&  GetDataRef<TField>(object class_references) cil managed aggressiveinlining
	{
		.maxstack 1
		
		ldarg.0
		call !!0 SuperComicLib.RuntimeMemoryMarshals.ILUnsafe::As<class SuperComicLib.RuntimeMemoryMarshals.iff__asref`1<!!TField>>(object)
		ldflda !0 class SuperComicLib.RuntimeMemoryMarshals.iff__asref`1<!!TField>::item
		ret
	}

	.method public hidebysig static !!TField&  GetDataRef<TField>(object class_references, int64 byteOffset) cil managed aggressiveinlining
	{
		.maxstack 2
		
		ldarg.0
		call !!0 SuperComicLib.RuntimeMemoryMarshals.ILUnsafe::As<class SuperComicLib.RuntimeMemoryMarshals.iff__asref`1<!!TField>>(object)
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
		call !!0 SuperComicLib.RuntimeMemoryMarshals.ILUnsafe::As<class SuperComicLib.RuntimeMemoryMarshals.iff__asref`1<!!TField>>(object)
		ldflda !0 class SuperComicLib.RuntimeMemoryMarshals.iff__asref`1<!!TField>::item
		conv.u
		ret
	}

	.method public hidebysig static !!TField*  GetDataPtr<valuetype .ctor (class [mscorlib]System.ValueType modreq([mscorlib]System.Runtime.InteropServices.UnmanagedType)) TField>(object class_references, int64 byteOffset) cil managed aggressiveinlining
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

	.method public hidebysig static !!T&  AddByteOffset<T>(void* source, int32 byteOffset) cil managed aggressiveinlining
	{
		.maxstack 2

		ldarg.0
		ldarg.1
		conv.i
		add
		call !!1& SuperComicLib.RuntimeMemoryMarshals.ILUnsafe::As<int8, !!T>(!!0&)
		ret
	}

	.method public hidebysig static !!T&  Add<T>(void* source, int32 elementOffset, int32 addByteOffset) cil managed aggressiveinlining
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
		call !!1& SuperComicLib.RuntimeMemoryMarshals.ILUnsafe::As<int8, !!T>(!!0&)
		ret
	}

	.method public hidebysig static !!T&  Add<T>(void* source, int64 elementOffset, int32 addByteOffset) cil managed aggressiveinlining
	{
		.maxstack 3
		
		ldarg.0
		ldarg.1
		sizeof !!0
		mul
		add
		ldarg.2
		add
		call !!1& SuperComicLib.RuntimeMemoryMarshals.ILUnsafe::As<int8, !!T>(!!0&)
		ret
	}

	.method public hidebysig static !!T&  Add<T>(void* source, int32 elementOffset) cil managed aggressiveinlining
	{
		.maxstack 3
		
		ldarg.0
		ldarg.1
		sizeof !!0
		conv.i
		mul
		add
		call !!1& SuperComicLib.RuntimeMemoryMarshals.ILUnsafe::As<int8, !!T>(!!0&)
		ret
	}

	.method public hidebysig static !!T&  Add<T>(void* source, int64 elementOffset) cil managed aggressiveinlining
	{
		.maxstack 3
		
		ldarg.0
		ldarg.1
		sizeof !!0
		mul
		add
		call !!1& SuperComicLib.RuntimeMemoryMarshals.ILUnsafe::As<int8, !!T>(!!0&)
		ret
	}

	.method public hidebysig static !!T& modreq([mscorlib]System.Runtime.InteropServices.InAttribute) ReadOnlyAdd<T>([in] !!T& source, int32 elementOffset) cil managed aggressiveinlining
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

	.method public hidebysig static !!T& modreq([mscorlib]System.Runtime.InteropServices.InAttribute) ReadOnlyAdd<T>([in] !!T& source, int64 elementOffset) cil managed aggressiveinlining
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
		ret
	}

	.method public hidebysig static !!T&  Increment<T>(!!T& source) cil managed aggressiveinlining
	{
		.maxstack 2

		ldarg.0
		sizeof !!0
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

	.method public hidebysig static !!T&  Subtract<T>(void* source, int32 elementOffset) cil managed aggressiveinlining
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

	.method public hidebysig static !!T&  Subtract<T>(void* source, int64 elementOffset) cil managed aggressiveinlining
	{
		.maxstack 4

		ldarg.0
		ldarg.1
		sizeof !!0
		mul
		sub
		ret
	}

	.method public hidebysig static valuetype SuperComicLib.RuntimeMemoryMarshals.refpoint`1<!!T>&  AsRefpoint<T>(!!T& source) cil managed aggressiveinlining
	{
		.maxstack 1

		ldarg.0
		ret
	}

	.method public hidebysig static valuetype SuperComicLib.RuntimeMemoryMarshals.refpoint`1<!!TTo>&  AsRefpoint<TFrom, TTo>(!!TFrom& source) cil managed aggressiveinlining
	{
		.maxstack 1

		ldarg.0
		ret
	}

	.method public hidebysig static valuetype SuperComicLib.RuntimeMemoryMarshals.refpoint`1<!!TTo>&  AsRefpoint<TFrom, TTo>(!!TFrom& source, int64 offset) cil managed aggressiveinlining
	{
		.maxstack 3

		ldarg.0
		ldarg.1
		conv.i
		sizeof !!0
		mul
		add
		ret
	}

	.method public hidebysig static valuetype SuperComicLib.RuntimeMemoryMarshals.refpoint`1<!!TTo>&  AsRefpoint<TFrom, TSize, TTo>(!!TFrom& source, int64 offset) cil managed aggressiveinlining
	{
		.maxstack 3

		ldarg.0
		ldarg.1
		conv.i
		sizeof !!1
		mul
		add
		ret
	}

	.method public hidebysig static valuetype SuperComicLib.RuntimeMemoryMarshals.refpoint`1<!!TTo>&  AsRefpoint<TFrom, TSize, TTo>(!!TFrom& source, int64 offset, int32 displacement) cil managed aggressiveinlining
	{
		.maxstack 3

		ldarg.0
		ldarg.1
		conv.i
		sizeof !!1
		mul
		add
		ldarg.2
		add
		ret
	}

	.method public hidebysig static !!TTo&  Add<TFrom, TTo>(!!TFrom& source, int64 offset) cil managed aggressiveinlining
	{
		.maxstack 3

		ldarg.0
		ldarg.1
		conv.i
		sizeof !!0
		mul
		add
		ret
	}

	.method public hidebysig static !!TTo&  Add<TFrom, TSize, TTo>(!!TFrom& source, int64 offset) cil managed aggressiveinlining
	{
		.maxstack 3

		ldarg.0
		ldarg.1
		conv.i
		sizeof !!1
		mul
		add
		ret
	}

	.method public hidebysig static !!TTo&  Subtract<TFrom, TTo>(!!TFrom& source, int64 offset) cil managed aggressiveinlining
	{
		.maxstack 3

		ldarg.0
		ldarg.1
		conv.i
		sizeof !!0
		mul
		sub
		ret
	}

	.method public hidebysig static !!TTo&  Subtract<TFrom, TSize, TTo>(!!TFrom& source, int64 offset) cil managed aggressiveinlining
	{
		.maxstack 3

		ldarg.0
		ldarg.1
		conv.i
		sizeof !!1
		mul
		sub
		ret
	}

	.method public hidebysig static !!TTo&  Add<TFrom, TSize, TTo>(!!TFrom& source, int64 offset, int32 displacement) cil managed aggressiveinlining
	{
		.maxstack 3

		ldarg.0
		ldarg.1
		conv.i
		sizeof !!1
		mul
		add
		ldarg.2
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

	.method public hidebysig static !!TFrom&  Decrement<TFrom, TSize>(!!TFrom& source) cil managed aggressiveinlining
	{
		.maxstack 2

		ldarg.0
		sizeof !!1
		sub
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

	.method public hidebysig static !!TTo&  Decrement<TFrom, TSize, TTo>(!!TFrom& source) cil managed aggressiveinlining
	{
		.maxstack 2

		ldarg.0
		sizeof !!1
		sub
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