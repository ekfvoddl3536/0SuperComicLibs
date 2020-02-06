using System;
using System.Reflection;
using System.Reflection.Emit;
using SuperComicLib.LowLevel;

namespace Sample_0000
{
    public struct ILGenerator2
    {
        public int length;
        public byte[] ilstream;
        public int[] labels;
        public int labelcount;
        public Array arr_fixupdata;
        public int fixupcount;
        public int[] relocfixuplist;
        public int relocfixupcount;
        public int exceptioncount;
        public int currexcstackcount;
        public Array arr_exceptions;
        public Array arr_currExcStack;
        public object scopeTree;
        public object lineNumber;
        public MethodInfo methodbuilder;
        public int localCount;
        public SignatureHelper signature;
        public int maxStackSize;
        public int maxMidStack;
        public int maxMidStackCur;

        // need "SuperComicLib.Core"
#if SUPERCOMICLIB_CORE
        public override string ToString() => 
            string.Format(" {0,-20} : {1}", "m_ILStream", ilstream.SubArray(0, length).Join(v => v.ToString("X2"))) + Environment.NewLine
            + string.Format(" {0,-20} : {1}", "m_Length", length) + Environment.NewLine
            + string.Format(" {0,-20} : {1}", "m_MaxStackSize", maxStackSize);
#else
        public override string ToString()
        {
            string first = ilstream[0].ToString("X2");
            for (int x = 1; x < length; x++)
                first = first + " " + ilstream[x].ToString("X2");

            return
                string.Format(" {0,-20} : {1}", "m_ILStream", first) + Environment.NewLine
                + string.Format(" {0,-20} : {1}", "m_Length", length) + Environment.NewLine
                + string.Format(" {0,-20} : {1}", "m_MaxStackSize", maxStackSize);
        }
#endif
    }

    public unsafe class Program
    {
        public static void Main(string[] args)
        {
            // introduction
            Console.WriteLine(" === SuperComicLib.LowLevel === ");
            Console.WriteLine(" =========== Sample =========== ");
            Console.WriteLine(Environment.NewLine);

            // prepare
            DynamicMethod dm = new DynamicMethod(string.Empty, typeof(void), new[] { typeof(int).MakeByRefType() });
            ILGenerator il = dm.GetILGenerator();

            // do something
            il.Emit(OpCodes.Ldarga_S, (byte)0);
            il.Emit(OpCodes.Ldind_I4, 400);
            il.Emit(OpCodes.Ret);

            Console.WriteLine(" === Before === ");

            // read memory
            BinaryStructureInfo bsi_bf = new BinaryStructureInfo(NativeClass.ReadMemory(ref il));
            {
                // read field memory
                ILGenerator2 il2_bf = bsi_bf.Read<ILGenerator2>(0);

                // check Before
                Console.WriteLine(il2_bf.ToString());            
                /*
                    m_ILStream           : 0F 00 4A 90 01 00 00 2A
                    m_Length             : 8
                    m_MaxStackSize       : 1
                */

                // overwrite
                il2_bf.ilstream = new byte[32];
                il2_bf.maxStackSize = 32;
                il2_bf.length = 1;

                // apply
                NativeClass.RefMemory(ref il, temp =>
                {
                    byte[] arr = NativeClass.ReadMemory(ref il2_bf);
                    fixed (byte* optr = arr)
                    {
                        NativeClass.Memcpy(optr, (uint)IntPtr.Size, temp, (uint)IntPtr.Size, (uint)(arr.Length - IntPtr.Size));
                    }
                });

            }
            // dispose
            bsi_bf.Dispose();

            Console.WriteLine(Environment.NewLine);
            Console.WriteLine(" === After === ");

            // read memory
            using (BinaryStructureInfo bsi_af = new BinaryStructureInfo(NativeClass.ReadMemory(ref il)))
            {
                // read field memory
                ILGenerator2 il2_af = bsi_af.Read<ILGenerator2>(0);

                // check After
                Console.WriteLine(il2_af.ToString());
                /*
                    m_ILStream           : 00
                    m_Length             : 1
                    m_MaxStackSize       : 32
                */
            } // dispose

            // wait
            Console.ReadLine();
        }
    }
}
