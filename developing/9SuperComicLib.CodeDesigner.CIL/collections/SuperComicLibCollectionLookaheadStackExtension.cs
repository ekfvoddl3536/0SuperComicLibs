using System.Runtime.CompilerServices;
using SuperComicLib.Collections;

namespace SuperComicLib.CodeDesigner
{
    public static class SuperComicLibCollectionLookaheadStackExtension
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.PreserveSig)]
        public static void InnerState(this LookaheadStack<uint> inst, uint root, uint inner)
        {
            inst.Push(root);
            inst.Push(inner);
        }
    }
}
