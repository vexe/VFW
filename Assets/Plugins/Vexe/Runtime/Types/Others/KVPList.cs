using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Vexe.Runtime.Extensions;
using Vexe.Runtime.Helpers;

namespace Vexe.Runtime.Types
{
    /// <summary>
    /// A dictionary-like structure that uses lists for keys and values so it plays well with Unity's serialization system
    /// Not out of the box of course as you still have to subclass to create your own custom class annotated with [Serializable]
    /// ex: [Serializable] public class Lookup : KVPList<int, GameObject> { }
    /// Since the keys/values are just likes it means you can access them independantly
    /// There is no hashing. Instead, a linear search is performed when you lookup values
    /// </summary>
	public class KVPList<TKey, TValue> : IDictionary<TKey, TValue>
	{
        // Should be just public readonly, but that would not play nice with Unity serialization
		[SerializeField] private List<TKey> keys;
		[SerializeField] private List<TValue> values;

        public List<TKey> Keys     { get { return keys;       } }
        public List<TValue> Values { get { return values;     } }
		public int Count           { get { return keys.Count; } }

        public KVPList(int capacity)
            : this(new List<TKey>(capacity), new List<TValue>(capacity))
        {
        }

        public KVPList()
            : this(new List<TKey>(), new List<TValue>())
        {
        }

        public KVPList(List<TKey> keys, List<TValue> values)
        {
            this.keys = keys;
            this.values = values;
        }

        public TValue this[TKey key]
        {
            get
            {
                int index;
                if (!TryGetIndex(key, out index))
                {
                    throw new KeyNotFoundException(key.ToString());
                }
                return values[index];
            }
            set
            {
                int index;
                if (!TryGetIndex(key, out index))
                {
                    Add(key, value);
                }
                else values[index] = value;
            }
        }

        public void SetKeyAt(int i, TKey value)
        {
            AssertIndexInBounds(i);
            if (value != null && !value.Equals(keys[i]))
                AssertUniqueKey(value);
            keys[i] = value;
        }

        public TKey GetKeyAt(int i)
        {
            AssertIndexInBounds(i);
            return keys[i];
        }

        public void SetValueAt(int i, TValue value)
        {
            AssertIndexInBounds(i);
            values[i] = value;
        }

        public TValue GetValueAt(int i)
        {
            AssertIndexInBounds(i);
            return values[i];
        }

        public KeyValuePair<TKey, TValue> GetPairAt(int i)
        {
            AssertIndexInBounds(i);
            return new KeyValuePair<TKey, TValue>(keys[i], values[i]);
        }

        private void AssertIndexInBounds(int i)
        {
            if (i < 0 || i >= keys.Count)
                throw new IndexOutOfRangeException("i");
        }

        public void Clear()
        {
            keys.Clear();
            values.Clear();
        }

        public void Insert(int i, TKey key, TValue value)
        {
            Insert(i, key, value, true);
        }

        public void Insert(int i, TKey key, TValue value, bool assertUniqueKey)
        {
            if (assertUniqueKey)
                AssertUniqueKey(key);
            if (key == null)
                throw new ArgumentNullException("Dictionary key cannot be null");
            keys.Insert(i, key);
            values.Insert(i, value);
        }

        private void AssertUniqueKey(TKey key)
        {
            if (ContainsKey(key))
                throw new ArgumentException(string.Format("There's already a key `{0}` defined in the dictionary", key.ToString()));
        }

        public void Insert(TKey key, TValue value)
        {
            Insert(0, key, value);
        }

        public void Add(TKey key, TValue value)
        {
            Add(key, value, true);
        }

        public void Add(TKey key, TValue value, bool assertUniqueKey)
        {
            Insert(Count, key, value, assertUniqueKey);
        }

        public bool Remove(TKey key)
        {
            int index;
            if (TryGetIndex(key, out index))
            {
                keys.RemoveAt(index);
                values.RemoveAt(index);
                return true;
            }
            return false;
        }

        public void RemoveAt(int i)
        {
            AssertIndexInBounds(i);
            keys.RemoveAt(i);
            values.RemoveAt(i);
        }

        public void RemoveLast()
        {
            RemoveAt(Count - 1);
        }

        public void RemoveFirst()
        {
            RemoveAt(0);
        }

        public bool TryGetValue(TKey key, out TValue result)
        {
            int index;
            if (!TryGetIndex(key, out index))
            {
                result = default(TValue);
                return false;
            }
            result = values[index];
            return true;
        }

        public bool ContainsValue(TValue value)
        {
            return values.Contains(value);
        }

        public bool ContainsKey(TKey key)
        {
            return keys.Contains(key);
        }

        private bool TryGetIndex(TKey key, out int index)
        {
            return (index = keys.IndexOf(key)) != -1;
        }

        public struct KVPEnumerator : IEnumerator<KeyValuePair<TKey, TValue>>
        {
            private int _idx;
            private readonly KVPList<TKey, TValue> _list;

            public KVPEnumerator(KVPList<TKey, TValue> list)
            {
                _list = list;
                _idx = -1;
            }

            public KeyValuePair<TKey, TValue> Current
            {
                get { return new KeyValuePair<TKey, TValue>(_list.Keys[_idx], _list.Values[_idx]); }
            }

            object IEnumerator.Current
            {
                get { return this.Current; }
            }

            public bool MoveNext()
            {
                return ++_idx < _list.Count;
            }

            public void Reset()
            {
                _idx = 0;
            }

            public void Dispose()
            {
            }
        }

        public KVPEnumerator GetEnumerator()
        {
            return new KVPEnumerator(this);
        }

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        ICollection<TKey> IDictionary<TKey, TValue>.Keys
        {
            get { return keys; }
        }

        bool IDictionary<TKey, TValue>.Remove(TKey key)
        {
            return Remove(key);
        }

        ICollection<TValue> IDictionary<TKey, TValue>.Values
        {
            get { return values; }
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return ContainsKey(item.Key);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            for (int i = arrayIndex; i < array.Length; i++)
            {
                array[i] = new KeyValuePair<TKey, TValue>(keys[i], values[i]);
            }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return Remove(item.Key);
        }

		public override string ToString()
		{
			return string.Format("<{0}, {1}> (Count = {2})", typeof(TKey).GetNiceName(), typeof(TValue).GetNiceName(), Count);
		}
	}

	public static class KVPListExtensions
	{
		public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this KVPList<TKey, TValue> d)
		{
			return RuntimeHelper.CreateDictionary(d.Keys, d.Values);
		}

		public static KVPList<TKey, TValue> ToKVPList<TKey, TValue>(this IDictionary<TKey, TValue> d)
		{
			return new KVPList<TKey, TValue>(d.Keys.ToList(), d.Values.ToList());
		}
	}
}