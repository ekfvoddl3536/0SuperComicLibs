using System;
using System.Reflection;
using System.Reflection.Emit;

namespace SuperComicLib.XPatch
{
    public sealed class ILMethodBody : MethodBody
    {
        private readonly byte[] ilstream;

        public ILMethodBody(ILGenerator gen)
        {
            Type t = typeof(ILGenerator);
            byte[] temp = (byte[])t.GetField("m_ILStream", Helper.mflag0).GetValue(gen);
            int len = (int)t.GetField("m_length", Helper.mflag0).GetValue(gen);

            ilstream = new byte[len];
            Buffer.BlockCopy(temp, 0, ilstream, 0, len);
        }

        public override byte[] GetILAsByteArray() => ilstream;
    }
}
