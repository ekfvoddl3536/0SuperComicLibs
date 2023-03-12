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
using System.Reflection;

namespace SuperComicLib.XPatch
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
        internal Type[] GenericArguments { get; }
        internal PatchMode Mode { get; }
        internal bool IsILOnly { get; }

        internal MethodInfo GetMethod()
        {
            MethodInfo meth =
                ArgumentTypes == null
                    ? DeclaringType.GetMethod(MethodName, Helper.flag1)
                    : DeclaringType.GetMethod(MethodName, Helper.flag1, null, ArgumentTypes, null);

            return
                meth.IsGenericMethod && GenericArguments != null && GenericArguments.Length > 0
                ? meth.MakeGenericMethod(GenericArguments)
                : meth;
        }

        public TargetMethodAttribute(Type targetClass, string targetMethodName, PatchMode patchMode)
        {
            DeclaringType = targetClass;
            MethodName = targetMethodName;
            Mode = patchMode & PatchMode.Replace;
            IsILOnly = (patchMode & PatchMode.ILOnly) != PatchMode.None;
        }

        public TargetMethodAttribute(Type targetClass, string targetMethodName, PatchMode patchMode, params Type[] argumentTypes)
        {
            DeclaringType = targetClass;
            MethodName = targetMethodName;
            ArgumentTypes = argumentTypes;
            Mode = patchMode & PatchMode.Replace;
            IsILOnly = (patchMode & PatchMode.ILOnly) != PatchMode.None;
        }

        public TargetMethodAttribute(Type targetClass, string targetMethodName, Type[] genericArguemtns, PatchMode patchMode, params Type[] argumentTypes) : this(targetClass, targetMethodName, patchMode, argumentTypes) =>
            GenericArguments = genericArguemtns;
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
