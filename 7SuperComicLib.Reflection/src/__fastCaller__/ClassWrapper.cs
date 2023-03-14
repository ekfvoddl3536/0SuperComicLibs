using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Serialization;
using System.Text;
using SuperComicLib.Collections;

namespace SuperComicLib.Runtime
{
    public static class ClassWrapper
    {
        private const BindingFlags flag_i = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
        private const BindingFlags flag_s = BindingFlags.Public | BindingFlags.Static;
        private const BindingFlags flag_sa = flag_s | BindingFlags.NonPublic;
        private const TypeAttributes tattrb = TypeAttributes.Public | TypeAttributes.Sealed;
        private const MethodAttributes mattrb = MethodAttributes.Public | MethodAttributes.Final | MethodAttributes.NewSlot | MethodAttributes.Virtual;
        private const CallingConventions cc = CallingConventions.HasThis;
        private static readonly Dictionary<Type, object> cached = new Dictionary<Type, object>();
        private static readonly ModuleBuilder mdb =
            AssemblyBuilder.DefineDynamicAssembly(
                new AssemblyName("7SuperComicLib.Reflection.FastCaller"),
                AssemblyBuilderAccess.RunAndCollect)
            .DefineDynamicModule("ClassWrapperMOD");

        public static T Make<T>(TypeBuilder tb) where T : class
        {
            try
            {
                return Make<T>(tb.CreateType());
            }
#pragma warning disable
            catch
            {
                return null;
            }
#pragma warning restore
        }

        /// <exception cref="Exception">Thrown when TargetField attribute cannot be found in the property</exception>
        /// <exception cref="InvalidOperationException">Thrown when try to invalid operation</exception>
        /// <summary>
        /// Create an interface implementation for dynamic type <paramref name="t"/>
        /// </summary>
        /// <typeparam name="T">Interface type</typeparam>
        /// <param name="t">
        /// Find only public static fields and methods
        /// Using static types(as opposed to dynamic types) is inefficient
        /// </param>
        /// <returns>Is not null if the method succeeds</returns>
        public static T Make<T>(Type t) where T : class
        {
            Type temp = typeof(T);
            if (!temp.IsInterface || temp.IsNotPublic || t == null || !(t.IsAbstract && t.IsSealed))
                return null;
            if (cached.TryGetValue(t, out object obj))
                return (T)obj;

            PropertyInfo[] implPpt = temp.GetProperties(flag_i);
            SpMI[] impl = Array.ConvertAll(temp.GetMethods(flag_i), CONV_SPI);
            OnlyMI[] code = Array.ConvertAll(t.GetMethods(flag_s), CONV_OMI);

            int init_x = impl.Length;
            int init_p = implPpt.Length;
            if (init_x == 0 && init_p == 0)
                return null;

            CHashSet<int> xdone =
                init_x == 0
                ? null
                : new CHashSet<int>(init_x);

            StringBuilder sb = new StringBuilder(t.FullName);
            sb.Append("__ClassWrapper__");
            sb.Append(cached.Count);

            TypeBuilder tb = mdb.DefineType(sb.ToString(), tattrb, null, new Type[1] { temp });

            int clen = code.Length;
            int z = 0;
            for (; z < init_x; z++)
            {
                SilmMI k = impl[z];
                int x = clen;

                while (--x >= 0)
                    if (code[x].Equals(k) && xdone.Add(x))
                    {
                        SomeMethod(tb, k, code[x]);
                        break;
                    }
            }
            if (xdone != null && init_x != xdone.Count)
                throw new InvalidOperationException("Some methods could not be implemented", GETMOREINFO(temp, init_x, xdone));

            for (z = 0; z < init_p; z++)
                if (implPpt[z].GetCustomAttribute<TargetFieldAttribute>() is TargetFieldAttribute tfa)
                {
                    PropertyInfo pp = implPpt[z];
                    temp = pp.PropertyType;
                    if (tfa.refm ^ temp.IsByRef)
                        throw new InvalidOperationException("Incorrect attribute declare", GETMOREINFO(temp.IsByRef));
                    else if (pp.GetSetMethod() != null)
                        throw new InvalidOperationException("use ref return type instead of set accessor", GETMOREINFO());

                    if (t.GetField(pp.GetCustomAttribute<TargetFunctionAttribute>()?.name ?? tfa.name, flag_sa) is FieldInfo fd)
                        FieldMethod(tb, pp.GetGetMethod(false), fd, tfa.refm);
                    else
                        throw new InvalidOperationException($"Field not found: {tfa.name}");
                }
                else
                    throw new Exception("Missing attribute");

            obj = FormatterServices.GetUninitializedObject(tb.CreateType());
            cached.Add(t, obj);

            return (T)obj;
        }

        private static void SomeMethod(TypeBuilder tb, SilmMI nb, OnlyMI target)
        {
            Type[] vs = nb.ParaTypes;
            ILGenerator g = tb.DefineMethod(nb.Name, mattrb, cc, nb.RetType, vs).GetILGenerator();

            int max = nb.ParaLength;
            for (int x = 0; x < max;)
                if (vs[x].IsByRef)
                    g.Emit_Ldarga(++x); // instance
                else
                    g.Emit_Ldarg(++x);

            g.Emit(OpCodes.Call, target.Info);
            g.Emit(OpCodes.Ret);
        }

        private static void FieldMethod(TypeBuilder tb, MethodInfo mi, FieldInfo fd, bool byref)
        {
            ILGenerator g = tb.DefineMethod(mi.Name, mattrb, cc, mi.ReturnType, null).GetILGenerator();
            g.Emit(
                byref
                    ? OpCodes.Ldsflda
                    : OpCodes.Ldsfld,
                fd);
            g.Emit(OpCodes.Ret);
        }

        private static Exception GETMOREINFO(Type t, int size, CHashSet<int> xdone)
        {
            StringBuilder sb = new StringBuilder(4096);
            sb.AppendLine("------- BEGIN INFORMATION -------");

            MethodInfo[] vs = t.GetMethods(flag_i);

            int len = vs.Length - 1;
            int x = 0;
            for (; x < len; x++)
                if (!xdone.Contains(x))
                {
                    sb.AppendLine(vs[x].ToString());
                    sb.AppendLine();
                }
            if (!xdone.Contains(len))
                sb.AppendLine(vs[len].ToString());

            sb.AppendLine("------- END INFOMATION -------");

            return
                new Exception($"Number of methods not implemented: {size - xdone.Count}",
                    new Exception(sb.ToString()));
        }

        private static Exception GETMOREINFO(bool byref) =>
            byref
            ? new Exception("'refMode' of <TargetFieldAttribute> was FALSE")
            : new Exception("current property must be declared as a 'ref return type' by <TargetFieldAttribute> (e.g.: 'ref int Name { get; }')");

        private static Exception GETMOREINFO() =>
            new Exception("set refMode to try using the 'TargetField(string, bool)' or 'TargetField(bool)' constructor");

        private static OnlyMI CONV_OMI(MethodInfo mi) => new OnlyMI(mi);

        private static SpMI CONV_SPI(MethodInfo mi) => new SpMI(mi);

        private abstract class SilmMI
        {
            public readonly string Name;
            public readonly Type RetType;
            public readonly Type[] ParaTypes;
            public readonly int ParaLength;

            protected SilmMI(MethodInfo m)
            {
                Name = m.Name;
                RetType = m.ReturnType;
                ParaTypes = Array.ConvertAll(m.GetParameters(), CONV);
                ParaLength = ParaTypes.Length;
            }

            public virtual string VName => Name;

            public bool Equals(SilmMI other)
            {
                if (VName != other.VName ||
                    RetType != other.RetType ||
                    ParaLength != other.ParaLength)
                    return false;

                int x = ParaLength;
                Type[] me = ParaTypes;
                Type[] ot = other.ParaTypes;
                while (--x >= 0)
                    if (me[x] != ot[x])
                        return false;

                return true;
            }

            private static Type CONV(ParameterInfo p) => p.ParameterType;
        }

        private sealed class OnlyMI : SilmMI
        {
            public readonly MethodInfo Info;

            public OnlyMI(MethodInfo m) : base(m) => Info = m;
        }

        private sealed class SpMI : SilmMI
        {
            public readonly string spName;

            public SpMI(MethodInfo m) : base(m)
            {
                if (m.GetCustomAttribute<TargetFunctionAttribute>() is TargetFunctionAttribute attrb1)
                {
                    if (string.IsNullOrWhiteSpace(attrb1.name))
                        throw new Exception("target function name cannot be NULL or Empty or Whitespace");

                    spName = attrb1.name;
                }
            }

            public override string VName => spName ?? Name;
        }
    }
}
