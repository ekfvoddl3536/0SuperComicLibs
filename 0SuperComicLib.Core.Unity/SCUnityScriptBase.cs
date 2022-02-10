#region LICENSE
/* MIT License
 * 
 * Copyright (c) 2021 SuperComic <ekfvoddl3535@naver.com>
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE
 */
#endregion

using System;
using System.Collections;
using UnityEngine;

namespace SuperComicWorld
{
    public abstract class SCUnityScriptBase : MonoBehaviour
    {
        protected virtual void Awake()
        {
            if (this is ISCDependencies _dependencies)
            {
                StartCoroutine(new LateRegisterAuto(_dependencies, RegisterAuto));
                return;
            }

            RegisterAuto();
        }

        private void RegisterAuto()
        {
            if (this is ISCSystem _system)
                _system.RegisterSystem();

            if (this is ISCUpdateFixed _update)
                _update.Register();
        }

        private sealed class LateRegisterAuto : IEnumerator
        {
            private readonly ISCUpdateFixed[] list;
            private readonly ISCSystem[] systemList;

            private int lastIndex;

            private bool firstPassed;

            private Action onComplete;

            public LateRegisterAuto(ISCDependencies item, Action onComplete)
            {
                list = item.GetList();
                systemList = item.GetSystemList();

                this.onComplete = onComplete;
            }

            public bool MoveNext()
            {
                if (!firstPassed)
                {
                    firstPassed = WaitAll(list, ref lastIndex, SCUpdateSystem.Contains);
                    return true;
                }
                else if (WaitAll(systemList, ref lastIndex, SCUpdateSystem.ContainsSystem))
                {
                    onComplete.Invoke();
                    return false;
                }
                else
                    return true;
            }

            private static bool WaitAll<T>(T[] list, ref int Lastidx, Predicate<T> cb) where T : class
            {
                if (list != null)
                    for (int max = list.Length, y = Lastidx; y < max; y++)
                    {
                        if (!cb.Invoke(list[Lastidx = y]))
                            return false;
                    }

                Lastidx = 0;
                return true;
            }

            public void Reset()
            {
            }

            public object Current => null;
        }
    }
}
