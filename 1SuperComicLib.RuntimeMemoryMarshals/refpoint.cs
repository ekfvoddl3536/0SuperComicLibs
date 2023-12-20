// The MIT License (MIT)
//
// Copyright (c) 2023. SuperComic (ekfvoddl3535@naver.com)
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

#pragma warning disable IDE1006
#pragma warning disable CS1591

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SuperComicLib.RuntimeMemoryMarshals
{
    /// <summary>
    /// An addressable reference wrapper type.
    /// <para/>
    /// Do not use as a value (e.g., specifying as a field, or assigning to a local variable or parameter without <see langword="ref"/>), or with out parameters.<br/>
    /// It must always be passed with the <see langword="ref"/> or <see langword="in"/> keywords to convey 'reference' (the address of the reference) rather than 'value'.
    /// <para/>
    /// Additionally, using it as a pointer(*) for reference passing is strongly discouraged, as JIT does not track the reference in this case.
    /// </summary>
    /// <remarks>
    /// [ko-KR]<br/>
    /// 주소 연산가능한 참조 래퍼 타입.
    /// <para/>
    /// 이 타입은 절대로 값(예: 필드로 지정, 지역 변수나 매개 변수에 ref 키워드 없이 지정 등)으로 사용되면 안됩니다.<br/>
    /// 반드시, ref 또는 in 키워드를 사용하여 '참조'(참조 주소 값)를 전달해야 합니다.
    /// <para/>
    /// 또한, 포인터(*)를 사용하여 참조를 전달하는 것은 JIT가 참조를 추적할 수 없게 되므로, 절대로 권장하지 않습니다.
    /// </remarks>
    [StructLayout(LayoutKind.Sequential, Pack = 0, Size = 1)]
    public readonly unsafe ref struct refpoint<T>
    {
        /// <summary>
        /// <c>(<see langword="void"/>*)(<see langword="ref this"/>)</c>
        /// </summary>
        /// <remarks>
        /// Gets an address that is only safely usable for an extremely short period (within 1 CPU cycle).<br/>
        /// This property does not provide a fixed address, and the returned address should not be stored or reused to ensure safety.
        /// <para/>
        /// It is recommended to use this only for <see langword="null"/> checking.
        /// </remarks>
        public void* address
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => throw new PlatformNotSupportedException();
        }

        /// <summary>
        /// <c>(<typeparamref name="T"/>*)(<see langword="ref this"/>)</c>
        /// </summary>
        public ref T value
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => throw new PlatformNotSupportedException();
        }

        /// <summary>
        /// <c>(<typeparamref name="T"/>*)(<see langword="ref this"/>) + <paramref name="offset"/></c>
        /// </summary>
        public ref T this[long offset]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => throw new PlatformNotSupportedException();
        }

        /// <summary>
        /// <c>(<typeparamref name="T"/>*)(<see langword="ref this"/>) + 1</c>
        /// </summary>
        public ref refpoint<T> inc
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => throw new PlatformNotSupportedException();
        }

        /// <summary>
        /// <c>(<typeparamref name="T"/>*)(<see langword="ref this"/>) - 1</c>
        /// </summary>
        public ref refpoint<T> dec
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => throw new PlatformNotSupportedException();
        }
        
        /// <summary>
        /// <c>(<typeparamref name="U"/>*)(<see langword="ref this"/>)</c>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref refpoint<U> cast<U>() => throw new PlatformNotSupportedException();

        /// <summary>
        /// <c>(<typeparamref name="T"/>*)(<see langword="ref this"/>) + <paramref name="offset"/></c>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref refpoint<T> add(int offset) => throw new PlatformNotSupportedException();
        /// <summary>
        /// <c>(<typeparamref name="T"/>*)(<see langword="ref this"/>) - <paramref name="offset"/></c>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref refpoint<T> sub(int offset) => throw new PlatformNotSupportedException();

        /// <summary>
        /// <c>(<typeparamref name="T"/>*)(<see langword="ref this"/>) + <paramref name="offset"/></c>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref refpoint<T> add(uint offset) => throw new PlatformNotSupportedException();
        /// <summary>
        /// <c>(<typeparamref name="T"/>*)(<see langword="ref this"/>) - <paramref name="offset"/></c>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref refpoint<T> sub(uint offset) => throw new PlatformNotSupportedException();

        /// <summary>
        /// <c>(<typeparamref name="T"/>*)(<see langword="ref this"/>) + <paramref name="offset"/></c>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref refpoint<T> add(long offset) => throw new PlatformNotSupportedException();
        /// <summary>
        /// <c>(<typeparamref name="T"/>*)(<see langword="ref this"/>) - <paramref name="offset"/></c>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref refpoint<T> sub(long offset) => throw new PlatformNotSupportedException();

        /// <summary>
        /// <c>(<typeparamref name="T"/>*)(<see langword="ref this"/>) + <paramref name="offset"/></c>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref refpoint<T> add(ulong offset) => throw new PlatformNotSupportedException();
        /// <summary>
        /// <c>(<typeparamref name="T"/>*)(<see langword="ref this"/>) - <paramref name="offset"/></c>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref refpoint<T> sub(ulong offset) => throw new PlatformNotSupportedException();

        #region extend 'add', 'sub'
        /// <summary>
        /// <c>(<see langword="byte"/>*)((<typeparamref name="T"/>*)(<see langword="ref this"/>) + <paramref name="offset"/>) + <paramref name="displacement"/></c>
        /// <para/>
        /// <c>LEA rax, [<see langword="this"/> + <paramref name="offset"/> * <see langword="sizeof"/>(<typeparamref name="T"/>) + <paramref name="displacement"/>]</c>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref refpoint<T> add(long offset, int displacement) => throw new PlatformNotSupportedException();

        /// <summary>
        /// <c>(<typeparamref name="TSize"/>*)(<see langword="ref this"/>) + <paramref name="offset"/></c>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref refpoint<T> add<TSize>(long offset) => throw new PlatformNotSupportedException();

        /// <summary>
        /// <c>(<see langword="byte"/>*)((<typeparamref name="TSize"/>*)(<see langword="ref this"/>) + <paramref name="offset"/>) + <paramref name="displacement"/></c>
        /// <para/>
        /// <c>LEA rax, [<see langword="this"/> + <paramref name="offset"/> * <see langword="sizeof"/>(<typeparamref name="TSize"/>) + <paramref name="displacement"/>]</c>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref refpoint<T> add<TSize>(long offset, int displacement) => throw new PlatformNotSupportedException();

        /// <summary>
        /// <c>(<typeparamref name="TSize"/>*)(<see langword="ref this"/>) - <paramref name="offset"/></c>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref refpoint<T> sub<TSize>(long offset) => throw new PlatformNotSupportedException();
        #endregion

        /// <summary>
        /// <c>(<see langword="byte"/>*)(<see langword="ref this"/>) - (<see langword="byte"/>*)(<see langword="ref"/> <paramref name="right"/>)</c>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long byteOffset(ref refpoint<T> right) => throw new PlatformNotSupportedException();

        /// <summary>
        /// <c>(<typeparamref name="T"/>*)(<see langword="ref this"/>) - (<typeparamref name="T"/>*)(<see langword="ref"/> <paramref name="right"/>)</c>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long elementOffset(ref refpoint<T> right) => throw new PlatformNotSupportedException();

        public override int GetHashCode() => throw new PlatformNotSupportedException();
        public override bool Equals(object obj) => throw new PlatformNotSupportedException();
        public override string ToString() => throw new PlatformNotSupportedException();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T GetPinnableReference() => throw new PlatformNotSupportedException();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref refpoint<T> As(void* source) => throw new PlatformNotSupportedException();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref refpoint<T> As(ref T source) => throw new PlatformNotSupportedException();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref refpoint<U> As<U>(ref T source) => throw new PlatformNotSupportedException();

        /// <summary>
        /// <c>(<see langword="void"/>*)(<see langword="ref"/> <paramref name="left"/>) == (<see langword="void"/>*)(<see langword="ref"/> <paramref name="right"/>)</c>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(in refpoint<T> left, in refpoint<T> right) => throw new PlatformNotSupportedException();
        /// <summary>
        /// <c>(<see langword="void"/>*)(<see langword="ref"/> <paramref name="left"/>) &#33;= (<see langword="void"/>*)(<see langword="ref"/> <paramref name="right"/>)</c>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(in refpoint<T> left, in refpoint<T> right) => throw new PlatformNotSupportedException();

        /// <summary>
        /// <c>(<see langword="void"/>*)(<see langword="ref"/> <paramref name="left"/>) &lt; (<see langword="void"/>*)(<see langword="ref"/> <paramref name="right"/>)</c>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <(in refpoint<T> left, in refpoint<T> right) => throw new PlatformNotSupportedException();
        /// <summary>
        /// <c>(<see langword="void"/>*)(<see langword="ref"/> <paramref name="left"/>) &gt; (<see langword="void"/>*)(<see langword="ref"/> <paramref name="right"/>)</c>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >(in refpoint<T> left, in refpoint<T> right) => throw new PlatformNotSupportedException();

        /// <summary>
        /// <c>(<see langword="void"/>*)(<see langword="ref"/> <paramref name="left"/>) &lt;= (<see langword="void"/>*)(<see langword="ref"/> <paramref name="right"/>)</c>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <=(in refpoint<T> left, in refpoint<T> right) => throw new PlatformNotSupportedException();
        /// <summary>
        /// <c>(<see langword="void"/>*)(<see langword="ref"/> <paramref name="left"/>) &gt;= (<see langword="void"/>*)(<see langword="ref"/> <paramref name="right"/>)</c>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >=(in refpoint<T> left, in refpoint<T> right) => throw new PlatformNotSupportedException();

        /// <summary>
        /// <c>(<typeparamref name="T"/>*)(<see langword="ref"/> <paramref name="left"/>) - (<typeparamref name="T"/>*)(<see langword="ref"/> <paramref name="right"/>)</c>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long operator -(in refpoint<T> left, in refpoint<T> right) => throw new PlatformNotSupportedException();

        /// <summary>
        /// *(<typeparamref name="T"/>*)(<see langword="ref"/> <paramref name="v"/>)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T operator +(in refpoint<T> v) => throw new PlatformNotSupportedException();
    }
}
