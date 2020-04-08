# Basic
## `NativeClass` static class
* Access the object's source memory
  1. Modify readonly fields
  2. Get or modify the value of non-public fields
  3. Get or modify the this pointer value (this is only an example)
* Memory data copy
* Get address of the object
* Get address of the object (pinned)
* Convert class reference pointer value to original type
* Copy memory datas from one object type to another **public struct type[1]**  
  (Only if the field layout is the same or the objects are the same size will the copy be successful)  
  
 
| EndsWith | Meaning | Note |
| :------: | :------: | :------ |
| _s | Safe | this function is safe |
| _c | Const | can use constant value as arguments |
| _cs | Const + Safe | |
| Addrss | Addresses  | get the addresses of the array elements |
| Memcpyff | | not using cpblk command |
  
  
`[1]  Type accessible from external library, regardless of which field is non-public`    
## `PointerMethods<T>` abstract class
this class defines the default behavior.    
### Derived
  * `NativeClass<T>`
  * `NativeStruct<T>`
  
### Methods  
| Name | Return | Parameters | Note |
| :------: | :------: | :------: | :------ |
| Default | `T` | `void*` |  |
| GetAddr | `IntPtr` | `ref T` | Get address of the object |
| GetPinnedPtr |  | `ref T`, `Action<IntPtr>` | Get address of the object (pinned) |

## `NativeClass<T>` sealed class
Cast the reference pointer value to `T`.  
`T` is `class`.  

### Methods
| Name | Return | Parameters | Note |
| :------: | :------: | :------: | :------ |
| Cast | `T` | `void*` | Cast to `T` |
| Default | `T` | `void*` | call `Cast<T>` |
| get_Instance | `NativeClass<T>` |  | static method |

## `NativeStruct<T>` sealed class
Read the data from the memory address to which the pointer value points, and convert it to `T`.  
`T` is `struct`.

### Methods
| Name | Return | Parameters | Note |
| :------: | :------: | :------: | :------ |
| Read | `T` | `void*` | read value |
| Default | `T` | `void*` | call `Read<T>` |
| get_Instance | `NativeStruct<T>` |  | static method |


  
# Extension <1>
## `BinaryStructureInfo` sealed class
Manages copies of object's original memory data separated by `this pointer` and` field datas`.  
You can easily read or write field data using functions such as `Cast<T>`,` Read<T>`, `Ref<T>`, and `Write<T>`.  
  
The modified data must be imported into the `ToArray` function and rewritten in original memory for it to take effect.
### Methods
| Name | Return | Parameters | Note |
| :-: | :-: | :- | :- |
| Constructor |  | `byte[]` |  |
| get_Length | `int` |  | get the lenght of an internal byte array |
| get_TypeHandle | `IntPtr` |  | |
| get_Syncblock | `IntPtr` |  | |
| get_Blank | `IntPtr` |  |  |
| get_Item | `byte&` | `int` | *Indexer* |
| ToArray | `byte[]` | `bool = false` |  |
| FixedPointer |  | `UnsafePointerAction<byte>` | Create a pointer to an internal byte array |
| ReadBytes | `byte[]` | `int`, `int` |  |
| WriteBytes |  | `int`, `byte[]` |  |
| Read | `T` | `<T> : struct`, `int` | when reading the same type multiple times, use `NativeStruct<T>` instead this function |
| Ref | `ref T` | `<T> : unmanaged`, `int` |  |
| Cast | `T` | `<T> : class`, `int` |  |
| Write |  | `<T> : struct`, `T`, `int` |  |
| Write |  | `<T> : struct`, `ref T`, `int` |  |
| Dispose |  |  |  |

   
   
   
   
   
   
# Extension <2>
## `NativeHeapMgr` sealed class
**Windows Only**  
Allocate new virtual memory and manage it yourself.  
Writing a **reference pointer value[2]** in this area is very dangerous, so only write the entire memory data.  
  
      
`[2] such as 'abcd' of "object abcd = new object ()"`
### Methods
| Name | Return | Parameters | Note |
| :-: | :-: | :- | :- |
| Constructor |  | `int` |  |
| Constructor |  | `long` |  |
| get_Size | `int` |  | Return allocated size |
| get_LongSize | `long` |  | Return allocated size |
| ReadValue | `T` | `<T> : unmanaged`, `int` |  |
| WriteValue | `T` | `<T> : unmanaged`, `int`, `T` |  |
| ReadAddrs | `T` | `<T> : class`, `int` | Read address stored in `offset` position and cast to `T` |
| WriteAddrs |  | `int`, `object` | Write the `reference pointer value` |
| SetBlock |  | `int`, `byte[]` | Write the `datas` values |
| ReadBlock | `byte[]` | `int`, `int` |  |
| Free |  | `int`, `int` | Set internal memory data to '0' |
| get_Item | `byte*` | `int` | *Indexer* |
| MemoryBlock | `byte*` |  |  |
| AsPointer | `T*` | `<T> : unmanaged` |  |
| AsPointer | `T*` | `<T> : unmanaged`, `int` |  |
| ToIntPtr | `IntPtr` |  |  |
| Protect | `P.S.H.R` |  | Defending itself from accessing internal memory |
| Readonly | `R.N.S.H.R` |  | Make internal memory read-only |
| Dispose |  |  |  |

### Example
```csharp
...using SuperComicLib.LowLevel;

public class CustomType {...};

static class Program
{
    // This type is non-public type, so using it will throw exception.
    public class ErrorType {...}

    static void Main(string[] args)
    {
        // 16 bytes reserved
        NativeHeapMgr mgr = new NativeHeapMgr(16);
        mgr.WriteAddrs(0, new CustomType(...));
        // An exception occurred when I tested this
        // GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true, true);
        var temp = mgr.ReadAddrs<CustomType>(0);
        mgr.Dispose();
    }
}
```  

## `NativeProtectData<T>` sealed class
**Windows Only**  
This class protects only the `T` size, and the data of `T` is stored in the protected area.  
  
`T` is `unmanaged`. 

### Methods
| Name | Return | Parameters | Note |
| :-: | :-: | :- | :- |
| Constructor |  | `T` |  |
| set_Value |  | `T` | write new value |
| get_Value | `T` |  | read value |
| Dispose |  |  |  |

## `SuperComicLib.Collection` namespace
### `NativeSecureArray<T>` sealed class
**Windows Only**  
This class prevents programs such as `Cheat Engine` from easily reading data.  
This works like a static array, can't change the size later.  
Unprotect when reading/writing data (Only from idx to `T` size), is safer than array.  
  
`T` is `unmanaged`.
### Methods
| Name | Return | Parameters | Note |
| :-: | :- | :- | :- |
| Constructor |  | `int` |  |
| Constructor |  | `T[]` |  |
| GetEnumerator | `IEnumerator<T>` |  |  |
| ToArray | `T[]` |  |  |
| CopyTo |  | `NativeSecureArray<T>` |  |
| get_Length | `int` |  |  |
| get_LongLength | `long` |  |  |
| get_Item | `T` | `int` | Read the value at position `idx` (*Indexer*) |
| set_Item |  | `int`, `T` | Write the new value at position `idx` (*Indexer*) |
| Dispose |  |  |  |


### Example
```csharp
using SuperComicLib.Collection;
using System;
...
// initialize an int array of size 10
NativeSecureArray<int> array = new NativeSecureArray<int>(10);
do
{
    array[0] += 10;
    Console.WriteLine(array[0]);
}
while (Console.ReadKey().Key != ConsoleKey.Escape);
// call Dispose when you no longer use it.
array.Dispose();
...
```
