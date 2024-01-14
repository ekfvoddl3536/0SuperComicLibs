//  
//  다음 라이브러리에 대한 예제가 포함됩니다:
//      0SuperComicLib.Core
//      1SuperComicLib.Runtime
//      2SuperComicLib.Runtime.Managed

using System;
using SuperComicLib;
using SuperComicLib.Runtime;

namespace Example_koKR
{
    internal unsafe static class Program
    {
        public static void Main(string[] _)
        {
            // SuperComicLib.CMath 정적 클래스는 조건 분기 없이
            // 다양한 산술, 논리 연산을 수행하는 것을 목표로 합니다.
            // 
            // 조건 분기가 없으므로 대부분의 경우에 (System.Math의 함수들 대비) 더 높은 성능을 갖습니다.

            int minimum_value = CMath.Min(10, -10);

            int normal_value = CMath.Normal(minimum_value);

            Console.WriteLine("Min(10, -10) -> " + minimum_value);
            Console.WriteLine("Min(10, -10) 연산 결과에 숫자의 방향성만 남김 -> " + normal_value);



            // SuperComicLib.Runtime 네임스페이스 #1
            // 1. 1SuperComicLib.Runtime.dll
            //
            //  .NET C# 에서 일반적으로 수행 불가능한 여러가지 특수 행위를 할 수 있도록 돕습니다.
            //
            //  대표적으로, 배열 참조를 수행할 때 조건 분기가 발생하지 않도록 할 수 있습니다.

            int[] array = new int[200];

            //  array[100]을 수행할 때, 실제로는 (100 < array.Length) 조건을 먼저 확인합니다.
            //  따라서 if 코드를 삽입하지 않았더라도 if 코드가 자동으로 삽입됩니다.
            //  if 코드가 삽입되는 시점이 컴파일 시점이 아니라 런타임의 JIT 컴파일 시점이므로
            //  평범한 방법으로는 if 코드가 삽입되는 것을 막을 수 없습니다.
            int element = array[100];

            // Mono 프레임워크를 사용하여 프로그램을 실행한다면,
            // refdata_clr 대신 refdata_mono 를 사용해야 합니다.
            //
            // 이렇게 하면 실행 시간에 if 코드가 삽입되지 않습니다.
            // 또한, call 코드도 삽입되지 않고 (높은 확률로) 인라인되기 때문에
            // 고성능 시나리오에서 유용하게 사용될 수 있습니다.
            //
            // 고성능 시나리오에서 사용가능한 특별한 옵션을 PerformanceArray 메소드에서 설명합니다.
            int element_ifLess = array.refdata_clr(100L);

            // Mono 구현체를 타겟으로 하는 예시
            // int element_ifLess_mono = array.refdata_mono(100L);

            // ILUnsafe 정적 클래스는 System.Runtime.CompilerServices.Unsafe에서 영감을 받아 만들어진 것으로
            // System.Runtime.CompilerServices.Unsafe와 마찬가지로 다양한 MSIL 수준의 기능을 제공합니다.
            //
            // 포함된 모든 기능이 .NET 전문가용 입니다.
            
            // 관리되는 객체의 주소를 가져옵니다.
            void* arrayObjectPointer = ILUnsafe.AsPointer(array);

            // 관리되지 않는 포인터의 주소 값을 사용하여, 관리되는 객체 참조로 변환합니다.
            int[] castedArray = ILUnsafe.AsClass<int[]>(arrayObjectPointer);

            // nint_t와 nuint_t는 native int 및 native uint를 각각 타겟팅합니다.
            // 각각 IntPtr 및 UIntPtr과 변환이 자유로우며, 산술, 비트, 논리 연산이 가능합니다.
            // 최신 .NET에서 사용가능한 nint와 nuint의 기능적인 측면에서 동일합니다.
            nint_t nativeInt_value = 100;

            // 산술 연산이 되는 모습
            nint_t operation = nativeInt_value * 100;

            // IntPtr로 자유로운 변환
            IntPtr intptr_value = operation;

            // IntPtr에서 자유로운 변환
            nint_t cast1 = intptr_value;

            Console.WriteLine();
            Console.WriteLine("nint_t (100 * 100) 출력 : " + intptr_value);



            // SuperComicLib.Runtime 네임스페이스 #2
            // 2. 2SuperComicLib.Runtime.Managed.dll
            //
            //  1SuperComicLib.Runtime.dll을 확장하여, 관리되는 코드(C#)에서 개발된 라이브러리.
            //  다양한 클래스, 구조체 및 기능이 포함돼있습니다.
            //
            //  대표적으로 '반관리 배열'(semi-managed array)가 있으며, NativeSpan 및 NativeConstSpan,
            //  구 SuperComicLib.LowLevel 네임스페이스에 있던 NativeClass 등이 있습니다.
            //
            //  이 라이브러리에서는 .NET 구현체 종류를 구분하지 않고 사용가능한 몇가지 API도 포함합니다.

            // .NET 구현체가 CoreCLR (.NET Fremework, .NET 계열)인지 Mono 인지 관계없이,
            // 이렇게 사용할 수 있습니다.
            arrayref<int> semiManagedArray = new arrayref<int>(120);

            // 반관리 배열은 C++ STL 규칙을 참고하여, 배열에 인덱서로 직접적으로 접근할 때 범위 검사가 수행되지 않습니다.
            // 즉, 고성능 시나리오에서 유용하게 사용할 수 있습니다.
            semiManagedArray[10] = 100;

            // 배열 범위 검사를 사용하여 안전하게 접근하고자 한다면, 다음과 같이 at 메소드를 사용할 수 있습니다.
            semiManagedArray.at(20) = 250;

            Console.WriteLine();
            Console.WriteLine("반관리 배열 index: 10 = " + semiManagedArray[10]);
            Console.WriteLine("반관리 배열 index: 20 = " + semiManagedArray[20]);

            // 반관리 배열은 관리 배열로 캐스팅이 쉽습니다.
            // 이때, 새로운 관리 배열을 만드는 것이 아닌 반관리 배열의 참조를 관리 배열 참조로 전달하는 것이 전부이므로
            // 이 과정에서 참조를 복사하는 것 이 외, 아무런 비용도 발생하지 않습니다.
            int[] managedArray = semiManagedArray.AsManaged();

            managedArray[10] = -100;

            // -100이 출력됩니다.
            Console.WriteLine();
            Console.WriteLine("관리 배열 변환 후 set 작업 후 ->");
            Console.WriteLine("반관리 배열 index: 10 = " + semiManagedArray[10]);
            Console.WriteLine("반관리 배열 index: 20 = " + semiManagedArray[20]);

            // 반관리 배열은 비관리 배열의 특징인 주소 고정 특성도 갖고 있으므로, fixed를 하지 않아도 됩니다.
            // AsSpan() 확장 메소드는 반관리 배열의 요소 타입 T가 unmanaged 특성을 갖고 있을 때에만 사용 가능합니다.
            NativeSpan<int> span = semiManagedArray.AsSpan();

            // 이렇게 포인터를 직접 얻을 수도 있습니다.
            // 이 포인터는 배열의 첫번째 요소의 위치를 가르킵니다.
            int* pointer = span.Source;

            // NativeSpan<>의 인덱서도 arrayref<>와 마찬가지로 C++ STL의 규칙을 참고하므로
            // 인덱서에서 범위 검사가 수행되지 않습니다.
            span[10] = 33;

            // 마찬가지로 at 메소드를 사용하면 범위 검사가 수행됩니다.
            span.at(20) = -1024;

            // Cast<> 메소드를 사용하면 요소 타입을 바꿀 수 있습니다.
            NativeSpan<byte> byteSpan = span.Cast<byte>();

            Console.WriteLine();
            Console.WriteLine("비관리 배열 변환 후 set 작업 후 ->");
            Console.WriteLine("반관리 배열 index: 10 = " + semiManagedArray[10]);
            Console.WriteLine("반관리 배열 index: 20 = " + semiManagedArray[20]);

            // 반관리 배열은 자동으로 메모리 수거가 되지 않으므로, 직접 Dispose()를 호출해서 메모리를 정리해야 합니다.
            semiManagedArray.Dispose();


            // 메모리 수거에 대한 스트레스 없이 사용하려면, SafeArrayref 클래스의 사용도 고려해볼 수 있습니다.
            SafeArrayref<int> safeSemiMaangedArray = new SafeArrayref<int>(140);

            // SafeArrayref는 '관리형 객체'에 더 가깝다고 평가하기 때문에, 여기서는 일반적인 C# 관리 배열과 같이
            // 인덱서로 접근하면 전달된 인수의 범위 검사가 수행됩니다.
            safeSemiMaangedArray[0] = 10;

            // at_fast 메소드를 사용하면, 범위 검사 없이 수행됩니다.
            safeSemiMaangedArray.at_fast(1) = 20;

            // arrayref와 마찬가지로 요소 타입 T가 unmanaged인 경우 AsSpan() 확장 메소드를 사용할 수 있습니다.
            NativeSpan<int> span2 = safeSemiMaangedArray.AsSpan();

            PerformanceArray(array);

            Console.WriteLine(Environment.NewLine);
            Console.WriteLine("계속하려면 아무 키나 누릅니다...");
            Console.ReadLine();
        }

        public static void PerformanceArray(int[] array)
        {
            //  1SuperComicLib.Runtime.dll
            //  2SuperComicLib.Runtime.Managed.dll
            //  위 두 라이브러리가 모두 참조 추가돼있다면, 다음과 같이 refdata()를 사용할 수 있습니다.
            //
            //  이 확장 메소드는 .NET 구현체 종류에 상관 없이 첫번째 요소의 참조를 반환합니다.

            // 현재 .NET 구현체가 Mono인지 검사하는 if 코드가 내장돼있긴 하지만,
            // 사용할 때 .NET 구현체 종류를 신경쓰지 않을 수 있다는 장점이 있습니다.
            // ref로 선언하여 참조만 저장합니다.
            ref int first = ref array.refdata();

            // ILUnsafe.Add<T>(ref T, int)를 사용하여, 배열의 끝에 대한 참조를 얻습니다.
            ref int end = ref ILUnsafe.Add(ref first, array.Length);

            for (ref int iter = ref first; !ILUnsafe.AreSame(iter, end); iter = ref ILUnsafe.Increment(ref iter))
            {
                // 배열의 요소가 하나씩 열거됩니다.
            }

            // 9번째 인덱스의 요소를 참조하여, 수정합니다.
            ILUnsafe.Add(ref first, 9) = 20;

            Console.WriteLine();
            Console.WriteLine("수정된 값. 관리 배열, index: 9 -> " + array[9]);
        }
    }
}
