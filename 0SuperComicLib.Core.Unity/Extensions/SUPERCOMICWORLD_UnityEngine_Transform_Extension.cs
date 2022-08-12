#region LICENSE
// NO LICENSE
#endregion
using System.Runtime.CompilerServices;

public static class SUPERCOMICWORLD_UnityEngine_Transform_Extension
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static UnityEngine.Vector3 LocalForward(this UnityEngine.Transform tr) => tr.worldToLocalMatrix.MultiplyVector(tr.forward);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static UnityEngine.Vector3 LocalRight(this UnityEngine.Transform tr) => tr.worldToLocalMatrix.MultiplyVector(tr.right);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static UnityEngine.Vector3 LocalUp(this UnityEngine.Transform tr) => tr.worldToLocalMatrix.MultiplyVector(tr.up);
}
