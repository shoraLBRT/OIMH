/********************************/
/* Cogs & Goggles Unity Library */
/*                              */
/* (c) 2020, Cogs & Goggles     */
/********************************/

#if NET_4_6 || NET_STANDARD_2_0
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

namespace CogsAndGoggles.Library.DataStructures.SerializableDictionary
{
    public abstract class SerializableHashSetBase
    {
        public abstract class Storage
        {
        }

        protected class HashSet<TValue> : System.Collections.Generic.HashSet<TValue>
        {
            public HashSet()
            {
            }

            public HashSet(ISet<TValue> set) : base(set)
            {
            }

            public HashSet(SerializationInfo info, StreamingContext context) : base(info, context)
            {
            }
        }
    }

    [Serializable]
    public abstract class SerializableHashSet<T> : SerializableHashSetBase, ISet<T>, ISerializationCallbackReceiver,
        IDeserializationCallback, ISerializable
    {
        protected HashSet<T> hashSet;
        [SerializeField] protected T[] keys;

        protected SerializableHashSet()
        {
            hashSet = new HashSet<T>();
        }

        protected SerializableHashSet(ISet<T> set)
        {
            hashSet = new HashSet<T>(set);
        }

        public void CopyFrom(ISet<T> set)
        {
            hashSet.Clear();
            foreach (var value in set)
            {
                hashSet.Add(value);
            }
        }

        public void OnAfterDeserialize()
        {
            if (keys != null)
            {
                hashSet.Clear();
                var n = keys.Length;
                for (var i = 0; i < n; ++i)
                {
                    hashSet.Add(keys[i]);
                }

                keys = null;
            }
        }

        public void OnBeforeSerialize()
        {
            var n = hashSet.Count;
            keys = new T[n];

            var i = 0;
            foreach (var value in hashSet)
            {
                keys[i] = value;
                ++i;
            }
        }

        #region ISet<TValue>

        public int Count => ((ISet<T>) hashSet).Count;
        public bool IsReadOnly => ((ISet<T>) hashSet).IsReadOnly;

        public bool Add(T item)
        {
            return ((ISet<T>) hashSet).Add(item);
        }

        public void ExceptWith(IEnumerable<T> other)
        {
            ((ISet<T>) hashSet).ExceptWith(other);
        }

        public void IntersectWith(IEnumerable<T> other)
        {
            ((ISet<T>) hashSet).IntersectWith(other);
        }

        public bool IsProperSubsetOf(IEnumerable<T> other)
        {
            return ((ISet<T>) hashSet).IsProperSubsetOf(other);
        }

        public bool IsProperSupersetOf(IEnumerable<T> other)
        {
            return ((ISet<T>) hashSet).IsProperSupersetOf(other);
        }

        public bool IsSubsetOf(IEnumerable<T> other)
        {
            return ((ISet<T>) hashSet).IsSubsetOf(other);
        }

        public bool IsSupersetOf(IEnumerable<T> other)
        {
            return ((ISet<T>) hashSet).IsSupersetOf(other);
        }

        public bool Overlaps(IEnumerable<T> other)
        {
            return ((ISet<T>) hashSet).Overlaps(other);
        }

        public bool SetEquals(IEnumerable<T> other)
        {
            return ((ISet<T>) hashSet).SetEquals(other);
        }

        public void SymmetricExceptWith(IEnumerable<T> other)
        {
            ((ISet<T>) hashSet).SymmetricExceptWith(other);
        }

        public void UnionWith(IEnumerable<T> other)
        {
            ((ISet<T>) hashSet).UnionWith(other);
        }

        void ICollection<T>.Add(T item)
        {
            if (item != null)
            {
                ((ISet<T>) hashSet).Add(item);
            }
        }

        public void Clear()
        {
            ((ISet<T>) hashSet).Clear();
        }

        public bool Contains(T item)
        {
            return ((ISet<T>) hashSet).Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            ((ISet<T>) hashSet).CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            return ((ISet<T>) hashSet).Remove(item);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return ((ISet<T>) hashSet).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((ISet<T>) hashSet).GetEnumerator();
        }

        #endregion

        #region IDeserializationCallback

        public void OnDeserialization(object sender)
        {
            ((IDeserializationCallback) hashSet).OnDeserialization(sender);
        }

        #endregion

        #region ISerializable

        protected SerializableHashSet(SerializationInfo info, StreamingContext context)
        {
            hashSet = new HashSet<T>(info, context);
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            ((ISerializable) hashSet).GetObjectData(info, context);
        }

        #endregion
    }
}

#endif