using System;

namespace SuperComicLib.CodeDesigner
{
    public sealed class ILMethodSignature : IEquatable<ILMethodSignature>
    {
        public readonly HashedString hashed_methodName;
        public readonly Type[] parameterTypes;

        public ILMethodSignature(HashedString hashed_methodName, Type[] parameterTypes)
        {
            this.hashed_methodName = hashed_methodName;
            this.parameterTypes = parameterTypes;
        }

        public bool Equals(ILMethodSignature other)
        {
            if (other.hashed_methodName != hashed_methodName)
                return false;

            var tp = parameterTypes;
            var op = other.parameterTypes;

            int len = tp.Length;
            if (len != op.Length)
                return false;

            while (--len >= 0)
                if (tp[len] != op[len])
                    return false;

            return true;
        }

        public override int GetHashCode() => hashed_methodName.GetHashCode();
    }
}
