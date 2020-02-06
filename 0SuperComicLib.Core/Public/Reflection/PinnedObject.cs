using System;
using System.Runtime.InteropServices;

namespace SuperComicLib.Core
{
    /// <summary>
    /// GCHandle을 이용해 참조를 만듭니다,
    /// 참조 유형이 포함되면 안되는 Unity Job 등에서 유용합니다
    /// </summary>
    /// <typeparam name="T">나중에 까먹지 않도록 유형을 정합니다</typeparam>
    public struct PinnedObject<T> : IDisposable where T : class
    {
        private readonly GCHandle m_handle;

        public PinnedObject(T original) => m_handle = GCHandle.Alloc(original, GCHandleType.Normal);

        public T Value => (T)m_handle.Target;

        public bool IsNull => !m_handle.IsAllocated;

        public void Dispose() => m_handle.Free();

        public override bool Equals(object obj) => Value.Equals(obj);
        public override int GetHashCode() => Value.GetHashCode();
        public override string ToString() => Value.ToString();

        public static bool operator ==(PinnedObject<T> left, PinnedObject<T> right) => left.m_handle == right.m_handle;
        public static bool operator !=(PinnedObject<T> left, PinnedObject<T> right) => left.m_handle != right.m_handle;

        public static implicit operator bool(PinnedObject<T> pinobj) => pinobj.IsNull;
        public static implicit operator T(PinnedObject<T> pinobj) => pinobj.Value;
        public static implicit operator PinnedObject<T>(T obj) => new PinnedObject<T>(obj);
    }
}
