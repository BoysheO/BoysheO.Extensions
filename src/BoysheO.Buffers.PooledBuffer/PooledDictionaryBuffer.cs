using System;
using System.Collections;
using System.Collections.Generic;
using Collections.Pooled;

namespace BoysheO.Buffers
{
    /// <summary>
    /// don't use again after disposable
    /// </summary>
    public readonly struct PooledDictionaryBuffer<TK, TV> : IDisposable, IReadOnlyDictionary<TK, TV>, IDictionary<TK, TV>, IDictionary
    {
        internal readonly int Version;
        internal readonly DictionaryProxy<TK, TV> ListProxy;
        private PooledDictionary<TK, TV> _Buffer => ListProxy.Map;

        private IReadOnlyDictionary<TK, TV> _ReadOnlyBuffer => ListProxy.Map;

        private IDictionary<TK, TV> _DictionaryBuffer => ListProxy.Map;

        private IDictionary _IDictionaryBuffer => ListProxy.Map;

        internal PooledDictionaryBuffer(DictionaryProxy<TK, TV> listProxy)
        {
            ListProxy = listProxy;
            Version = listProxy.Version;
        }

        public void Dispose()
        {
            if (_Buffer != null && Version == ListProxy.Version)
            {
                PooledDictionaryBufferPool<TK, TV>.Share.Return(this);
            }
        }

        public static PooledDictionaryBuffer<TK, TV> Rent()
        {
            return PooledDictionaryBufferPool<TK, TV>.Share.Rent();
        }

        private void ThrowIfVersionNotMatch()
        {
            if (_Buffer == null || Version != ListProxy.Version)
                throw new ObjectDisposedException("this buffer is disposed");
        }

        public PooledDictionary<TK, TV>.Enumerator GetEnumerator()
        {
            ThrowIfVersionNotMatch();
            return _Buffer.GetEnumerator();
        }
        
        bool IDictionary.Contains(object key)
        {
            ThrowIfVersionNotMatch();
            return _IDictionaryBuffer.Contains(key);
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            ThrowIfVersionNotMatch();
            return _IDictionaryBuffer.GetEnumerator();
        }

        void IDictionary.Remove(object key)
        {
            ThrowIfVersionNotMatch();
            _IDictionaryBuffer.Remove(key);
        }

        bool IDictionary.IsFixedSize
        {
            get
            {
                ThrowIfVersionNotMatch();
                return _IDictionaryBuffer.IsFixedSize;
            }
        }

        IEnumerator<KeyValuePair<TK, TV>> IEnumerable<KeyValuePair<TK,TV>>.GetEnumerator()
        {
            ThrowIfVersionNotMatch();
            return _ReadOnlyBuffer.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(KeyValuePair<TK, TV> item)
        {
            ThrowIfVersionNotMatch();
            _DictionaryBuffer.Add(item);
        }

        void IDictionary.Add(object key, object value)
        {
            ThrowIfVersionNotMatch();
            _IDictionaryBuffer.Add(key, value);
        }

        public void Clear()
        {
            ThrowIfVersionNotMatch();
            _DictionaryBuffer.Clear();
        }

        public bool Contains(KeyValuePair<TK, TV> item)
        {
            ThrowIfVersionNotMatch();
            return _DictionaryBuffer.Contains(item);
        }

        public void CopyTo(KeyValuePair<TK, TV>[] array, int arrayIndex)
        {
            ThrowIfVersionNotMatch();
            _DictionaryBuffer.CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<TK, TV> item)
        {
            ThrowIfVersionNotMatch();
            return _DictionaryBuffer.Remove(item);
        }

        void ICollection.CopyTo(Array array, int index)
        {
            ThrowIfVersionNotMatch();
            _IDictionaryBuffer.CopyTo(array, index);
        }

        public int Count
        {
            get
            {
                ThrowIfVersionNotMatch();
                return _ReadOnlyBuffer.Count;
            }
        }

        bool ICollection.IsSynchronized
        {
            get
            {
                ThrowIfVersionNotMatch();
                return _IDictionaryBuffer.IsSynchronized;
            }
        }

        object ICollection.SyncRoot
        {
            get
            {
                ThrowIfVersionNotMatch();
                return _IDictionaryBuffer.SyncRoot;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                ThrowIfVersionNotMatch();
                return _DictionaryBuffer.IsReadOnly;
            }
        }

        object IDictionary.this[object key]
        {
            get
            {
                ThrowIfVersionNotMatch();
                return _IDictionaryBuffer[key];
            }
            set
            {
                ThrowIfVersionNotMatch();
                _IDictionaryBuffer[key] = value;
            }
        }

        public void Add(TK key, TV value)
        {
            ThrowIfVersionNotMatch();
            _DictionaryBuffer.Add(key, value);
        }

        public bool ContainsKey(TK key)
        {
            ThrowIfVersionNotMatch();
            return _ReadOnlyBuffer.ContainsKey(key);
        }

        public bool Remove(TK key)
        {
            ThrowIfVersionNotMatch();
            return _DictionaryBuffer.Remove(key);
        }

        public bool TryGetValue(TK key, out TV value)
        {
            ThrowIfVersionNotMatch();
            return _ReadOnlyBuffer.TryGetValue(key, out value);
        }

        public TV this[TK key]
        {
            get
            {
                ThrowIfVersionNotMatch();
                return _ReadOnlyBuffer[key];
            }
            set
            {
                ThrowIfVersionNotMatch();
                _DictionaryBuffer[key] = value;
            }
        }

        public IEnumerable<TK> Keys
        {
            get
            {
                ThrowIfVersionNotMatch();
                return _ReadOnlyBuffer.Keys;
            }
        }

        ICollection IDictionary.Values
        {
            get
            {
                ThrowIfVersionNotMatch();
                return _IDictionaryBuffer.Values;
            }
        }

        ICollection IDictionary.Keys
        {
            get
            {
                ThrowIfVersionNotMatch();
                return _IDictionaryBuffer.Keys;
            }
        }

        ICollection<TV> IDictionary<TK, TV>.Values
        {
            get
            {
                ThrowIfVersionNotMatch();
                return _DictionaryBuffer.Values;
            }
        }

        ICollection<TK> IDictionary<TK, TV>.Keys
        {
            get
            {
                ThrowIfVersionNotMatch();
                return _DictionaryBuffer.Keys;
            }
        }

        public IEnumerable<TV> Values
        {
            get
            {
                ThrowIfVersionNotMatch();
                return _ReadOnlyBuffer.Values;
            }
        }
    }
}