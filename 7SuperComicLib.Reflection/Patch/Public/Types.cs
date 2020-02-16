using System;
using System.Reflection;

namespace SuperComicLib.Reflection.Patch
{
    [Flags]
    public enum PatchMode : byte
    {
        None = 0,
        Prefix = 1,
        Postfix,
        Replace,
        ILOnly = 4
    }

    [AttributeUsage(AttributeTargets.Method)]
    public sealed class PrepareAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Method)]
    public sealed class TargetMethodAttribute : Attribute
    {
        internal Type DeclaringType { get; }
        internal string MethodName { get; }
        internal Type[] ArgumentTypes { get; }
        internal PatchMode Mode { get; }
        internal bool IsILOnly { get; }

        internal MethodInfo GetMethod() =>
            ArgumentTypes == null
                ? DeclaringType.GetMethod(MethodName, Helper.flag1)
                : DeclaringType.GetMethod(MethodName, Helper.flag1, null, ArgumentTypes, null);

        public TargetMethodAttribute(Type targetClass, string methodName, PatchMode patchMode)
        {
            DeclaringType = targetClass;
            MethodName = methodName;
            Mode = patchMode & PatchMode.Replace;
            IsILOnly = (patchMode & PatchMode.ILOnly) != PatchMode.None;
        }

        public TargetMethodAttribute(Type targetClass, string methodName, PatchMode patchMode, params Type[] argumentTypes)
        {
            DeclaringType = targetClass;
            MethodName = methodName;
            ArgumentTypes = argumentTypes;
            Mode = patchMode & PatchMode.Replace;
            IsILOnly = (patchMode & PatchMode.ILOnly) != PatchMode.None;
        }
    }

#if true
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class RegisterPatchMethodAttribute : Attribute { }

#pragma warning disable
    public readonly struct TargetMethodInfo
    {
        public readonly MethodBase oldMethod;
        public readonly MethodInfo newMethod;
        public readonly PatchMode mode;

        public TargetMethodInfo(MethodBase oldMethod, MethodInfo newMethod, PatchMode mode)
        {
            this.oldMethod = oldMethod;
            this.newMethod = newMethod;
            this.mode = mode;
        }

        public TargetMethodInfo(MethodBase oldMethod, MethodInfo newMethod) : this(oldMethod, newMethod, PatchMode.Replace) { }

        internal bool IsReady() =>
            oldMethod == null || 
            newMethod == null ||
            mode == PatchMode.None;

        internal bool IsILOnly => (mode & PatchMode.ILOnly) != 0;

        internal PatchMode GetMode => mode & PatchMode.Replace;
    }
#pragma warning restore
}
#endif
