﻿namespace SuperComicLib.Collections
{
    public sealed class LinkedNode<T>
    {
        internal LinkedNode<T> m_next;
        internal LinkedNode<T> m_prev;
        internal T m_value;

        internal LinkedNode() { }

        internal LinkedNode(T value)
        {
            m_next = m_prev = this;
            m_value = value;
        }

        public LinkedNode<T> Next => m_next;

        public LinkedNode<T> Prev => m_prev;

        public T Value => m_value;

        internal void Disconnect()
        {
            ref LinkedNode<T> next = ref m_next;
            ref LinkedNode<T> prev = ref m_prev;
            if (next != null)
                next.m_prev = prev;

            if (prev != null)
                prev.m_next = next;

            next = this;
            prev = this;
            m_value = default;
        }
    }
}