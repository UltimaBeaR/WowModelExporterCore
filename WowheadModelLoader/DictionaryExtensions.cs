using System.Collections.Generic;

namespace WowheadModelLoader
{
    public static class DictionaryExtensions
    {
        public static TValue GetOrDefault<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dictionary, TKey key)
        {
            if (!dictionary.TryGetValue(key, out TValue value))
                return default(TValue);

            return value;
        }
    }
}
