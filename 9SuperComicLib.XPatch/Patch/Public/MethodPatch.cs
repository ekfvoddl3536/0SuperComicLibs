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
