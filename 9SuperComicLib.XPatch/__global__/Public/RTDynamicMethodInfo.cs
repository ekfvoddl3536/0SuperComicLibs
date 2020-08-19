using System;
using System.Globalization;
using System.Reflection;
using System.Reflection.Emit;

namespace SuperComicLib.XPatch
{
    public sealed class RTDynamicMethodInfo : MethodInfo
    {
        private DynamicMethod m_owner;

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
