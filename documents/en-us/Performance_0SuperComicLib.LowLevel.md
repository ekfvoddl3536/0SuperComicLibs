# Reflection Vs. LowLevel
## Summary
### Only `ValueType` Fields
|  | Reflection | LowLevel | Note |
|:--| :--: | :--: | :-- |
| Elapsed Time (s) | 51.5 | **1.996** | lower is better |
| Total Memory Used (GB) | 6.19 | **2.87** | lower is better |
### Mixed `ReferenceType`(e.g.: `string`) and `ValueType`(e.g.: `int`) Fields
#### Reflection without __makeref
|  | Reflection | LowLevel | Note |
|:--| :--: | :--: | :-- |
| Dest Object Type | Class | Struct |  |
| Elapsed Time (s) | 0.9083 | **0.1054** | lower is better |
| Total Memory Used (MB) | 394.88 | **172.93** | lower is better |
#### Reflection with __makeref 
|  | Reflection | LowLevel | Note |
|:--| :--: | :--: | :-- |
| Dest Object Type | **Struct** | Struct |  |
| Elapsed Time (s) | 0.5562 | **0.1054** | lower is better |
| Total Memory Used (MB) | 433.06 | **172.93** | lower is better |
## Details
Original Text (KR): [Blog](https://ekfvoddl3535.blog.me/221783566697)  


### Only `ValueType` Fields
#### Reflection
![img0](https://user-images.githubusercontent.com/42625666/73598874-abe0fa80-4580-11ea-8869-5d65ac039c32.png)
#### LowLevel
![img1](https://user-images.githubusercontent.com/42625666/73598886-d763e500-4580-11ea-8865-0fa86099d8ce.png)
### Mixed `ReferenceType` and `ValueType` Fields
#### Reflection without __makeref
![img2](https://user-images.githubusercontent.com/42625666/73598903-f8c4d100-4580-11ea-8b4e-e22dab2d4705.png)
#### Reflection with __makeref
![img3](https://user-images.githubusercontent.com/42625666/73598920-30337d80-4581-11ea-8dce-d3c53580d3f0.png)
#### LowLevel
![img4](https://user-images.githubusercontent.com/42625666/73598911-0ed29180-4581-11ea-93ef-e73fee4aeeda.png)
#### Source Code
````csharp
#define VS // vs MODE
#define LOWLEVEL // vs MODE - TEST LowLevel
#define OPTIMIZE
#define REFERENCE // include reference type
// #define DEEPCOPY
#define TYPEDREF // using __makeref
using SuperComicLib.LowLevel;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Sample2
{
    public unsafe class Program
    {
        private class Foo
        {
            public int x;
            public int y;
#if REFERENCE
            public string str;
#endif
        }

#if TYPEDREF == false
        private class Foo2
        {
            public int x;
            public int y;
#if REFERENCE
            public string str;
#endif
        }
#endif

        public struct Foo3
        {
            public int x;
            public int y;
#if REFERENCE
            public string str;
#endif
        }

        public static void Main(string[] _)
        {
#if REFERENCE
            Foo source = new Foo { x = 20, y = 50, str = "hello, lowlevel!" };
#else
            Foo source = new Foo { x = 20, y = 50 };
#endif

            const BindingFlags flags = BindingFlags.Static | BindingFlags.NonPublic;
            Type clsType = typeof(Program);
            RuntimeHelpers.PrepareMethod(clsType.GetMethod(nameof(Use_Reflection), flags).MethodHandle);
            RuntimeHelpers.PrepareMethod(clsType.GetMethod(nameof(Use_LowLevel), flags).MethodHandle);

            Console.WriteLine("Wait 3 sec...");
            System.Threading.Thread.Sleep(3000);
#if REFERENCE
            const int SIZE = 10000;
#else
            const int SIZE = 1000000;
#endif
#if VS
            double totalMemoryUsed = 0.0;
            Stopwatch totalSw = new Stopwatch();
            totalSw.Start();
#endif
            Stopwatch sw = new Stopwatch();

            for (int x = 1; x <= 100; x++)
            {
                Console.WriteLine($"  === Test {x} ===");
#if VS
#if LOWLEVEL == false
                sw.Reset();
                sw.Start();
                Use_Reflection(source, SIZE);
                sw.Stop();
                totalMemoryUsed += GC.GetTotalMemory(false);
                
                Console.WriteLine("  Reflection: " + sw.Elapsed.ToString());
#else
                sw.Reset();
                sw.Start();
                Use_LowLevel(source, SIZE);
                sw.Stop();
                totalMemoryUsed += GC.GetTotalMemory(false);

                Console.WriteLine("  LowLevel: " + sw.Elapsed.ToString());
#endif
#else
                sw.Reset();
                sw.Start();
                Use_Reflection(source, SIZE);
                sw.Stop();
                
                Console.WriteLine("  Reflection: " + sw.Elapsed.ToString());

                sw.Reset();
                sw.Start();
                Use_LowLevel(source, SIZE);
                sw.Stop();
                
                Console.WriteLine("  LowLevel: " + sw.Elapsed.ToString());
#endif
                Console.WriteLine(Environment.NewLine);
            }

#if VS
            totalSw.Stop();
            Console.WriteLine("  Final Performance: " + totalSw.Elapsed.ToString());
            Console.WriteLine("  Total Memory Used: " + totalMemoryUsed.ToString());
            Console.WriteLine(Environment.NewLine);
            Console.WriteLine(Environment.NewLine);
#endif

            Console.ReadLine();
        }

#if TYPEDREF == false
#if OPTIMIZE
        static FieldInfo[] destFields = typeof(Foo2).GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        static FieldInfo[] srcFields = typeof(Foo).GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
#endif
        private static Foo2[] Use_Reflection(Foo source, int size)
        {
            Foo2[] result = new Foo2[size];

#if OPTIMIZE == false
            FieldInfo[] destFields = typeof(Foo2).GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            FieldInfo[] srcFields = source.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
#endif

            for (int x = 0; x < size; x++)
            {
                ref Foo2 current = ref result[x];
                current = new Foo2();
                for (int i = 0; i < srcFields.Length; i++)
#if REFERENCE
#if DEEPCOPY
                {
                    object temp = srcFields[i].GetValue(source);
                    if (temp is ICloneable clone)
                        destFields[i].SetValue(current, clone.Clone());
                    else
                        destFields[i].SetValue(current, temp);
                }
#else
                    destFields[i].SetValue(current, srcFields[i].GetValue(source));
#endif
#else
                    destFields[i].SetValue(current, srcFields[i].GetValue(source));
#endif
            }

            return result;
        }
#else
#if OPTIMIZE
        static FieldInfo[] dstFields = typeof(Foo3).GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        static FieldInfo[] srcFields = typeof(Foo).GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        static int fieldLen = srcFields.Length;
#endif
        private static Foo3[] Use_Reflection(Foo source, int size)
        {
            Foo3[] result = new Foo3[size];

            for (int x = 0; x < size; x++)
            {
                TypedReference typed = __makeref(result[x]);
                for (int i = 0; i < fieldLen; i++)
                    dstFields[i].SetValueDirect(typed, srcFields[i].GetValue(source));
            }
            return result;
        }
#endif

        private static Foo3[] Use_LowLevel(Foo source, int size)
        {
            Foo3[] result = new Foo3[size];

#if OPTIMIZE && REFERENCE
            NativeStruct<Foo3> item = new NativeStruct<Foo3>();
            fixed (byte* ptr = NativeClass.ReadMemory(ref source))
            {
                void* nptr = ptr + IntPtr.Size;
                for (int x = 0; x < size; x++)
                    result[x] = item.Read(nptr);
            }
#else
            using (BinaryStructureInfo bsi = new BinaryStructureInfo(NativeClass.ReadMemory(ref source)))
            {
                for (int x = 0; x < size; x++)
#if REFERENCE
                    result[x] = bsi.Read<Foo3>(0);
#else
                    result[x] = bsi.Ref<Foo3>(0);
#endif
            }
#endif

            return result;
        }
    }
}
````
