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



.class public sequential ansi sealed beforefieldinit SuperComicLib.Runtime.ReadOnlyRefpoint`1<T>
    extends [mscorlib]System.ValueType
{
    .custom instance void [mscorlib]System.Runtime.CompilerServices.IsByRefLikeAttribute::.ctor() = ( 01 00 00 00 )
    .custom instance void [mscorlib]System.Runtime.CompilerServices.IsReadOnlyAttribute::.ctor() = ( 01 00 00 00 )
    .custom instance void [mscorlib]System.Reflection.DefaultMemberAttribute::.ctor(string) = ( 01 00 04 49 74 65 6D 00 00 )

    .pack 0
    .size 1

    .method public hidebysig specialname instance void*  get_address() cil managed aggressiveinlining
    {
        .maxstack 1
        
        ldarg.0
        conv.u
        ret
    }

    .method public hidebysig specialname instance !T&  get_value() cil managed aggressiveinlining
    {
        .param [0]
        .custom instance void [mscorlib]System.Runtime.CompilerServices.IsReadOnlyAttribute::.ctor() = ( 01 00 00 00 )

        .maxstack 1
        
        ldarg.0
        ret
    }
    
    .method public hidebysig specialname instance !T&  get_Item(int64 index) cil managed aggressiveinlining
    {
        .param [0]
        .custom instance void [mscorlib]System.Runtime.CompilerServices.IsReadOnlyAttribute::.ctor() = ( 01 00 00 00 )

        .maxstack 3

        ldarg.0
        ldarg.1
        sizeof !T
        conv.i
        mul
        add
        ret
    }
    
    .method public hidebysig specialname instance valuetype SuperComicLib.Runtime.ReadOnlyRefpoint`1<!T>&  get_inc() cil managed aggressiveinlining
    {
        .param [0]
        .custom instance void [mscorlib]System.Runtime.CompilerServices.IsReadOnlyAttribute::.ctor() = ( 01 00 00 00 )

        .maxstack 2

        ldarg.0
        sizeof !T
        conv.i
        add
        ret
    }
    
    .method public hidebysig specialname instance valuetype SuperComicLib.Runtime.ReadOnlyRefpoint`1<!T>&  get_dec() cil managed aggressiveinlining
    {
        .param [0]
        .custom instance void [mscorlib]System.Runtime.CompilerServices.IsReadOnlyAttribute::.ctor() = ( 01 00 00 00 )

        .maxstack 2

        ldarg.0
        sizeof !T
        conv.i
        sub
        ret
    }
    
    .method public hidebysig instance valuetype SuperComicLib.Runtime.ReadOnlyRefpoint`1<!!U>&  cast<U>() cil managed aggressiveinlining
    {
        .param [0]
        .custom instance void [mscorlib]System.Runtime.CompilerServices.IsReadOnlyAttribute::.ctor() = ( 01 00 00 00 )

        .maxstack 1

        ldarg.0
        ret
    }
    
    .method public hidebysig instance valuetype SuperComicLib.Runtime.ReadOnlyRefpoint`1<!T>&  'add'(int32 offset) cil managed aggressiveinlining
    {
        .param [0]
        .custom instance void [mscorlib]System.Runtime.CompilerServices.IsReadOnlyAttribute::.ctor() = ( 01 00 00 00 )

        .maxstack 3

        ldarg.0
        ldarg.1
        sizeof !T
        conv.i
        mul
        add
        ret
    }
    
    .method public hidebysig instance valuetype SuperComicLib.Runtime.ReadOnlyRefpoint`1<!T>&  'sub'(int32 offset) cil managed aggressiveinlining
    {
        .param [0]
        .custom instance void [mscorlib]System.Runtime.CompilerServices.IsReadOnlyAttribute::.ctor() = ( 01 00 00 00 )

        .maxstack 3

        ldarg.0
        ldarg.1
        sizeof !T
        conv.i
        mul
        sub
        ret
    }
    
    .method public hidebysig instance valuetype SuperComicLib.Runtime.ReadOnlyRefpoint`1<!T>&  'add'(uint32 offset) cil managed aggressiveinlining
    {
        .param [0]
        .custom instance void [mscorlib]System.Runtime.CompilerServices.IsReadOnlyAttribute::.ctor() = ( 01 00 00 00 )

        .maxstack 3

        ldarg.0
        ldarg.1
        sizeof !T
        conv.i
        mul
        add
        ret
    }
    
    .method public hidebysig instance valuetype SuperComicLib.Runtime.ReadOnlyRefpoint`1<!T>&  'sub'(uint32 offset) cil managed aggressiveinlining
    {
        .param [0]
        .custom instance void [mscorlib]System.Runtime.CompilerServices.IsReadOnlyAttribute::.ctor() = ( 01 00 00 00 )

        .maxstack 3

        ldarg.0
        ldarg.1
        sizeof !T
        conv.i
        mul
        sub
        ret
    }
    
    .method public hidebysig instance valuetype SuperComicLib.Runtime.ReadOnlyRefpoint`1<!T>&  'add'(int64 offset) cil managed aggressiveinlining
    {
        .param [0]
        .custom instance void [mscorlib]System.Runtime.CompilerServices.IsReadOnlyAttribute::.ctor() = ( 01 00 00 00 )

        .maxstack 3

        ldarg.0
        ldarg.1
        sizeof !T
        conv.i
        mul
        add
        ret
    }
    
    .method public hidebysig instance valuetype SuperComicLib.Runtime.ReadOnlyRefpoint`1<!T>&  'sub'(int64 offset) cil managed aggressiveinlining
    {
        .param [0]
        .custom instance void [mscorlib]System.Runtime.CompilerServices.IsReadOnlyAttribute::.ctor() = ( 01 00 00 00 )

        .maxstack 3

        ldarg.0
        ldarg.1
        sizeof !T
        conv.i
        mul
        sub
        ret
    }
    
    .method public hidebysig instance valuetype SuperComicLib.Runtime.ReadOnlyRefpoint`1<!T>&  'add'(uint64 offset) cil managed aggressiveinlining
    {
        .param [0]
        .custom instance void [mscorlib]System.Runtime.CompilerServices.IsReadOnlyAttribute::.ctor() = ( 01 00 00 00 )

        .maxstack 3

        ldarg.0
        ldarg.1
        sizeof !T
        conv.i
        mul
        add
        ret
    }
    
    .method public hidebysig instance valuetype SuperComicLib.Runtime.ReadOnlyRefpoint`1<!T>&  'sub'(uint64 offset) cil managed aggressiveinlining
    {
        .param [0]
        .custom instance void [mscorlib]System.Runtime.CompilerServices.IsReadOnlyAttribute::.ctor() = ( 01 00 00 00 )

        .maxstack 3

        ldarg.0
        ldarg.1
        sizeof !T
        conv.i
        mul
        sub
        ret
    }




    //
    //				extend 'add'
    //
    .method public hidebysig instance valuetype SuperComicLib.Runtime.ReadOnlyRefpoint`1<!T>&  'add'(int64 offset, int32 displacement) cil managed aggressiveinlining
    {
        .param [0]
        .custom instance void [mscorlib]System.Runtime.CompilerServices.IsReadOnlyAttribute::.ctor() = ( 01 00 00 00 )

        .maxstack 3

        ldarg.0
        ldarg.1
        sizeof !T
        conv.i
        mul
        add
        ldarg.2
        add
        ret
    }

    .method public hidebysig instance valuetype SuperComicLib.Runtime.ReadOnlyRefpoint`1<!T>&  'add'<TSize>(int64 offset) cil managed aggressiveinlining
    {
        .param [0]
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

    .method public hidebysig instance valuetype SuperComicLib.Runtime.ReadOnlyRefpoint`1<!T>&  'add'<TSize>(int64 offset, int32 displacement) cil managed aggressiveinlining
    {
        .param [0]
        .custom instance void [mscorlib]System.Runtime.CompilerServices.IsReadOnlyAttribute::.ctor() = ( 01 00 00 00 )

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

    .method public hidebysig instance valuetype SuperComicLib.Runtime.ReadOnlyRefpoint`1<!T>&  'sub'<TSize>(int64 offset) cil managed aggressiveinlining
    {
        .param [0]
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



    .method public hidebysig instance int64  byteOffset(valuetype SuperComicLib.Runtime.ReadOnlyRefpoint`1<!T>& right) cil managed aggressiveinlining
    {
        .param [1]
        .custom instance void [mscorlib]System.Runtime.CompilerServices.IsReadOnlyAttribute::.ctor() = ( 01 00 00 00 )

        .maxstack 2

        ldarg.0
        ldarg.1
        sub
        conv.i8
        ret
    }

    .method public hidebysig instance int64  elementOffset(valuetype SuperComicLib.Runtime.ReadOnlyRefpoint`1<!T>& right) cil managed aggressiveinlining
    {
        .param [1]
        .custom instance void [mscorlib]System.Runtime.CompilerServices.IsReadOnlyAttribute::.ctor() = ( 01 00 00 00 )

        .maxstack 2

        ldarg.0
        ldarg.1
        sub
        sizeof !T
        div
        conv.i8
        ret
    }

    .method public final hidebysig virtual instance int32  GetHashCode() cil managed
    {
        .maxstack 1
        
        ldc.i4.0
        ret
    }
    
    .method public final hidebysig virtual instance bool  Equals(object obj) cil managed
    {
        .maxstack 1

        ldc.i4.0
        ret
    }

    .method public final hidebysig virtual instance string  ToString() cil managed
    {
        .maxstack 8

        .locals (
            [0] uint32
        )

        // (uint)(this >> 32)
        ldarg.0
        conv.u
        ldc.i4.s 32
        shr.un
        conv.u4
        stloc.0
        
        // .ToString("X4")
        ldloca.s 0
        ldstr "X4"
        call instance string [mscorlib]System.UInt32::ToString(string)
        
        // + "`" +
        ldstr "`"

        // (uint)this
        ldarg.0
        conv.u4
        stloc.0

        // .ToString("X8")
        ldloca.s 0
        ldstr "X8"
        call instance string [mscorlib]System.UInt32::ToString(string)

        // make result
        call string [mscorlib]System.String::Concat(string, string, string)
        ret
    }

    .method public hidebysig instance !T&  GetPinnableReference() cil managed aggressiveinlining
    {
        .custom instance void [mscorlib]System.ComponentModel.EditorBrowsableAttribute::.ctor(valuetype [mscorlib]System.ComponentModel.EditorBrowsableState) = ( 01 00 01 00 00 00 00 00 )

        .maxstack 1

        ldarg.0
        ret
    }
    
    .method public hidebysig static valuetype SuperComicLib.Runtime.ReadOnlyRefpoint`1<!T>&  As(void* source) cil managed aggressiveinlining
    {
        .param [0]
        .custom instance void [mscorlib]System.Runtime.CompilerServices.IsReadOnlyAttribute::.ctor() = ( 01 00 00 00 )

        .maxstack 1

        ldarg.0
        conv.i
        ret
    }

    .method public hidebysig static valuetype SuperComicLib.Runtime.ReadOnlyRefpoint`1<!T>&  As(!T& source) cil managed aggressiveinlining
    {
        .param [0]
        .custom instance void [mscorlib]System.Runtime.CompilerServices.IsReadOnlyAttribute::.ctor() = ( 01 00 00 00 )

        .param [1]
        .custom instance void [mscorlib]System.Runtime.CompilerServices.IsReadOnlyAttribute::.ctor() = ( 01 00 00 00 )

        .maxstack 1

        ldarg.0
        ret
    }
    
    .method public hidebysig static valuetype SuperComicLib.Runtime.ReadOnlyRefpoint`1<!!U>&  As<U>(!T& source) cil managed aggressiveinlining
    {
        .param [0]
        .custom instance void [mscorlib]System.Runtime.CompilerServices.IsReadOnlyAttribute::.ctor() = ( 01 00 00 00 )

        .param [1]
        .custom instance void [mscorlib]System.Runtime.CompilerServices.IsReadOnlyAttribute::.ctor() = ( 01 00 00 00 )

        .maxstack 1

        ldarg.0
        ret
    }
    
    .method public hidebysig specialname static bool  op_Equality(
            [in] valuetype SuperComicLib.Runtime.ReadOnlyRefpoint`1<!T>& left,
            [in] valuetype SuperComicLib.Runtime.ReadOnlyRefpoint`1<!T>& right) cil managed aggressiveinlining
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
    
    .method public hidebysig specialname static bool  op_Inequality(
            [in] valuetype SuperComicLib.Runtime.ReadOnlyRefpoint`1<!T>& left,
            [in] valuetype SuperComicLib.Runtime.ReadOnlyRefpoint`1<!T>& right) cil managed aggressiveinlining
    {
        .param [1]
        .custom instance void [mscorlib]System.Runtime.CompilerServices.IsReadOnlyAttribute::.ctor() = ( 01 00 00 00 )

        .param [2]
        .custom instance void [mscorlib]System.Runtime.CompilerServices.IsReadOnlyAttribute::.ctor() = ( 01 00 00 00 )
        
        .maxstack 2

        ldarg.0
        ldarg.1
        ceq
        ldc.i4.0
        ceq
        ret
    }
    
    .method public hidebysig specialname static bool  op_LessThan(
            [in] valuetype SuperComicLib.Runtime.ReadOnlyRefpoint`1<!T>& left,
            [in] valuetype SuperComicLib.Runtime.ReadOnlyRefpoint`1<!T>& right) cil managed aggressiveinlining
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
    
    .method public hidebysig specialname static bool  op_GreaterThan(
            [in] valuetype SuperComicLib.Runtime.ReadOnlyRefpoint`1<!T>& left,
            [in] valuetype SuperComicLib.Runtime.ReadOnlyRefpoint`1<!T>& right) cil managed aggressiveinlining
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
    
    .method public hidebysig specialname static bool  op_LessThanOrEqual(
            [in] valuetype SuperComicLib.Runtime.ReadOnlyRefpoint`1<!T>& left,
            [in] valuetype SuperComicLib.Runtime.ReadOnlyRefpoint`1<!T>& right) cil managed aggressiveinlining
    {
        .param [1]
        .custom instance void [mscorlib]System.Runtime.CompilerServices.IsReadOnlyAttribute::.ctor() = ( 01 00 00 00 )

        .param [2]
        .custom instance void [mscorlib]System.Runtime.CompilerServices.IsReadOnlyAttribute::.ctor() = ( 01 00 00 00 )
        
        .maxstack 2

        ldarg.0
        ldarg.1
        cgt.un
        ldc.i4.0
        ceq
        ret
    }
    
    .method public hidebysig specialname static bool  op_GreaterThanOrEqual(
            [in] valuetype SuperComicLib.Runtime.ReadOnlyRefpoint`1<!T>& left,
            [in] valuetype SuperComicLib.Runtime.ReadOnlyRefpoint`1<!T>& right) cil managed aggressiveinlining
    {
        .param [1]
        .custom instance void [mscorlib]System.Runtime.CompilerServices.IsReadOnlyAttribute::.ctor() = ( 01 00 00 00 )

        .param [2]
        .custom instance void [mscorlib]System.Runtime.CompilerServices.IsReadOnlyAttribute::.ctor() = ( 01 00 00 00 )
        
        .maxstack 2

        ldarg.0
        ldarg.1
        clt.un
        ldc.i4.0
        ceq
        ret
    }

    .method public hidebysig specialname static int64  op_Subtraction(
            [in] valuetype SuperComicLib.Runtime.ReadOnlyRefpoint`1<!T>& left,
            [in] valuetype SuperComicLib.Runtime.ReadOnlyRefpoint`1<!T>& right) cil managed aggressiveinlining
    {
        .param [1]
        .custom instance void [mscorlib]System.Runtime.CompilerServices.IsReadOnlyAttribute::.ctor() = ( 01 00 00 00 )

        .param [2]
        .custom instance void [mscorlib]System.Runtime.CompilerServices.IsReadOnlyAttribute::.ctor() = ( 01 00 00 00 )
        
        .maxstack 2

        ldarg.0
        ldarg.1
        sub
        sizeof !T
        div
        conv.i8
        ret
    }

    .method public hidebysig specialname static !T  op_UnaryPlus([in] valuetype SuperComicLib.Runtime.ReadOnlyRefpoint`1<!T>& v) cil managed aggressiveinlining
    {
        .param [1]
        .custom instance void [mscorlib]System.Runtime.CompilerServices.IsReadOnlyAttribute::.ctor() = ( 01 00 00 00 )

        .maxstack 1

        ldarg.0
        ldobj !T
        ret
    }
    
    .property instance void* address()
    {
        .get instance void* SuperComicLib.Runtime.ReadOnlyRefpoint`1::get_address()
    }

    .property instance !T& 'value'()
    {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.IsReadOnlyAttribute::.ctor() = ( 01 00 00 00 )

        .get instance !T& SuperComicLib.Runtime.ReadOnlyRefpoint`1::get_value()
    }

    .property instance !T& Item(int64)
    {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.IsReadOnlyAttribute::.ctor() = ( 01 00 00 00 )

        .get instance !T& SuperComicLib.Runtime.ReadOnlyRefpoint`1::get_Item(int64)
    }

    .property instance valuetype SuperComicLib.Runtime.ReadOnlyRefpoint`1<!T>& inc()
    {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.IsReadOnlyAttribute::.ctor() = ( 01 00 00 00 )
        .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 )

        .get instance valuetype SuperComicLib.Runtime.ReadOnlyRefpoint`1<!T>& SuperComicLib.Runtime.ReadOnlyRefpoint`1::get_inc()
    }

    .property instance valuetype SuperComicLib.Runtime.ReadOnlyRefpoint`1<!T>& dec()
    {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.IsReadOnlyAttribute::.ctor() = ( 01 00 00 00 )
        .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 )

        .get instance valuetype SuperComicLib.Runtime.ReadOnlyRefpoint`1<!T>& SuperComicLib.Runtime.ReadOnlyRefpoint`1::get_dec()
    }
}