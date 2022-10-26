/********************************/
/* Cogs & Goggles Unity Library */
/*                              */
/* (c) 2020, Cogs & Goggles     */
/********************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

namespace CogsAndGoggles.Library.DataStructures.SerializableDictionary
{
    public abstract class SerializableDictionaryBase
    {
        public abstract class Storage
        {
        }

        protected class Dictionary<TKey, TValue> : System.Collections.Generic.Dictionary<TKey, TValue>
        {
            public Dictionary()
            {
            }

            public Dictionary(IDictionary<TKey, TValue> dict) : base(dict)
            {
            }

            public Dictionary(SerializationInfo info, StreamingContext context) : base(info, context)
            {
            }
        }
    }

    [Serializable]
    public abstract class SerializableDictionaryBase<TKey, TValue, TValueStorage> : SerializableDictionaryBase,
        IDictionary<TKey, TValue>, IDictionary, ISerializationCallbackReceiver, IDeserializationCallback, ISerializable
    {
        protected Dictionary<TKey, TValue> dictionary;
        [SerializeField] 
        protected TKey[] keys;
        [SerializeField] 
        protected TValueStorage[] values;

        protected SerializableDictionaryBase()
        {
            dictionary = new Dictionary<TKey, TValue>();
        }

        protected SerializableDictionaryBase(IDictionary<TKey, TValue> dict)
        {
            dictionary = new Dictionary<TKey, TValue>(dict);
        }

        protected abstract void SetValue(TValueStorage[] storage, int i, TValue value);
        protected abstract TValue GetValue(TValueStorage[] storage, int i);

        public void CopyFrom(IDictionary<TKey, TValue> dict)
        {
            dictionary.Clear();
            foreach (var kvp in dict)
            {
                dictionary[kvp.Key] = kvp.Value;
            }
        }

        public void OnAfterDeserialize()
        {
            if (keys == null || values == null || keys.Length != values.Length) return;
            dictionary.Clear();
            var n = keys.Length;
            for (var i = 0; i < n; ++i)
            {
                dictionary[keys[i]] = GetValue(values, i);
            }

            keys = null;
            values = null;
        }

        public void OnBeforeSerialize()
        {
            var n = dictionary.Count;
            keys = new TKey[n];
            values = new TValueStorage[n];

            var i = 0;
            foreach (var kvp in dictionary)
            {
                keys[i] = kvp.Key;
                SetValue(values, i, kvp.Value);
                ++i;
            }
        }

        #region IDictionary<TKey, TValue>

        public ICollection<TKey> Keys => ((IDictionary<TKey, TValue>) dictionary).Keys;

        public ICollection<TValue> Values => ((IDictionary<TKey, TValue>) dictionary).Values;

        public int Count => ((IDictionary<TKey, TValue>) dictionary).Count;

        public bool IsReadOnly => ((IDictionary<TKey, TValue>) dictionary).IsReadOnly;

        public TValue this[TKey key]
        {
            get => ((IDictionary<TKey, TValue>) dictionary)[key];
            set => ((IDictionary<TKey, TValue>) dictionary)[key] = value;
        }

        public void Add(TKey key, TValue value)
        {
            ((IDictionary<TKey, TValue>) dictionary).Add(key, value);
        }

        public bool ContainsKey(TKey key)
        {
            return ((IDictionary<TKey, TValue>) dictionary).ContainsKey(key);
        }

        public bool Remove(TKey key)
        {
            return ((IDictionary<TKey, TValue>) dictionary).Remove(key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return ((IDictionary<TKey, TValue>) dictionary).TryGetValue(key, out value);
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            ((IDictionary<TKey, TValue>) dictionary).Add(item);
        }

        public void Clear()
        {
            ((IDictionary<TKey, TValue>) dictionary).Clear();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return ((IDictionary<TKey, TValue>) dictionary).Contains(item);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            ((IDictionary<TKey, TValue>) dictionary).CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return ((IDictionary<TKey, TValue>) dictionary).Remove(item);
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return ((IDictionary<TKey, TValue>) dictionary).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IDictionary<TKey, TValue>) dictionary).GetEnumerator();
        }

        #endregion

        #region IDictionary

        public bool IsFixedSize => ((IDictionary) dictionary).IsFixedSize;

        ICollection IDictionary.Keys => ((IDictionary) dictionary).Keys;

        ICollection IDictionary.Values => ((IDictionary) dictionary).Values;

        public bool IsSynchronized => ((IDictionary) dictionary).IsSynchronized;

        public object SyncRoot => ((IDictionary) dictionary).SyncRoot;

        public object this[object key]
        {
            get => ((IDictionary) dictionary)[key];
            set => ((IDictionary) dictionary)[key] = value;
        }

        public void Add(object key, object value)
        {
            ((IDictionary) dictionary).Add(key, value);
        }

        public bool Contains(object key)
        {
            return ((IDictionary) dictionary).Contains(key);
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            return ((IDictionary) dictionary).GetEnumerator();
        }

        public void Remove(object key)
        {
            ((IDictionary) dictionary).Remove(key);
        }

        public void CopyTo(Array array, int index)
        {
            ((IDictionary) dictionary).CopyTo(array, index);
        }

        #endregion

        #region IDeserializationCallback

        public void OnDeserialization(object sender)
        {
            ((IDeserializationCallback) dictionary).OnDeserialization(sender);
        }

        #endregion

        #region ISerializable

        protected SerializableDictionaryBase(SerializationInfo info, StreamingContext context)
        {
            dictionary = new Dictionary<TKey, TValue>(info, context);
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            ((ISerializable) dictionary).GetObjectData(info, context);
        }

        #endregion
    }

    public static class SerializableDictionary
    {
        public class Storage<T> : SerializableDictionaryBase.Storage
        {
            public T data;
        }
    }

    public class SerializableDictionary<TKey, TValue> : SerializableDictionaryBase<TKey, TValue, TValue>
    {
        public SerializableDictionary()
        {
        }

        public SerializableDictionary(IDictionary<TKey, TValue> dict) : base(dict)
        {
        }

        protected SerializableDictionary(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        protected override TValue GetValue(TValue[] storage, int i)
        {
            return storage[i];
        }

        protected override void SetValue(TValue[] storage, int i, TValue value)
        {
            storage[i] = value;
        }
    }

    public class
        SerializableDictionary<TKey, TValue, TValueStorage> : SerializableDictionaryBase<TKey, TValue, TValueStorage>
        where TValueStorage : SerializableDictionary.Storage<TValue>, new()
    {
        public SerializableDictionary()
        {
        }

        public SerializableDictionary(IDictionary<TKey, TValue> dict) : base(dict)
        {
        }

        protected SerializableDictionary(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        protected override TValue GetValue(TValueStorage[] storage, int i)
        {
            return storage[i].data;
        }

        protected override void SetValue(TValueStorage[] storage, int i, TValue value)
        {
            storage[i] = new TValueStorage {data = value};
        }
    }
}