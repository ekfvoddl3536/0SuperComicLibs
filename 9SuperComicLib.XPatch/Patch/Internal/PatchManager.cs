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
