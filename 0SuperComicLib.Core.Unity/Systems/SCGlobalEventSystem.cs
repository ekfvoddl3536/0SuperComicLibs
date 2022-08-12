using System;
using System.Collections.Generic;

namespace SuperComicWorld
{
    public static class SCGlobalEventSystem
    {
        private static readonly Dictionary<int, EventListeners> events = new Dictionary<int, EventListeners>();

        public static bool IsRegistered(int hashcode) => events.ContainsKey(hashcode);

        public static void EventRaise(int hashcode, object sender, object argument)
        {
            if (events.TryGetValue(hashcode, out EventListeners item))
                item.Invoke(sender, argument);
        }

        public static void Register(int hashcode, Action<object, object> callback)
        {
            if (events.TryGetValue(hashcode, out EventListeners res))
                res.Add(callback);
            else
                events.Add(hashcode, new EventListeners(callback));
        }

        public static bool Unregister(int hashcode, Action<object, object> callback) =>
            events.TryGetValue(hashcode, out EventListeners res) && res.Remove(callback);

        private sealed class EventListeners
        {
            public Delegate[] items;
            public int count;

            public EventListeners(Delegate value) => Add(value);

            public void Add(Delegate value)
            {
                Array.Resize(ref items, count + 1);
                items[count++] = value;
            }

            public bool Remove(Delegate value)
            {
                Delegate[] items = this.items;
                for (int n = count; --n >= 0;)
                    if (items[n] == value)
                    {
                        // 삭제된 위치의 잉여 공간을 없앱니다.
                        Array.Copy(items, n + 1, items, n, --count - n);
                        items[count] = null;

                        return true;
                    }

                return false;
            }

            public void Invoke(object sender, object argument)
            {
                Delegate[] items = this.items;
                for (int n = count; --n >= 0;)
                    ((Action<object, object>)items[n]).Invoke(sender, argument);
            }
        }
    }
}
