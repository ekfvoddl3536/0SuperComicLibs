using System.Runtime.CompilerServices;
using pp = UnityEngine.PlayerPrefs;

namespace SuperComicWorld
{
    public static class SCOptions
    {
        public static float MouseSensitivity
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => pp.GetFloat(nameof(MouseSensitivity), 1.0f);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                pp.SetFloat(nameof(MouseSensitivity), value);
                SCGlobalEventSystem.EventRaise((int)SCGlobalEventCode.UPDATE_MOUSE_SPEED, null, value);
            }
        }
    }
}
