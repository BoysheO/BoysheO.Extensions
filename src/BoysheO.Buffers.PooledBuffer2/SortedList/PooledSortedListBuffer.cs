using System;
using System.Collections;
using System.Collections.Generic;
using BoysheO.Buffer.PooledBuffer;

namespace BoysheO.Buffers
{
    /// <summary>
    /// don't use again after disposable
    /// </summary>
    public readonly struct PooledSortedListBuffer<TK, TV> : IDisposable, IReadOnlyDictionary<TK, TV>,
        IDictionary<TK, TV>
    {
        internal readonly long Version;
        internal readonly UnsafePooledSortedList<TK, TV> Buffer;

        internal PooledSortedListBuffer(UnsafePooledSortedList<TK, TV> buffer)
        {
            Buffer = buffer;
            Version = buffer.LifeVersion;
        }

        public void Dispose()
        {
            if (Buffer != null && Version == Buffer.LifeVersion)
            {
                PooledSortedListBufferPool<TK, TV>.Share.Return(this);
            }
        }

        public static PooledSortedListBuffer<TK, TV> Rent(IComparer<TK> comparer)
        {
            if (comparer == null) throw new ArgumentNullException(nameof(comparer));
            return PooledSortedListBufferPool<TK, TV>.Share.Rent(comparer);
        }

        private void ThrowIfVersionNotMatch()
        {
            if (Buffer == null || Version != Buffer.LifeVersion)
                throw new ObjectDisposedException("this buffer is disposed");
        }

        public PooledSortedListBufferEnumerator<TK, TV> GetEnumerator()
        {
            ThrowIfVersionNotMatch();
            return Buffer.GetEnumerator();
        }

        IEnumerator<KeyValuePair<TK, TV>> IEnumerable<KeyValuePair<TK, TV>>.GetEnumerator()
        {
            ThrowIfVersionNotMatch();
            return Buffer.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(KeyValuePair<TK, TV> item)
        {
            ThrowIfVersionNotMatch();
            Buffer.Add(item.Key, item.Value);
        }

        public void Clear()
        {
            ThrowIfVersionNotMatch();
            Buffer.Clear();
        }

        bool ICollection<KeyValuePair<TK, TV>>.Contains(KeyValuePair<TK, TV> item)
        {
            return ((ICollection<KeyValuePair<TK, TV>>) Buffer).Contains(item);
        }

        public void CopyTo(KeyValuePair<TK, TV>[] array, int arrayIndex)
        {
            ThrowIfVersionNotMatch();
            Buffer.CopyTo(array, arrayIndex);
        }

        bool ICollection<KeyValuePair<TK, TV>>.Remove(KeyValuePair<TK, TV> item)
        {
            ThrowIfVersionNotMatch();
            return ((ICollection<KeyValuePair<TK, TV>>) Buffer).Remove(item);
        }

        public int Count
        {
            get
            {
                ThrowIfVersionNotMatch();
                return Buffer.Count;
            }
        }

        bool ICollection<KeyValuePair<TK, TV>>.IsReadOnly => false;

        public void Add(TK key, TV value)
        {
            ThrowIfVersionNotMatch();
            Buffer.Add(key, value);
        }

        public bool ContainsKey(TK key)
        {
            ThrowIfVersionNotMatch();
            return Buffer.ContainsKey(key);
        }

        public bool Remove(TK key)
        {
            ThrowIfVersionNotMatch();
            return Buffer.Remove(key);
        }

        public bool TryGetValue(TK key, out TV value)
        {
            ThrowIfVersionNotMatch();
            return Buffer.TryGetValue(key, out value);
        }

        public TV this[TK key]
        {
            get
            {
                ThrowIfVersionNotMatch();
                return Buffer[key];
            }
            set
            {
                ThrowIfVersionNotMatch();
                Buffer[key] = value;
            }
        }

        IEnumerable<TK> IReadOnlyDictionary<TK, TV>.Keys
        {
            get
            {
                ThrowIfVersionNotMatch();
                return Buffer.Keys;
            }
        }

        IEnumerable<TV> IReadOnlyDictionary<TK, TV>.Values
        {
            get
            {
                ThrowIfVersionNotMatch();
                return Buffer.Values;
            }
        }

        ICollection<TK> IDictionary<TK, TV>.Keys
        {
            get
            {
                ThrowIfVersionNotMatch();
                return Buffer.Keys;
            }
        }

        ICollection<TV> IDictionary<TK, TV>.Values
        {
            get
            {
                ThrowIfVersionNotMatch();
                return Buffer.Values;
            }
        }

        public IReadOnlyPooledList<TK> Keys
        {
            get
            {
                ThrowIfVersionNotMatch();
                return Buffer.Keys;
            }
        }

        public IReadOnlyPooledList<TV> Values
        {
            get
            {
                ThrowIfVersionNotMatch();
                return Buffer.Values;
            }
        }
    }
}