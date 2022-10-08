// MIT License
//
// Copyright (c) 2019-2022 SuperComic (ekfvoddl3535@naver.com)
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

namespace SuperComicLib.Collections
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
