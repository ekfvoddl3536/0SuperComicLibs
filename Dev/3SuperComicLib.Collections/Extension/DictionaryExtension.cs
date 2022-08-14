using System.Collections.Generic;

namespace SuperComicLib.Collections
{
    public static class DictionaryExtension
    {
        public static bool TryAdd<TKey, TValue>(this IDictionary<TKey, TValue> inst, TKey key, TValue value)
        {
            if (inst.ContainsKey(key))
                return false;

            inst.Add(key, value);
            return true;
        }
    }
}
