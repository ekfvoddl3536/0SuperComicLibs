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
* Copy memory datas from one object type to another public struct type  
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
  
### Functions  
| Name | Return | Parameters | Note |
| :------: | :------: | :------: | :------ |
| Default | `T` | `void*` |  |
| GetAddr | `IntPtr` | `ref T` | Get address of the object |
| GetPinnedPtr |  | `ref T`, `Action<IntPtr>` | Get address of the object (pinned) |

## `NativeClass<T>` sealed class
Cast the reference pointer value to `T`.  
`T` is `class`.  

### Functions
| Name | Return | Parameters | Note |
| :------: | :------: | :------: | :------ |
| Cast | `T` | `void*` | Cast to `T` |
| Default | `T` | `void*` | call `Cast<T>` |

## `NativeStruct<T>` sealed class
Read the data from the memory address to which the pointer value points, and convert it to `T`.  
`T` is `struct`.

### Functions
| Name | Return | Parameters | Note |
| :------: | :------: | :------: | :------ |
| Read | `T` | `void*` | read value |
| Default | `T` | `void*` | call `Read<T>` |


  
# Extension <1>
## `BinaryStructureInfo` sealed class
Manages copies of object's original memory data separated by `this pointer` and` field datas`.  
You can easily read or write field data using functions such as `Cast<T>`,` Read<T>`, `Ref<T>`, and `Write<T>`.  
  
The modified data must be imported into the `ToArray` function and rewritten in original memory for it to take effect.
### Functions
| Name | Return | Parameters | Note |
| :-: | :-: | :- | :- |
| Constructor |  | `byte[]` |  |
| get_Length | `int` |  | get the lenght of an internal byte array |
| get_This | `IntPtr` |  | get this pointer address |
| get_Blank | `IntPtr` |  |  |
| get_Item | `byte&` | `int` |  |
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
## `NativeProtectData<T>` sealed class
**Windows Only**
## `SuperComicLib.Collection` namespace
### `NativeSecureArray<T>` sealed class
**Windows Only**
