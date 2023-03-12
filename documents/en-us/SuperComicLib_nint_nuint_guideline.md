## Pointer operations
```csharp
using System;
using SuperComicLib;
...
public static int Sum(int* ptr, nint_t count)
{
	int getItem = ptr[count - 1];
	//                ↑
	//                ERROR: CS0266

	int solution = ptr[(long)(count - 1)]
	//                   ↑
	//                For 32-bit compatibility, convert after calculation.	  

	int _32bitProblem = ptr[(long)count - 1]
	// This operation is slower in 32-bit applications, as it performs 
	// the operation between two 64-bit integers.

	int sum = 0;
	for (nint_t x = 0; x < count; ++x)
		sum += ptr[(long)x];

	return sum;
}
```
  
## Using `checked`
```csharp
using System;
using SuperComicLib;
...
public static int CheckedExmaple(nuint_t x) {
	return checked((int)x);
	//              ↑
	//              This operation is equivalent to unchecked.
	//              Currently, it has no effect.

	return checked((int)(ulong)x);
	//                    ↑
	//              Converting to 64-bit integer before converting
	//              to 32-bit integer solves the problem.
}
```

