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


// nuint_t
.class public sequential ansi sealed beforefieldinit SuperComicLib.nuint_t
       extends [mscorlib]System.ValueType
       implements class [mscorlib]System.IComparable`1<valuetype SuperComicLib.nuint_t>, 
                  class [mscorlib]System.IEquatable`1<valuetype SuperComicLib.nuint_t>
{
        
    .custom instance void [mscorlib]System.Runtime.CompilerServices.IsReadOnlyAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .field private initonly native uint _value

    // .ctor
    .method private hidebysig specialname rtspecialname instance void  .ctor(void* x) cil managed aggressiveinlining
    {
        .maxstack 2

        ldarg.0
        ldarg.1
        stfld native uint SuperComicLib.nuint_t::_value
        ret
    }

    .method public hidebysig newslot virtual final instance bool  Equals(valuetype SuperComicLib.nuint_t other) cil managed aggressiveinlining
    {
        .maxstack 2
        
        ldarg.0
        ldarg.1
        call bool SuperComicLib.nuint_t::op_Equality(valuetype SuperComicLib.nuint_t, valuetype SuperComicLib.nuint_t)
        ret
    }

    .method public hidebysig newslot virtual final instance int32  CompareTo(valuetype SuperComicLib.nuint_t other) cil managed aggressiveinlining
    {
        .maxstack 8

        ldarg.0
        ldfld native uint SuperComicLib.nuint_t::_value
        ldarg.1
        ldfld native uint SuperComicLib.nuint_t::_value
        clt
        neg
        ldarg.0
        ldfld native uint SuperComicLib.nuint_t::_value
        ldarg.1
        ldfld native uint SuperComicLib.nuint_t::_value
        cgt
        xor
        ret
    }

    .method public hidebysig instance string ToString(string format) cil managed aggressiveinlining
    {
        .maxstack 8
        
        ldarg.0
        ldflda native uint SuperComicLib.nuint_t::_value
        ldarg.1
        call instance string [mscorlib]System.UIntPtr::ToString(string)
        ret
    }

    .method public hidebysig virtual instance bool Equals(object obj) cil managed aggressiveinlining
    {
        .maxstack 8
        
        ldarg.1
        isinst SuperComicLib.nuint_t
        brfalse.s _FALSE

        ldarg.1
        unbox.any SuperComicLib.nuint_t
        ldfld native uint SuperComicLib.nuint_t::_value
        ldarg.0
        ldfld native uint SuperComicLib.nuint_t::_value
        ceq
        ret

        _FALSE:
        ldc.i4.0
        ret
    }

    .method public hidebysig virtual instance int32 GetHashCode() cil managed aggressiveinlining
    {
        .maxstack 8
        
        ldarg.0
        ldflda native uint SuperComicLib.nuint_t::_value
        call instance int32 [mscorlib]System.UIntPtr::GetHashCode()
        ret
    }

    .method public hidebysig virtual instance string ToString() cil managed aggressiveinlining
    {
        .maxstack 8
        
        ldarg.0
        ldflda native uint SuperComicLib.nuint_t::_value
        call instance string [mscorlib]System.UIntPtr::ToString()
        ret
    }

    .method public hidebysig specialname static bool  op_Equality(valuetype SuperComicLib.nuint_t a, valuetype SuperComicLib.nuint_t b) cil managed aggressiveinlining
    {
        .maxstack 8
        
        ldarg.0
        ldfld native uint SuperComicLib.nuint_t::_value
        ldarg.1
        ldfld native uint SuperComicLib.nuint_t::_value
        ceq
        ret
    }

    .method public hidebysig specialname static bool  op_Inequality(valuetype SuperComicLib.nuint_t a, valuetype SuperComicLib.nuint_t b) cil managed aggressiveinlining
    {
        .maxstack 8
        
        ldarg.0
        ldfld native uint SuperComicLib.nuint_t::_value
        ldarg.1
        ldfld native uint SuperComicLib.nuint_t::_value
        ceq
        ldc.i4.0
        ceq
        ret
    }

    .method public hidebysig specialname static bool  op_LessThan(valuetype SuperComicLib.nuint_t a, valuetype SuperComicLib.nuint_t b) cil managed aggressiveinlining
    {
        .maxstack 8
        
        ldarg.0
        ldfld native uint SuperComicLib.nuint_t::_value
        ldarg.1
        ldfld native uint SuperComicLib.nuint_t::_value
        clt
        ret
    }

    .method public hidebysig specialname static bool  op_GreaterThan(valuetype SuperComicLib.nuint_t a, valuetype SuperComicLib.nuint_t b) cil managed aggressiveinlining
    {
        .maxstack 8
        
        ldarg.0
        ldfld native uint SuperComicLib.nuint_t::_value
        ldarg.1
        ldfld native uint SuperComicLib.nuint_t::_value
        cgt
        ret
    }

    .method public hidebysig specialname static bool  op_GreaterThanOrEqual(valuetype SuperComicLib.nuint_t a, valuetype SuperComicLib.nuint_t b) cil managed aggressiveinlining
    {
        .maxstack 8
        
        ldarg.0
        ldfld native uint SuperComicLib.nuint_t::_value
        ldarg.1
        ldfld native uint SuperComicLib.nuint_t::_value
        clt
        ldc.i4.0
        ceq
        ret
    }

    .method public hidebysig specialname static bool  op_LessThanOrEqual(valuetype SuperComicLib.nuint_t a, valuetype SuperComicLib.nuint_t b) cil managed aggressiveinlining
    {
        .maxstack 8
        
        ldarg.0
        ldfld native uint SuperComicLib.nuint_t::_value
        ldarg.1
        ldfld native uint SuperComicLib.nuint_t::_value
        cgt
        ldc.i4.0
        ceq
        ret
    }

    .method public hidebysig specialname static valuetype SuperComicLib.nuint_t op_Addition(valuetype SuperComicLib.nuint_t a, valuetype SuperComicLib.nuint_t b) cil managed aggressiveinlining
    {
        .maxstack 8
        
        ldarg.0
        ldfld native uint SuperComicLib.nuint_t::_value
        ldarg.1
        ldfld native uint SuperComicLib.nuint_t::_value
        add
        newobj instance void SuperComicLib.nuint_t::.ctor(void*)
        ret
    }

    .method public hidebysig specialname static valuetype SuperComicLib.nuint_t op_Subtraction(valuetype SuperComicLib.nuint_t a, valuetype SuperComicLib.nuint_t b) cil managed aggressiveinlining
    {
        .maxstack 8
        
        ldarg.0
        ldfld native uint SuperComicLib.nuint_t::_value
        ldarg.1
        ldfld native uint SuperComicLib.nuint_t::_value
        sub
        newobj instance void SuperComicLib.nuint_t::.ctor(void*)
        ret
    }

    .method public hidebysig specialname static valuetype SuperComicLib.nuint_t op_Multiply(valuetype SuperComicLib.nuint_t a, valuetype SuperComicLib.nuint_t b) cil managed aggressiveinlining
    {
        .maxstack 8
        
        ldarg.0
        ldfld native uint SuperComicLib.nuint_t::_value
        ldarg.1
        ldfld native uint SuperComicLib.nuint_t::_value
        mul
        newobj instance void SuperComicLib.nuint_t::.ctor(void*)
        ret
    }

    .method public hidebysig specialname static valuetype SuperComicLib.nuint_t op_Division(valuetype SuperComicLib.nuint_t a, valuetype SuperComicLib.nuint_t b) cil managed aggressiveinlining
    {
        .maxstack 8
        
        ldarg.0
        ldfld native uint SuperComicLib.nuint_t::_value
        ldarg.1
        ldfld native uint SuperComicLib.nuint_t::_value
        div
        newobj instance void SuperComicLib.nuint_t::.ctor(void*)
        ret
    }

    .method public hidebysig specialname static valuetype SuperComicLib.nuint_t op_Modulus(valuetype SuperComicLib.nuint_t a, valuetype SuperComicLib.nuint_t b) cil managed aggressiveinlining
    {
        .maxstack 8
        
        ldarg.0
        ldfld native uint SuperComicLib.nuint_t::_value
        ldarg.1
        ldfld native uint SuperComicLib.nuint_t::_value
        rem
        newobj instance void SuperComicLib.nuint_t::.ctor(void*)
        ret
    }

    .method public hidebysig specialname static valuetype SuperComicLib.nuint_t op_LeftShift(valuetype SuperComicLib.nuint_t a, int32 b) cil managed aggressiveinlining
    {
        .maxstack 8
        
        ldarg.0
        ldfld native uint SuperComicLib.nuint_t::_value
        ldarg.1
        sizeof void*
        ldc.i4.3
        shl
        ldc.i4.1
        sub
        and
        shl
        newobj instance void SuperComicLib.nuint_t::.ctor(void*)
        ret
    }

    .method public hidebysig specialname static valuetype SuperComicLib.nuint_t op_RightShift(valuetype SuperComicLib.nuint_t a, int32 b) cil managed aggressiveinlining
    {
        .maxstack 8
        
        ldarg.0
        ldfld native uint SuperComicLib.nuint_t::_value
        ldarg.1
        sizeof void*
        ldc.i4.3
        shl
        ldc.i4.1
        sub
        and
        shr
        newobj instance void SuperComicLib.nuint_t::.ctor(void*)
        ret
    }

    .method public hidebysig specialname static valuetype SuperComicLib.nuint_t op_BitwiseAnd(valuetype SuperComicLib.nuint_t a, valuetype SuperComicLib.nuint_t b) cil managed aggressiveinlining
    {
        .maxstack 8
        
        ldarg.0
        ldfld native uint SuperComicLib.nuint_t::_value
        ldarg.1
        ldfld native uint SuperComicLib.nuint_t::_value
        and
        newobj instance void SuperComicLib.nuint_t::.ctor(void*)
        ret
    }

    .method public hidebysig specialname static valuetype SuperComicLib.nuint_t op_BitwiseOr(valuetype SuperComicLib.nuint_t a, valuetype SuperComicLib.nuint_t b) cil managed aggressiveinlining
    {
        .maxstack 8
        
        ldarg.0
        ldfld native uint SuperComicLib.nuint_t::_value
        ldarg.1
        ldfld native uint SuperComicLib.nuint_t::_value
        or
        newobj instance void SuperComicLib.nuint_t::.ctor(void*)
        ret
    }

    .method public hidebysig specialname static valuetype SuperComicLib.nuint_t op_ExclusiveOr(valuetype SuperComicLib.nuint_t a, valuetype SuperComicLib.nuint_t b) cil managed aggressiveinlining
    {
        .maxstack 8
        
        ldarg.0
        ldfld native uint SuperComicLib.nuint_t::_value
        ldarg.1
        ldfld native uint SuperComicLib.nuint_t::_value
        xor
        newobj instance void SuperComicLib.nuint_t::.ctor(void*)
        ret
    }

    .method public hidebysig specialname static valuetype SuperComicLib.nuint_t op_LogicalNot(valuetype SuperComicLib.nuint_t a) cil managed aggressiveinlining
    {
        .maxstack 8
        
        ldarg.0
        ldfld native uint SuperComicLib.nuint_t::_value
        ldc.i4.0
        conv.u
        ceq
        newobj instance void SuperComicLib.nuint_t::.ctor(void*)
        ret
    }

    .method public hidebysig specialname static valuetype SuperComicLib.nuint_t op_OnesComplement(valuetype SuperComicLib.nuint_t a) cil managed aggressiveinlining
    {
        .maxstack 8
        
        ldarg.0
        ldfld native uint SuperComicLib.nuint_t::_value
        not
        newobj instance void SuperComicLib.nuint_t::.ctor(void*)
        ret
    }

    .method public hidebysig specialname static valuetype SuperComicLib.nuint_t op_UnaryNegation(valuetype SuperComicLib.nuint_t a) cil managed aggressiveinlining
    {
        .maxstack 8
        
        ldarg.0
        ldfld native uint SuperComicLib.nuint_t::_value
        neg
        newobj instance void SuperComicLib.nuint_t::.ctor(void*)
        ret
    }

    .method public hidebysig specialname static valuetype SuperComicLib.nuint_t op_UnaryPlus(valuetype SuperComicLib.nuint_t a) cil managed aggressiveinlining
    {
        .maxstack 8
        
        ldarg.0
        ret
    }

    .method public hidebysig specialname static valuetype SuperComicLib.nuint_t op_Increment(valuetype SuperComicLib.nuint_t a) cil managed aggressiveinlining
    {
        .maxstack 8
        
        ldarg.0
        ldfld native uint SuperComicLib.nuint_t::_value
        ldc.i4.1
        conv.u
        add
        newobj instance void SuperComicLib.nuint_t::.ctor(void*)
        ret
    }

    .method public hidebysig specialname static valuetype SuperComicLib.nuint_t op_Decrement(valuetype SuperComicLib.nuint_t a) cil managed aggressiveinlining
    {
        .maxstack 8
        
        ldarg.0
        ldfld native uint SuperComicLib.nuint_t::_value
        ldc.i4.1
        conv.u
        sub
        newobj instance void SuperComicLib.nuint_t::.ctor(void*)
        ret
    }

    .method public hidebysig specialname static valuetype SuperComicLib.nuint_t op_Implicit(bool a) cil managed aggressiveinlining
    {
        .maxstack 1
        
        ldarg.0
        conv.u
        newobj instance void SuperComicLib.nuint_t::.ctor(void*)
        ret
    }

    .method public hidebysig specialname static bool op_True(valuetype SuperComicLib.nuint_t a) cil managed aggressiveinlining
    {
        .maxstack 2

        ldarg.0
        ldfld native uint SuperComicLib.nuint_t::_value
        ldc.i4.0
        conv.u
        cgt.un
        ret
    }

    .method public hidebysig specialname static bool op_False(valuetype SuperComicLib.nuint_t a) cil managed aggressiveinlining
    {
        .maxstack 2

        ldarg.0
        ldfld native uint SuperComicLib.nuint_t::_value
        ldc.i4.0
        conv.u
        cgt.un
        not
        ret
    }

    .method public hidebysig specialname static valuetype SuperComicLib.nuint_t op_Explicit(native int a) cil managed aggressiveinlining
    {
        .maxstack 1

        ldarg.0
        newobj instance void SuperComicLib.nuint_t::.ctor(void*)
        ret
    }

    .method public hidebysig specialname static valuetype SuperComicLib.nuint_t op_Implicit(native uint a) cil managed aggressiveinlining
    {
        .maxstack 1

        ldarg.0
        newobj instance void SuperComicLib.nuint_t::.ctor(void*)
        ret
    }

    .method public hidebysig specialname static valuetype SuperComicLib.nuint_t op_Explicit(int32 a) cil managed aggressiveinlining
    {
        .maxstack 1

        ldarg.0
        conv.u
        newobj instance void SuperComicLib.nuint_t::.ctor(void*)
        ret
    }

    .method public hidebysig specialname static valuetype SuperComicLib.nuint_t op_Implicit(uint32 a) cil managed aggressiveinlining
    {
        .maxstack 1

        ldarg.0
        conv.u
        newobj instance void SuperComicLib.nuint_t::.ctor(void*)
        ret
    }

    .method public hidebysig specialname static valuetype SuperComicLib.nuint_t op_Explicit(int64 a) cil managed aggressiveinlining
    {
        .maxstack 1

        ldarg.0
        conv.u
        newobj instance void SuperComicLib.nuint_t::.ctor(void*)
        ret
    }

    .method public hidebysig specialname static valuetype SuperComicLib.nuint_t op_Implicit(uint64 a) cil managed aggressiveinlining
    {
        .maxstack 1

        ldarg.0
        conv.u
        newobj instance void SuperComicLib.nuint_t::.ctor(void*)
        ret
    }

    .method public hidebysig specialname static bool op_Implicit(valuetype SuperComicLib.nuint_t a) cil managed aggressiveinlining
    {
        .maxstack 2
        
        ldarg.0
        ldfld native uint SuperComicLib.nuint_t::_value
        ldc.i4.0
        conv.u
        cgt.un
        ret
    }

    .method public hidebysig specialname static int8 op_Explicit(valuetype SuperComicLib.nuint_t a) cil managed aggressiveinlining
    {
        .maxstack 1

        ldarg.0
        ldfld native uint SuperComicLib.nuint_t::_value
        conv.i1
        ret
    }

    .method public hidebysig specialname static uint8 op_Explicit(valuetype SuperComicLib.nuint_t a) cil managed aggressiveinlining
    {
        .maxstack 1

        ldarg.0
        ldfld native uint SuperComicLib.nuint_t::_value
        conv.u1
        ret
    }

    .method public hidebysig specialname static int16 op_Explicit(valuetype SuperComicLib.nuint_t a) cil managed aggressiveinlining
    {
        .maxstack 1

        ldarg.0
        ldfld native uint SuperComicLib.nuint_t::_value
        conv.i2
        ret
    }

    .method public hidebysig specialname static uint16 op_Explicit(valuetype SuperComicLib.nuint_t a) cil managed aggressiveinlining
    {
        .maxstack 1

        ldarg.0
        ldfld native uint SuperComicLib.nuint_t::_value
        conv.u2
        ret
    }

    .method public hidebysig specialname static char op_Explicit(valuetype SuperComicLib.nuint_t a) cil managed aggressiveinlining
    {
        .maxstack 1

        ldarg.0
        ldfld native uint SuperComicLib.nuint_t::_value
        conv.u2
        ret
    }

    .method public hidebysig specialname static int32 op_Explicit(valuetype SuperComicLib.nuint_t a) cil managed aggressiveinlining
    {
        .maxstack 1

        ldarg.0
        ldfld native uint SuperComicLib.nuint_t::_value
        conv.i4
        ret
    }

    .method public hidebysig specialname static uint32 op_Explicit(valuetype SuperComicLib.nuint_t a) cil managed aggressiveinlining
    {
        .maxstack 1

        ldarg.0
        ldfld native uint SuperComicLib.nuint_t::_value
        conv.u4
        ret
    }

    .method public hidebysig specialname static int64 op_Explicit(valuetype SuperComicLib.nuint_t a) cil managed aggressiveinlining
    {
        .maxstack 1

        ldarg.0
        ldfld native uint SuperComicLib.nuint_t::_value
        conv.i8
        ret
    }

    .method public hidebysig specialname static uint64 op_Explicit(valuetype SuperComicLib.nuint_t a) cil managed aggressiveinlining
    {
        .maxstack 1

        ldarg.0
        ldfld native uint SuperComicLib.nuint_t::_value
        conv.u8
        ret
    }

    .method public hidebysig specialname static valuetype SuperComicLib.nint_t op_Explicit(valuetype SuperComicLib.nuint_t a) cil managed aggressiveinlining
    {
        .maxstack 1

        ldarg.0
        ldfld native uint SuperComicLib.nuint_t::_value
        newobj instance void SuperComicLib.nint_t::.ctor(void*)
        ret
    }

    .method public hidebysig specialname static valuetype SuperComicLib.nuint_t op_Explicit(valuetype SuperComicLib.nint_t a) cil managed aggressiveinlining
    {
        .maxstack 1

        ldarg.0
        ldfld native int SuperComicLib.nint_t::_value
        newobj instance void SuperComicLib.nuint_t::.ctor(void*)
        ret
    }

    .method public hidebysig specialname static native int op_Explicit(valuetype SuperComicLib.nuint_t a) cil managed aggressiveinlining
    {
        .maxstack 1

        ldarg.0
        ldfld native uint SuperComicLib.nuint_t::_value
        conv.i
        ret
    }

    .method public hidebysig specialname static native uint op_Implicit(valuetype SuperComicLib.nuint_t a) cil managed aggressiveinlining
    {
        .maxstack 1

        ldarg.0
        ldfld native uint SuperComicLib.nuint_t::_value
        ret
    }

    .method public hidebysig specialname static void* op_Explicit(valuetype SuperComicLib.nuint_t a) cil managed aggressiveinlining
    {
        .maxstack 1

        ldarg.0
        ldfld native uint SuperComicLib.nuint_t::_value
        ret
    }

    .method public hidebysig specialname static valuetype SuperComicLib.nuint_t op_Explicit(void* a) cil managed aggressiveinlining
    {
        .maxstack 1

        ldarg.0
        newobj instance void SuperComicLib.nuint_t::.ctor(void*)
        ret
    }
}
