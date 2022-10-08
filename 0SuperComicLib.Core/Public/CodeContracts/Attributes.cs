// MIT License
//
// Copyright (c) 2019-2022 SuperComic (ekfvoddl3535@naver.com)
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

using System;

namespace SuperComicLib.CodeContracts
{
    /// <summary>
    /// 이 특성으로 표시된 반환 값은 Null이 아닙니다
    /// </summary>
    [AttributeUsage(AttributeTargets.ReturnValue, AllowMultiple = false, Inherited = false)]
    public sealed class NotNullAttribute : Attribute { }

    /// <summary>
    /// 이 특성으로 표시된 인수나 필드는 Null을 허용합니다.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public sealed class AllowNullAttribute : Attribute { }

    /// <summary>
    /// 이 특성으로 표시된 인수나 필드는 Null을 허용하지 않습니다
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public sealed class DisallowNullAttribute : Attribute { }

    /// <summary>
    /// 이 특성으로 표시된 배열이나 문자열은 비어있을 수 있습니다 (allow Length == 0)
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public sealed class AllowEmptyArrayAttribute : Attribute { }

    /// <summary>
    /// 이 특성으로 표시된 배열이나 문자열은 비어있을 수 없으며, 허용하지 않습니다 (disallow Length == 0)
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public sealed class DisallowEmptyArrayAttribute : Attribute { }

    /// <summary>
    /// 이 특성으로 표시된 메소드나 속성은 객체(class나 struct)의 상태(field 값)를 변경하지 않습니다
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public sealed class ConstFieldAttribute : Attribute { }

    /// <summary>
    /// 이 특성으로 표시된 반환 값은 빈 데이터(또는 배열)이 아닙니다
    /// </summary>
    [AttributeUsage(AttributeTargets.ReturnValue, AllowMultiple = false, Inherited = false)]
    public sealed class NotEmptyArrayAttribute : Attribute { }

    /// <summary>
    /// 이 특성으로 표시된 배열이나 문자열은 Null과 Empty 상태를 허용하지 않습니다
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public sealed class DisallowNullOrEmptyAttribute : Attribute { }

    /// <summary>
    /// Need 64-bit Processor, 64-bit OS
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Constructor, AllowMultiple = false, Inherited = true)]
    public sealed class X64OnlyAttribute : Attribute { }

    /// <summary>
    /// It convert 64-bit integer (QWORD) to 32-bit integer (DWORD).
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Constructor, AllowMultiple = false, Inherited = true)]
    public sealed class X64LossOfLengthAttribute : Attribute { }

    /// <summary>
    /// This functional or <see langword="class"/>, <see langword="struct"/>, <see langword="interface"/>, API is development version
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    public sealed class DevReleaseAttribute : Attribute { }

    /// <summary>
    /// This struct must be passed using the keywords <see langword="ref"/> or <see langword="in"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = true)]
    public sealed class ParamRefAttribute : Attribute { }
}
