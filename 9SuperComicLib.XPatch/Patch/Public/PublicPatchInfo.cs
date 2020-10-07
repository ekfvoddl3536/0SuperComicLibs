using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace SuperComicLib.XPatch
{
    public struct PublicPatchInfo : IDisposable
    {
        private PatchManager instance;

        internal PublicPatchInfo(PatchManager instance) => this.instance = instance;

        public void Add(MethodBase oldMeth, MethodInfo newMeth, PatchMode mode)
        {
            if (oldMeth == null)
                throw new ArgumentNullException(nameof(oldMeth), $"{nameof(oldMeth)} cannot be null");
            if (newMeth == null || newMeth.IsStatic == false)
                throw new ArgumentNullException(nameof(newMeth), $"{nameof(newMeth)} cannot be null or instance type");

            instance.Add(oldMeth, newMeth, mode & PatchMode.Replace, mode > PatchMode.Replace);
        }

        public void TryAdd(MethodInfo patchMethod)
        {
            if (patchMethod.IsStatic)
                MethodPatch.TryAdd(instance, patchMethod);
        }

        public void TryAdd(Type[] types)
        {
            foreach (Type t in types)
                foreach (MethodInfo meth in t.GetMethods(Helper.mflag2))
                    MethodPatch.TryAdd(instance, meth);
        }

        public List<DynamicMethod> PatchAll() => instance.PatchAll();

        public void Dispose()
        {
            if (instance != null)
            {
                instance.Dispose();
                instance = null;
            }
        }

        public override bool Equals(object obj) => false;
        public override int GetHashCode() => 0;
        public static bool operator ==(PublicPatchInfo left, PublicPatchInfo right) => left.Equals(right);
        public static bool operator !=(PublicPatchInfo left, PublicPatchInfo right) => !left.Equals(right);
    }
}
