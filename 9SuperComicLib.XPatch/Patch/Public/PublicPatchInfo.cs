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
