using System.Collections.Generic;

namespace FactoryFactory.Util
{
    public class DictionaryOfLists<TKey, TValue> : Dictionary<TKey, IList<TValue>>
    {
        public void AddOne(TKey key, TValue value)
        {
            if (!TryGetValue(key, out var list)) {
                list = new List<TValue>();
                Add(key, list);
            }
            list.Add(value);
        }
    }
}
