using System;
using System.Collections.Generic;
using SuperComicLib.Core;

namespace SuperComicLib.CodeDesigner
{
    public static partial class TypeTable
    {
        private static readonly Type[] primitive =
        {
            CTypes.BOOL_T,
            CTypes.SBYTE_T,
            CTypes.BYTE_T,
            CTypes.SHORT_T,
            CTypes.INT_T,
            CTypes.LONG_T,

            UCTypes.SHORT_T,
            UCTypes.INT_T,
            UCTypes.LONG_T,

            CTypes.FLOAT_T,
            CTypes.DOUBLE_T
        };
        private static readonly string[] names =
        {
            "bool",
            "sbyte",
            "byte",
            "short",
            "int",
            "long",

            "ushort",
            "uint",
            "ulong",

            "float",
            "double"
        };

        public static ITypeMap Instance = new ConcurrentTypeMap();
        public static event ITypeMapChangedHandler ChangedEvent;

        public static void InitDefault() => InitDefault(Instance);

        private static void InitDefault(ITypeMap inst)
        {
            Type[] v1 = primitive;
            string[] v2 = names;

            int x = v1.Length;

            while (--x >= 0)
                if (inst.Contains(v2[x]) == false)
                    inst.Add(v2[x], v1[x]);
        }

        public static void Add<T>(string name) => Instance.Add<T>(name);

        public static void Add(string name, Type value) => Instance.Add(name, value);

        public static bool Contains(string name) => Instance.Contains(name);

        public static bool TryGet(string name, out Type result) => Instance.TryGet(name, out result);

        public static IEnumerable<Type> ToArray() => Instance.ToArray();

        public static ITypeMap Current => Instance;

        public static void Change(ITypeMap newinst) => Change(null, newinst);

        public static void Change(object sender, ITypeMap newinst)
        {
            ITypeMap old = Instance;
            if (old == newinst)
                throw new ArgumentException();

            Instance = newinst ?? throw new ArgumentNullException(nameof(newinst));

            ITypeMapChangedArgs args = new ITypeMapChangedArgs(old, newinst);
            ChangedEvent.Invoke(sender, args);

            if (args.AutoDisposing)
                if (old is IDisposable v1)
                    v1.Dispose();
                else if (old is IRuntimeTypeMapEX v2)
                    v2.Dispose();
        }

#pragma warning disable IDE0004
        public static ITypeMap New(bool isThreadSafe, bool initDefault)
        {
            ITypeMap result = 
                isThreadSafe
                ? (ITypeMap)new SafeRuntimeTypeMap()
                : (ITypeMap)new ConcurrentTypeMap();

            if (initDefault)
                InitDefault(result);

            return result;
        }

        internal static IRuntimeTypeMapEX New(bool isThreadSafe) =>
            isThreadSafe
            ? (IRuntimeTypeMapEX)new SafeRuntimeTypeMap()
            : (IRuntimeTypeMapEX)new ConcurrentTypeMap();
#pragma warning restore IDE0004
    }
}
