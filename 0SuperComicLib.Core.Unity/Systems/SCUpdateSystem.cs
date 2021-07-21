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

using System.Collections.Generic;

namespace SuperComicWorld
{
    public static class SCUpdateSystem
    {
        private static readonly List<ISCUpdateFixed>[] updates = new List<ISCUpdateFixed>[(int)SCUpdateType.SCU_1000ms + 1];
        private static readonly float[] times = new float[(int)SCUpdateType.SCU_1000ms + 1];

        private static readonly SortedList<int, ISCSystem> systems = new SortedList<int, ISCSystem>();

        public static void Register(this ISCUpdateFixed instance) => 
            updates[(int)instance.UpdateType].Add(instance);

        public static void RegisterSystem(this ISCSystem instance) => 
            systems[instance.UpdateOrder] = instance;

        public static bool Contains(ISCUpdateFixed value) => 
            updates[(int)value.UpdateType].Contains(value);

        public static bool ContainsSystem(ISCSystem value) =>
            systems.ContainsKey(value.UpdateOrder);

        public static void Update()
        {
            float dt = UnityEngine.Time.deltaTime;

            // System 업데이트 최우선
            var iter_s = systems.Values.GetEnumerator();
            while (iter_s.MoveNext())
            {
                var cur_sytm = iter_s.Current;
                if (cur_sytm.IsDirty)
                    cur_sytm.Update(dt);
            }

            iter_s.Dispose();

            TryUpdate(1f / 30, (int)SCUpdateType.SCU_33ms_30fps, dt);
            TryUpdate(1f / 10, (int)SCUpdateType.SCU_100ms_10fps, dt);
            TryUpdate(1f / 4, (int)SCUpdateType.SCU_250ms_4fps, dt);
            TryUpdate(1f, (int)SCUpdateType.SCU_1000ms, dt);
        }

        private static void TryUpdate(float ms, int index, float dt)
        {
            if ((times[index] += dt) >= ms)
            {
                times[index] = 0;

                var list = updates[index];
                for (int i = list.Count; --i >= 0;)
                    list[i].Update(dt);
            }
        }
    }
}
