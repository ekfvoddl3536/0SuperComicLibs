// using System;
// using System.Runtime.InteropServices;
// 
// refer https://stackoverflow.com/questions/17339928/c-sharp-how-to-convert-object-to-intptr-and-back
// public static class SCL_CORE_ObjectExtension
// {
//     public static IntPtr ToIntPtr(this object target)
//     {
//         GCHandle handle = GCHandle.Alloc(target);
//         IntPtr ptr = GCHandle.ToIntPtr(handle);
//         handle.Free();
//         return ptr;
//     }
// }