using System;

namespace SuperComicLib
{
    public sealed unsafe class ReverseIterator<T> : IDisposable where T : unmanaged
    {
        public static readonly ReverseIterator<T> Empty = new ReverseIterator<T>(null, 0);

        private T* m_array;
        private int m_length;
        private int m_current;

        internal ReverseIterator(T* array, int length)
        {
            m_array = array;
            m_length = length;
            m_current = length - 1;
        }

        internal ReverseIterator(T* array, int length, int current)
        {
            m_array = array;
            m_length = length;
            m_current = current;
        }

        public ref T Current => ref m_array[m_current];

        public ref TType Value<TType>() where TType : unmanaged => ref *(TType*)(m_array + m_current);

        public bool IsEnd => m_array == null || m_current < 0 || m_current >= m_length;

        public void Add(int v) => m_current -= v;
        public void Sub(int v) => m_current += v;
        public void Reset() => m_current = 0;

        private bool Equals(in ReverseIterator<T> other) =>
            other.m_array == m_array &&
            other.m_current == m_current &&
            other.m_length == m_length;

        public override bool Equals(object obj) =>
            obj is ReverseIterator<T> other
            ? Equals(in other)
            : false;

        public override int GetHashCode() => (*(IntPtr*)m_array).GetHashCode();
        public void Dispose() => m_array = null;

        public static implicit operator bool(ReverseIterator<T> value) => value.IsEnd == false;

        public static bool operator ==(ReverseIterator<T> left, ReverseIterator<T> right) => left.Equals(in right);
        public static bool operator !=(ReverseIterator<T> left, ReverseIterator<T> right) => left.Equals(in right) == false;

        public static ReverseIterator<T> operator +(ReverseIterator<T> left, int right) 
        { 
            left.Add(right); 
            return left; 
        }
        public static ReverseIterator<T> operator -(ReverseIterator<T> left, int right) 
        { 
            left.Sub(right); 
            return left; 
        }
        public static ReverseIterator<T> operator ++(ReverseIterator<T> left) 
        { 
            left.Add(1); 
            return left; 
        }
        public static ReverseIterator<T> operator --(ReverseIterator<T> left) 
        { 
            left.Sub(1); 
            return left; 
        }
        public static ReverseIterator<T> operator !(ReverseIterator<T> value) 
        { 
            value.Reset(); 
            return value; 
        }
    }
}
