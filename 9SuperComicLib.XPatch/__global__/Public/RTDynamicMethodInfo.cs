// MIT License
//
// Copyright (c) 2019-2022 SuperComic (ekfvoddl3535@naver.com)
// Copyright (c) 2017 Andreas Pardeike
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
using System.Globalization;
using System.Reflection;
using System.Reflection.Emit;

namespace SuperComicLib.XPatch
{
    public sealed class RTDynamicMethodInfo : MethodInfo
    {
        private readonly DynamicMethod m_owner;

        public RTDynamicMethodInfo(DynamicMethod method) => m_owner = method ?? throw new InvalidOperationException();

        public override Module Module => m_owner.Module;

        public override Type[] GetGenericArguments() => null;

        public override ICustomAttributeProvider ReturnTypeCustomAttributes => m_owner.ReturnTypeCustomAttributes;
        public override RuntimeMethodHandle MethodHandle => m_owner.MethodHandle;
        public override MethodAttributes Attributes => m_owner.Attributes;
        public override string Name => m_owner.Name;
        public override Type DeclaringType => m_owner.DeclaringType;
        public override Type ReflectedType => m_owner.ReflectedType;

        public override MethodInfo GetBaseDefinition() => m_owner.GetBaseDefinition();
        public override object[] GetCustomAttributes(bool inherit) => m_owner.GetCustomAttributes(inherit);
        public override object[] GetCustomAttributes(Type attributeType, bool inherit) => m_owner.GetCustomAttributes(attributeType, inherit);
        public override MethodImplAttributes GetMethodImplementationFlags() => m_owner.GetMethodImplementationFlags();
        public override ParameterInfo[] GetParameters() => m_owner.GetParameters();
        public override object Invoke(object obj, BindingFlags invokeAttr, Binder binder, object[] parameters, CultureInfo culture) =>
            m_owner.Invoke(obj, invokeAttr, binder, parameters, culture);
        public override bool IsDefined(Type attributeType, bool inherit) =>
            m_owner.IsDefined(attributeType, inherit);

        public override MethodBody GetMethodBody() => new ILMethodBody(m_owner.GetILGenerator());
    }
}
