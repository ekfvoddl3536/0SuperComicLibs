# 0SuperComicLib.NET5Core (Reflection) Example
## Get Instruction Pointer (Hereinafter `IP`)
You can get the `IP` by calling `Register.IP`  
  
The return value is safe on all platforms and build configurations.  
So, Irrespective of platform and build configuration, `IP + 2` will give you where the next instruction in the `Register.IP` call instruction starts.  
  
```asm
0x8100: call eax      ; <--- call Register.IP(), returned 0x8100
0x8102: add eax, 2    ; <--- Register.IP() + 2
```
## `RuntimeAsm.JmpHere` Example
Assume the following situation:
```csharp
using SuperComicLib.Reflection;
...
static void M(string extreme_large_text) {
  // extreme large text (min length: 1,000,000+).
  // but, You know that the result of the conditional statement below branches always to else.
  string v = extreme_large_text;
  if (v.Contains("SomeMessage")) { ... }
  else if (v.Contains("SomeText_01")) { ... }
  else { // Always jump here }
}
```
Stupid cpu does over 2,000,000+ operations every time, slowing performance.  
You can use RuntimeAsm.JmpHere to help the stupid cpu.  
  
```csharp
static void M(string extreme_large_text) {
  nint ip = Register.IP();
  
  string v = extreme_large_text;
  if (v.Contains("SomeMessage")) { ... }
  else if (v.Contains("SomeText_01")) { ... }
  else 
  { // Always jump here
    RuntimeAsm.JmpHere(ip);
  }
```
Now the cpu checks every conditional statement only the first time.  
I got **over 3200%p performance improvement** in a similar case.  
  
With an easier example, let's see RuntimeAsm.JmpHere in action.  
  
## `RuntimeAsm.JmpHere` Example (+Run)
```csharp
for (int i = 0; i < 3; i++)
{
  Console.Write('A');
  
  nint ip = Register.IP();
  
  Console.Write('B');
  
  RuntimeAsm.JmpHere(ip);
  
  Console.WriteLine('C');
}
```
Executing the above source code produces the following output:  
```
ABC
AC
AC
```
You can see that output B is performed only the first time.  
  
## `RegisterValue` Desc +Example
This structure stores the values of 8 default registers for x86 and x64 in 8 fields.  
  
You can read the values of CPU registers using `Register.Capture` or `Register.Capture64Ex`.  
  
Refer to the following table to see which register's value is stored in which field:  
| Name | x86 REG | x64 REG |
| :--- | :------ | :------ |
| `nax` | `eax` | `rax` |
| `nbx` | `ebx` | `rbx` |
| `ncx` | `ecx` | `rcx` |
| `ndx` | `edx` | `rdx` |
| `nsi` | `esi` | `rsi` |
| `ndi` | `edi` | `rdi` |
| `nsp` | `esp` | `rsp` |
| `nbp` | `ebp` | `rbp` |
  
See the table below for the results of `Register.Capture64Ex`:  
| Name | x86 REG | x64 REG |
| :--- | :------ | :------ |
| `nax` | `eax` | `r8` |
| `nbx` | `ebx` | `r9` |
| `ncx` | `ecx` | `r10` |
| `ndx` | `edx` | `r11` |
| `nsi` | `esi` | `r12` |
| `ndi` | `edi` | `r13` |
| `nsp` | `esp` | `r14` |
| `nbp` | `ebp` | `r15` |

### Example
```csharp
RegisterValue regX64_X86 = Register.Capture();
int my_eax_int = (int)regX64_X86.nax;

RegisterValue regX64Ex = Register.Capture64Ex();
if (System.IntPtr.Size == sizeof(long)) // 64-bit
{
  Console.WriteLine("R8 Value: " + regX64Ex.nax);
}
else
{
  Console.WriteLine("EAX Value: " + regX64Ex.nax);
}
```
