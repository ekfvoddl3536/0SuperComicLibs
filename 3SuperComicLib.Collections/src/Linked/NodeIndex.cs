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

using System.Diagnostics;

namespace SuperComicLib.Collections
{
    [DebuggerDisplay("Value = {Value}")]
    public readonly struct NodeIndex<T>
    {
        internal readonly ILinkedListSlim_Internal<T> m_owner;
        internal readonly int m_index;

        internal NodeIndex(ILinkedListSlim_Internal<T> owner, int index)
        {
            m_owner = owner;
            m_index = index;
        }

        public int RawIndex => m_index;

        public bool IsInvalid => m_owner == null || m_index < 0;

        public ref T Value => ref m_owner.ByRefValue(m_index);

        public NodeIndex<T> Next => new NodeIndex<T>(m_owner, m_owner.GetNextNode(m_index));

        public NodeIndex<T> Prev => new NodeIndex<T>(m_owner, m_owner.GetPrevNode(m_index));

        public override bool Equals(object obj) => false;
        public override int GetHashCode() => m_owner.GetHashCode() ^ m_index;

        public static bool operator ==(NodeIndex<T> left, NodeIndex<T> right) => left.m_owner == right.m_owner && left.m_index == right.m_index;
        public static bool operator !=(NodeIndex<T> left, NodeIndex<T> right) => left.m_owner != right.m_owner || left.m_index != right.m_index;

        public static NodeIndex<T> operator ++(NodeIndex<T> self) => self.Next;
        public static NodeIndex<T> operator --(NodeIndex<T> self) => self.Prev;
    }
}
