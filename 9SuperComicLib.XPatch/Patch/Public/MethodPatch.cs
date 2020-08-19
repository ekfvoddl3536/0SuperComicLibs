using System;
using System.Reflection;

namespace SuperComicLib.XPatch
{
    public static class MethodPatch
    {
        public static PublicPatchInfo Load(Assembly asmb) => Load(asmb.GetTypes());
        public static PublicPatchInfo Load(Type[] types)
        {
            PatchManager pm = new PatchManager();
            foreach (Type t in types)
                foreach (MethodInfo meth in t.GetMethods(Helper.mflag2))
                    TryAdd(pm, meth);

            return new PublicPatchInfo(pm);
        }

        public static PublicPatchInfo Empty() => new PublicPatchInfo(new PatchManager());

        internal static void TryAdd(PatchManager mgr, MethodInfo meth)
        {
            if (meth.Name == "Prepare" || meth.GetCustomAttribute<PrepareAttribute>() != null)
                meth.Invoke(null, null);
            else if (meth.GetCustomAttribute<TargetMethodAttribute>() is TargetMethodAttribute attrb)
                mgr.Add(meth, attrb);
            else if (meth.GetCustomAttribute<RegisterPatchMethodAttribute>() != null)
                mgr.AddUnready(meth);
        }
    }
}
