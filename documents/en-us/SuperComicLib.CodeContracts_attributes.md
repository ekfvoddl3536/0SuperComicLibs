# SuperComicLib.CodeContracts `Attributes`
## Index
**WARNING**: This API may be modified or changed without notice.  
  To improve readability, the suffix `Attribute` is not indicated.  

-  AllowEmptyArray
 - AllowNull
 - AssumeInputsValid
 - AssumeOperationValid
 - ConstField
 - DevRelease
 - DisallowEmptyArray
 - DisallowNull
 - DisallowNullOrEmpty
 - KeepAlive
 - NoExcept
 - NoOverhead
 - NotEmptyArray
 - NotNull
 - ValidRange
 - X64LossOfLength
 - X64Only

### AllowEmptyArray
`Array` or `string` marked with this attribute can be `zero-length`.  
Defaults to `AllowNull` unless used with `AllowNull` or `DisallowNull` attribute.  

### AllowNull
`Arguments` or `fields` marked with this attribute are `nullable`.   
By default, it should be used for `nullable` types, but when used for `non-nullable` types, the `default` value is considered acceptable.  

### AssumeInputsValid
Assumed that the argument values are valid.  
By default, it means no validation (null checking, index bounds checking, etc.) for all arguments.  
For `high-performance` scenarios.

### AssumeOperationValid
Assume that the object's state (field data) is valid.  
To reduce confusion, this attribute only considers the state of the object's instance fields of the declared `method` or `property`.  
***Does not contain the state of external objects.***  
For `high-performance` scenarios.  
  
Examples of **incorrect usage**:
```csharp
// !!! INCORRECT USAGE EXAMPLE !!!
// !!! INCORRECT USAGE EXAMPLE !!!

using System;
using SuperComicLib.CodeContracts;

internal static class MyClass2Extern_Incorrect
{
	public static int index;
}

public class MyClass1_Incorrect
{
	private int[]? array;

	...
	
	// 'MyClass2Extern_Incorrect.index' cannot be
	// assumed to be valid and will not be considered.
	[AssumeOperationValid]
	public void Set(int new_value)
	{
		array![MyClass2Extern_Incorrect.index] = new_value;
	}
}
```

### ConstField
Does not change the instance state (field values) of the `class` or `struct` in which the `method` or `property` is declared.  
  
```csharp
using SuperComicLib.CodeContracts;

public class Class2
{
	private int _value;

	[ConstField]
	public void Foo(Class2 item)
	{
		if (item == null || item == this)
			return;
		
		// The value of '_value' must not be modified.
		int k = this._value;

		// Values in other instances can be modified.
		item._value = k;
	}
}
```

### DevRelease
Features marked with this attribute, indicate that they are in development.  

### DisallowEmptyArray
`Array` or `string` marked with this attribute **cannot** be `zero-length`.  
Defaults to `AllowNull` unless used with `AllowNull` or `DisallowNull` attribute.  

### DisallowNull
`Arguments` or `fields` marked with this attribute are **cannot** be `null`.  

### DisallowNullOrEmpty
`Array` or `string` marked with this attribute **cannot** be `zero-length` **AND** **cannot** be `null`.   
Same as: `[DisallowEmptyArray, DisallowNull]`

### KeepAlive
Does not call `Dispose()` for a `field` or `argument` declared as a type that implements `System.IDisposable`.  
  
Example:
```csharp
public static void Foo_keepAlive([KeepAlive] Stream stream)
{
	stream.Read(...);
	stream.Write(...);

	// Do not dispose this object.
	// 	stream.Close();
	// 	-OR-
	// 	stream.Dispose();
}
```

### NoExcept
Indicates that does not throw exceptions.  
Includes any exceptions that could potentially occur.  
  
Constructors list:
| Signature | Description |
| :------- | :--------- |
| `()` | No exceptions of any kind are thrown. |
| `(params System.Type[])` | Only the specified exception types are not thrown. |

### NoOverhead
Indicates not to call other methods declared with the same name.  

### NotEmptyArray
Indicates that the returned `array` or `string` is **non-zero** length.

### NotNull
Indicates that the returned `value` is **non-null**  

###  ValidRange
`Arguments` or `fields` marked with this attribute are in the valid range.  
It is **not recommended** to use it for `fields` as they can have unpredictable ranges.  
  
***By default***, this attribute represents a range from **min (include) to max (exclude)**, which can have different meanings depending on the type of constructor used.  
  
Constructor list:
| Signature | Description |
|:--|:--|
| `()` | Must be in the range 0 (include) to `Length` or `Count` (exclude). It also means that there must be a logical range validity to perform the method. |
| `(int min)` | Must be in the range `min` (include) to `Length` or `Count` (exclude) |
| `(uint min)` | *Same as above description* |
| `(long min)` | *Same as above description* |
| `(ulong min)` | *Same as above description* |
| `(int min, int max, bool includeMax)` | Must be in the range `min` (include) to `max` (include if `includeMax` is `true`; otherwise, exclude) |
| `(uint min, uint max, bool includemax)` | *Same as above description* |
| `(long min, long max, bool includemax)` | *Same as above description* |
| `(ulong min, ulong max, bool includemax)` | *Same as above description* |
| `(string custom_description)` | If complex conditions exist, can include a custom description |
  
Default constructors can have a wider meaning than others.  
```csharp
// 'ValidRange' assumes the condition to be 'true', 
// so use it together with 'AssumeInputsValid'.
[AssumeInputsValid]
public static void CopyTo([DisallowNullOrEmpty] int[] src, [DisallowNullOrEmpty, ValidRange] int[] dest)
{
	// The following conditions are assumed to be 'true'.
	// src:
	//	1. Not null
	//	2. Not empty
	// dest:
	//	1. Not null
	//	2. Not empty
	//	3. Equal to or greater than 'src.Length'
	Foo_copy(src, 0, dest, 0, src.Length);
}
```

### X64LossOfLength
Indicates that a loss occurred due to converting a `64-bit` integer to `32-bit`.    

```csharp
// x64 environment
[X64LossOfLength]
public static int Foo_lossOfLength(NativeSpan<byte> value)
{
	long length = value.Length;
	return (int)length;
}
```

### X64Only
Indicates that `method` or `property` is **64-bit only**.   

```csharp
[X64Only]
public static long Foo_x64Only(long a, long b)
{
	return a * b;
}
```
