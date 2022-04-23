using System.Collections.Generic;

namespace Swordfish.Library.Types
{
    public class SwitchDictionary<TKey1, TKey2, TValue>
    {
        private LinkedDictionary<TKey1, TKey2> link = new LinkedDictionary<TKey1, TKey2>();

        private Dictionary<TKey1, TValue> dictionary1 = new Dictionary<TKey1, TValue>();

        private Dictionary<TKey2, TValue> dictionary2 = new Dictionary<TKey2, TValue>();

        public TValue this[TKey1 key1] {
            get => dictionary1[key1];
            set => dictionary1[key1] = value;
        }

        public TValue this[TKey2 key2] {
            get => dictionary2[key2];
            set => dictionary2[key2] = value;
        }

        public int Count => dictionary1.Count;

        public void Add(TKey2 key2, TKey1 key1, TValue value) => Add(key1, key2, value);

        public void Add(TKey1 key1, TKey2 key2, TValue value)
        {
            dictionary1.Add(key1, value);
            dictionary2.Add(key2, value);
            link.Add(key1, key2);
        }

        public void Clear()
        {
            dictionary1.Clear();
            dictionary2.Clear();
        }
        
        public bool Contains(TKey1 key) => link.Contains(key);

        public bool Contains(TKey2 key) => link.Contains(key);

        public bool Remove(TKey1 key)
        {
            if (dictionary1.Remove(key))
            {
                dictionary2.Remove(link[key]);
                link.Remove(key);
                return true;
            }

            return false;
        }

        public bool Remove(TKey2 key)
        {
            if (dictionary2.Remove(key))
            {
                dictionary1.Remove(link[key]);
                link.Remove(key);
                return true;
            }

            return false;
        }

        public bool TryGetValue(TKey1 key, out TValue value) => dictionary1.TryGetValue(key, out value);

        public bool TryGetValue(TKey2 key, out TValue value) => dictionary2.TryGetValue(key, out value);

    }
}
