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
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace SuperComicLib.XPatch
{
    internal sealed class PatchManager : IDisposable
    {
        internal Dictionary<MethodBase, PatchInfo> patchinfos = new Dictionary<MethodBase, PatchInfo>();

        internal PatchManager() { }

        public void Add(MethodInfo patcher, TargetMethodAttribute attr) =>
            Add(attr.GetMethod(), patcher, attr.Mode, attr.IsILOnly);

        public void AddUnready(MethodInfo patcher)
        {
            TargetMethodInfo meth = (TargetMethodInfo)patcher.Invoke(null, null);
            if (meth.IsReady())
                return;

            Add(meth.oldMethod, meth.newMethod, meth.GetMode, meth.IsILOnly);
        }

        public void Add(MethodBase meth, MethodInfo patcher, PatchMode mode, bool ilonly)
        {
            if (meth.GetCustomAttribute<ILBranchableAttribute>() == null)
                if (patchinfos.TryGetValue(meth, out PatchInfo value))
                {
                    if (mode == PatchMode.Replace)
                        value.replace = ilonly ? new ILOnlyPatchMethod(patcher) : (ExMethodInfo)new ReplaceMethodInfo(patcher);
                    else if (mode == PatchMode.Prefix)
                        value.prefixes.Add(ilonly ? new ILOnlyPatchMethod(patcher) : new ExMethodInfo(patcher));
                    else
                        value.postfixes.Add(ilonly ? new ILOnlyPatchMethod(patcher) : new ExMethodInfo(patcher));
                }
                else
                {
                    value = new PatchInfo(meth);
                    if (mode == PatchMode.Replace)
                        value.replace = ilonly ? new ILOnlyPatchMethod(patcher) : (ExMethodInfo)new ReplaceMethodInfo(patcher);
                    else if (mode == PatchMode.Prefix)
                        value.prefixes.Add(ilonly ? new ILOnlyPatchMethod(patcher) : new ExMethodInfo(patcher));
                    else
                        value.postfixes.Add(ilonly ? new ILOnlyPatchMethod(patcher) : new ExMethodInfo(patcher));

                    patchinfos[meth] = value;
                }
        }

        public List<DynamicMethod> PatchAll()
        {
            try
            {
                List<DynamicMethod> rets = new List<DynamicMethod>();
                foreach (PatchInfo p in patchinfos.Values)
                    rets.Add(p.DoPatch());

                return rets;
            }
#pragma warning disable
#if DEBUG
            catch (Exception exc)
            {
                System.Diagnostics.Debug.WriteLine(exc.ToString());
                System.Diagnostics.Debugger.Break();
#else
            catch
            {
#endif
                return null;
            }
#pragma warning restore
        }

        public void Dispose()
        {
            if (patchinfos != null)
            {
                foreach (PatchInfo p in patchinfos.Values)
                    p.Dispose();

                patchinfos.Clear();
                patchinfos = null;
            }
            GC.SuppressFinalize(this);
        }
    }
}
