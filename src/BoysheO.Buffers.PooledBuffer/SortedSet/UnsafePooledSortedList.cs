using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Collections.Pooled;

namespace BoysheO.Buffer.PooledBuffer
{
    internal sealed class UnsafePooledSortedList<TK, TV> :
        IDictionary<TK, TV>,
        IReadOnlyDictionary<TK, TV>,
        IReadOnlyList<KeyValuePair<TK, TV>>,
        IDisposable
    {
        internal IComparer<TK> Comparer;
        internal readonly PooledList<TK> Keys = new PooledList<TK>();
        internal readonly PooledList<TV> Values = new PooledList<TV>();

        /// <summary>
        /// increase every dispose
        /// </summary>
        public long LifeVersion;

        internal PooledSortedListBufferEnumerator<TK, TV> GetEnumerator()
        {
            return new PooledSortedListBufferEnumerator<TK, TV>(this);
        }

        IEnumerator<KeyValuePair<TK, TV>> IEnumerable<KeyValuePair<TK, TV>>.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        void ICollection<KeyValuePair<TK, TV>>.Add(KeyValuePair<TK, TV> item)
        {
            Add(item.Key, item.Value);
        }

        public void Clear()
        {
            Keys.Clear();
            Values.Clear();
        }

        bool ICollection<KeyValuePair<TK, TV>>.Contains(KeyValuePair<TK, TV> item)
        {
            var idx = Keys.BinarySearch(item.Key, Comparer);
            if (idx < 0) return false;
            var value = Values[idx];
            return EqualityComparer<TV>.Default.Equals(value, item.Value);
        }

        public void CopyTo(KeyValuePair<TK, TV>[] array, int arrayIndex)
        {
            var keySpan = Keys.Span;
            var valueSpan = Values.Span;
            var targetSpan = array.AsSpan(arrayIndex);
            var count = keySpan.Length;
            for (var i = 0; i < count; i++)
            {
                targetSpan[arrayIndex + i] = new KeyValuePair<TK, TV>(keySpan[i], valueSpan[i]);
            }
        }

        public int IndexOfKey(TK key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));
            int ret = Keys.BinarySearch(key, Comparer);
            return ret >= 0 ? ret : -1;
        }

        bool ICollection<KeyValuePair<TK, TV>>.Remove(KeyValuePair<TK, TV> item)
        {
            var idx = Keys.BinarySearch(item.Key, Comparer);
            if (idx < 0) return false;
            var value = Values[idx];
            if (!EqualityComparer<TV>.Default.Equals(value, item.Value)) return false;
            Keys.RemoveAt(idx);
            Values.RemoveAt(idx);
            return true;
        }

        public int Count => Keys.Count;

        bool ICollection<KeyValuePair<TK, TV>>.IsReadOnly => false;

        public void Add(TK key, TV value)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));
            var idx = Keys.BinarySearch(key, Comparer);
            if (idx >= 0)
                throw new ArgumentException("key already exists");
            idx = ~idx;
            Keys.Insert(idx, key);
            Values.Insert(idx, value);
        }

        public bool ContainsKey(TK key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));
            return Keys.BinarySearch(key, Comparer) >= 0;
        }

        public bool TryGetValue(TK key, out TV value)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));
            var idx = Keys.BinarySearch(key, Comparer);
            if (idx < 0)
            {
                value = default;
                return false;
            }

            value = Values[idx];
            return true;
        }

        public TV this[TK key]
        {
            get
            {
                if (!TryGetValue(key, out var value)) throw new KeyNotFoundException(key.ToString());
                return value;
            }
            set
            {
                if (key == null)
                    throw new ArgumentNullException(nameof(key));
                var idx = Keys.BinarySearch(key, Comparer);
                if (idx < 0)
                {
                    idx = ~idx;
                    Keys.Insert(idx, key);
                    Values.Insert(idx, value);
                }
                else
                {
                    Values[idx] = value;
                }
            }
        }

        IEnumerable<TK> IReadOnlyDictionary<TK, TV>.Keys => Keys;

        IEnumerable<TV> IReadOnlyDictionary<TK, TV>.Values => Values;

        ICollection<TK> IDictionary<TK, TV>.Keys => Keys;

        ICollection<TV> IDictionary<TK, TV>.Values => Values;

        public bool Remove(TK key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));
            var idx = Keys.BinarySearch(key, Comparer);
            if (idx < 0) return false;
            Keys.RemoveAt(idx);
            Values.RemoveAt(idx);
            return true;
        }

        public void Dispose()
        {
            Keys.Dispose();
            Values.Dispose();
            Comparer = null;
        }

        KeyValuePair<TK, TV> IReadOnlyList<KeyValuePair<TK, TV>>.this[int index] =>
            new KeyValuePair<TK, TV>(Keys[index], Values[index]);
    }
}