﻿using System;
using System.Collections;
using System.Collections.Generic;
using BoysheO.Buffer.PooledBuffer;
using BoysheO.Buffer.PooledBuffer.Dictionary;
using BoysheO.Buffers.PooledBuffer.Linq;

namespace BoysheO.Buffers
{
    /// <summary>
    /// don't use again after disposable
    /// </summary>
    [Obsolete("set value will cause gc allow after Clear() called")]
    public readonly struct PooledDictionaryBuffer<TK, TV> : IDisposable, IReadOnlyDictionary<TK, TV>,
        IDictionary<TK, TV>, IDictionary,IReadOnlyPooledDictionaryBuff<TK,TV>
    {
        internal readonly long Version;
        internal readonly DictionaryProxy<TK, TV> BufferProxy;

        internal PooledDictionaryBuffer(DictionaryProxy<TK, TV> bufferProxy)
        {
            BufferProxy = bufferProxy;
            Version = bufferProxy.Version;
        }

        public void Dispose()
        {
            if (IsAlive)
            {
                PooledDictionaryBufferPool<TK, TV>.Share.Return(this);
            }
        }

        
        /// <summary>
        /// 尽管提供了IsAlive判断，但是使用者应当在业务流程上确保Buff生命周期正确且单一，而不是频繁使用IsAlive判定。频繁使用IsAlive违背本库设计初衷
        /// Although IsAlive verdicts are provided, users should ensure that the buff lifetime is correct and single in the business process, rather than using IsAlive verdicts frequently. Frequent use of IsAlive is contrary to the original design intent of this library
        /// </summary>
        public bool IsAlive
        {
            get
            {
                return BufferProxy != null && BufferProxy.Buffer != null && Version == BufferProxy.Version;
            }
        }

        public static PooledDictionaryBuffer<TK, TV> Rent()
        {
            return PooledDictionaryBufferPool<TK, TV>.Share.Rent();
        }

        private void ThrowIfVersionNotMatch()
        {
            if (BufferProxy == null)
                throw new ObjectDisposedException(
                    $"you must get this buffer using static method {nameof(PooledDictionaryBuffer<TK, TV>)}.{nameof(Rent)}()");
            if (BufferProxy == null || Version != BufferProxy.Version)
                throw new ObjectDisposedException("this buffer is disposed");
        }

        public DictionaryBufferEnumerator<TK, TV> GetEnumerator()
        {
            ThrowIfVersionNotMatch();
            return new DictionaryBufferEnumerator<TK, TV>(BufferProxy, BufferProxy.Version);
        }

        public PooledListBuffer<TK> GetKeys()
        {
            return Keys.ToPooledListBuffer();
        }

        public PooledListBuffer<TV> GetValues()
        {
            return Values.ToPooledListBuffer();
        }

        bool IDictionary.Contains(object key)
        {
            ThrowIfVersionNotMatch();
            return ((IDictionary) BufferProxy.Buffer).Contains(key);
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            ThrowIfVersionNotMatch();
            return GetEnumerator();
        }

        void IDictionary.Remove(object key)
        {
            ThrowIfVersionNotMatch();
            ((IDictionary) BufferProxy.Buffer).Remove(key);
        }

        bool IDictionary.IsFixedSize
        {
            get
            {
                ThrowIfVersionNotMatch();
                return ((IDictionary) BufferProxy.Buffer).IsFixedSize;
            }
        }

        IEnumerator<KeyValuePair<TK, TV>> IEnumerable<KeyValuePair<TK, TV>>.GetEnumerator()
        {
            ThrowIfVersionNotMatch();
            return GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(KeyValuePair<TK, TV> item)
        {
            ThrowIfVersionNotMatch();
            ((ICollection<KeyValuePair<TK, TV>>) BufferProxy.Buffer).Add(item);
        }

        void IDictionary.Add(object key, object value)
        {
            ThrowIfVersionNotMatch();
            ((IDictionary) BufferProxy.Buffer).Add(key, value);
        }

        public void Clear()
        {
            ThrowIfVersionNotMatch();
            BufferProxy.Buffer.Clear();
        }

        public bool Contains(KeyValuePair<TK, TV> item)
        {
            ThrowIfVersionNotMatch();
            return ((ICollection<KeyValuePair<TK, TV>>) BufferProxy.Buffer).Contains(item);
        }

        public void CopyTo(KeyValuePair<TK, TV>[] array, int arrayIndex)
        {
            ThrowIfVersionNotMatch();
            ((IDictionary) BufferProxy.Buffer).CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<TK, TV> item)
        {
            ThrowIfVersionNotMatch();
            return ((ICollection<KeyValuePair<TK, TV>>) BufferProxy.Buffer).Remove(item);
        }

        void ICollection.CopyTo(Array array, int index)
        {
            ThrowIfVersionNotMatch();
            ((ICollection) BufferProxy.Buffer).CopyTo(array, index);
        }

        public int Count
        {
            get
            {
                ThrowIfVersionNotMatch();
                return BufferProxy.Buffer.Count;
            }
        }

        bool ICollection.IsSynchronized
        {
            get
            {
                ThrowIfVersionNotMatch();
                return ((IDictionary) BufferProxy.Buffer).IsSynchronized;
            }
        }

        object ICollection.SyncRoot
        {
            get
            {
                ThrowIfVersionNotMatch();
                return ((IDictionary) BufferProxy.Buffer).SyncRoot;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                ThrowIfVersionNotMatch();
                return ((IDictionary) BufferProxy.Buffer).IsReadOnly;
            }
        }

        object IDictionary.this[object key]
        {
            get
            {
                ThrowIfVersionNotMatch();
                return ((IDictionary) BufferProxy.Buffer)[key];
            }
            set
            {
                ThrowIfVersionNotMatch();
                ((IDictionary) BufferProxy.Buffer)[key] = value;
            }
        }

        public void Add(TK key, TV value)
        {
            ThrowIfVersionNotMatch();
            (BufferProxy.Buffer).Add(key, value);
        }

        public bool ContainsKey(TK key)
        {
            ThrowIfVersionNotMatch();
            return (BufferProxy.Buffer).ContainsKey(key);
        }

        public bool Remove(TK key)
        {
            ThrowIfVersionNotMatch();
            return (BufferProxy.Buffer).Remove(key);
        }

        public bool TryGetValue(TK key, out TV value)
        {
            ThrowIfVersionNotMatch();
            return (BufferProxy.Buffer).TryGetValue(key, out value);
        }

        public TV GetValueOrDefault(TK key, TV fallback)
        {
            return TryGetValue(key, out var v) ? v : fallback;
        }
        
        public TV GetValueOrDefault(TK key)
        {
            return TryGetValue(key, out var v) ? v : default;
        }
        public bool Remove(TK key, out TV value)
        {
            return BufferProxy.Buffer.Remove(key, out value);
        }

        public TV this[TK key]
        {
            get
            {
                ThrowIfVersionNotMatch();
                return (BufferProxy.Buffer)[key];
            }
            set
            {
                ThrowIfVersionNotMatch();
                (BufferProxy.Buffer)[key] = value;
            }
        }

        public IEnumerable<TK> Keys
        {
            get
            {
                ThrowIfVersionNotMatch();
                return (BufferProxy.Buffer).Keys;
            }
        }

        ICollection IDictionary.Values
        {
            get
            {
                ThrowIfVersionNotMatch();
                return BufferProxy.Buffer.Values;
            }
        }

        ICollection IDictionary.Keys
        {
            get
            {
                ThrowIfVersionNotMatch();
                return BufferProxy.Buffer.Keys;
            }
        }

        ICollection<TV> IDictionary<TK, TV>.Values
        {
            get
            {
                ThrowIfVersionNotMatch();
                return BufferProxy.Buffer.Values;
            }
        }

        ICollection<TK> IDictionary<TK, TV>.Keys
        {
            get
            {
                ThrowIfVersionNotMatch();
                return BufferProxy.Buffer.Keys;
            }
        }

        public IEnumerable<TV> Values
        {
            get
            {
                ThrowIfVersionNotMatch();
                return (BufferProxy.Buffer).Values;
            }
        }
    }
}