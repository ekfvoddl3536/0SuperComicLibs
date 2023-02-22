// MIT License
//
// Copyright (c) 2019-2023. SuperComic (ekfvoddl3535@naver.com)
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

#pragma warning disable IDE1006
using System;
using System.Collections.Generic;

namespace SuperComicLib.Collections
{
    internal sealed class SuperComic_VersionBasedReadOnlyEnumerator<T> : SuperComic_InnerEnumeratorBaseImpl<T>
    {
        private readonly IVersionControlledCollection target;
        private int version;

        public SuperComic_VersionBasedReadOnlyEnumerator(IVersionControlledCollection target, IEnumerator<T> source) : base(source)
        {
            this.target = target;
            version = target.LatestVersion;
        }

        public override bool MoveNext()
        {
            if (target.LatestVersion != version)
                throw new InvalidOperationException("The collection was modified after the enumerator was created.");

            return innerEnumerator.MoveNext();
        }

        public override void Reset()
        {
            version = target.LatestVersion;
            innerEnumerator.Reset();
        }
    }
}
