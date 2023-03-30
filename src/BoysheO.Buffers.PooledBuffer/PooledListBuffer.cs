using System;
using System.Collections;
using System.Collections.Generic;
using Collections.Pooled;

namespace BoysheO.Buffers
{
    /// <summary>
    /// don't use again after disposable
    /// </summary>
    public readonly struct PooledListBuffer<T> : IDisposable, IList<T>, IList, IReadOnlyList<T>
    {
        internal readonly int Version;
        internal readonly ListProxy<T> ListProxy;
        private PooledList<T> _Buffer => ListProxy.List;

        internal PooledListBuffer(ListProxy<T> listProxy)
        {
            Version = listProxy.Version;
            ListProxy = listProxy;
        }

        public void Dispose()
        {
            if (_Buffer != null && Version == ListProxy.Version)
            {
                PooledListBufferPool<T>.Share.Return(this);
            }
        }

        public static PooledListBuffer<T> Rent()
        {
            return PooledListBufferPool<T>.Share.Rent();
        }

        private void ThrowIfVersionNotMatch()
        {
            if (_Buffer == null || Version != ListProxy.Version)
                throw new ObjectDisposedException("this buffer is disposed");
        }

        public int Capacity
        {
            get
            {
                ThrowIfVersionNotMatch();
                return _Buffer.Capacity;
            }
            set
            {
                ThrowIfVersionNotMatch();
                _Buffer.Capacity = value;
            }
        }

        /// <summary>
        /// Adds the elements of the given collection to the end of this list. If
        /// required, the capacity of the list is increased to twice the previous
        /// capacity or the new size, whichever is larger.
        /// </summary>
        public void AddRange(IEnumerable<T> collection)
        {
            ThrowIfVersionNotMatch();
            _Buffer.AddRange(collection);
        }

        /// <summary>
        /// Adds the elements of the given array to the end of this list. If
        /// required, the capacity of the list is increased to twice the previous
        /// capacity or the new size, whichever is larger.
        /// </summary>
        public void AddRange(T[] array)
        {
            ThrowIfVersionNotMatch();
            _Buffer.AddRange(array);
        }

        /// <summary>
        /// design for ArrayPool&lt;T&gt;
        /// </summary>
        public void AddRange(T[] ary, int count)
        {
            ThrowIfVersionNotMatch();
            _Buffer.AddRange(ary.AsSpan(0, count));
        }

        public void AddRange(ReadOnlySpan<T> span)
        {
            ThrowIfVersionNotMatch();
            _Buffer.AddRange(span);
        }

        /// <summary>
        /// Searches a section of the list for a given element using a binary search
        /// algorithm.
        /// </summary>
        /// <remarks><para>Elements of the list are compared to the search value using
        /// the given IComparer interface. If comparer is null, elements of
        /// the list are compared to the search value using the IComparable
        /// interface, which in that case must be implemented by all elements of the
        /// list and the given search value. This method assumes that the given
        /// section of the list is already sorted; if this is not the case, the
        /// result will be incorrect.</para>
        /// 
        /// <para>The method returns the index of the given value in the list. If the
        /// list does not contain the given value, the method returns a negative
        /// integer. The bitwise complement operator (~) can be applied to a
        /// negative result to produce the index of the first element (if any) that
        /// is larger than the given search value. This is also the index at which
        /// the search value should be inserted into the list in order for the list
        /// to remain sorted.
        /// </para></remarks>
        public int BinarySearch(int index, int count, T item, IComparer<T> comparer)
        {
            ThrowIfVersionNotMatch();
            return _Buffer.BinarySearch(index, count, item, comparer);
        }

        /// <summary>
        /// Searches the list for a given element using a binary search
        /// algorithm. If the item implements <see cref="T:System.IComparable`1" />
        /// then that is used for comparison, otherwise <see cref="P:System.Collections.Generic.Comparer`1.Default" /> is used.
        /// </summary>
        public int BinarySearch(T item)
        {
            ThrowIfVersionNotMatch();
            return _Buffer.BinarySearch(item);
        }

        /// <summary>
        /// Searches the list for a given element using a binary search
        /// algorithm. If the item implements <see cref="T:System.IComparable`1" />
        /// then that is used for comparison, otherwise <see cref="P:System.Collections.Generic.Comparer`1.Default" /> is used.
        /// </summary>
        public int BinarySearch(T item, IComparer<T> comparer)
        {
            ThrowIfVersionNotMatch();
            return _Buffer.BinarySearch(item, comparer);
        }

        public Span<T> Span
        {
            get
            {
                ThrowIfVersionNotMatch();
                return _Buffer.Span;
            }
        }

        public Span<T> GetSpanAdding(int capacity)
        {
            ThrowIfVersionNotMatch();
            return _Buffer.AddSpan(capacity);
        }

        public Span<T> GetSpanInserting(int idx, int count)
        {
            ThrowIfVersionNotMatch();
            return _Buffer.InsertSpan(idx, count);
        }

        /// <summary>
        /// Sorts the elements in this list.  Uses the default comparer and
        /// Array.Sort.
        /// </summary>
        public void Sort()
        {
            ThrowIfVersionNotMatch();
            _Buffer.Sort();
        }

        /// <summary>
        /// Sorts the elements in this list.  Uses Array.Sort with the
        /// provided comparer.
        /// </summary>
        /// <param name="comparer"></param>
        public void Sort(IComparer<T> comparer)
        {
            ThrowIfVersionNotMatch();
            _Buffer.Sort(comparer);
        }

        /// <summary>
        /// Sorts the elements in a section of this list. The sort compares the
        /// elements to each other using the given IComparer interface. If
        /// comparer is null, the elements are compared to each other using
        /// the IComparable interface, which in that case must be implemented by all
        /// elements of the list.
        /// 
        /// This method uses the Array.Sort method to sort the elements.
        /// </summary>
        public void Sort(int index, int count, IComparer<T> comparer)
        {
            ThrowIfVersionNotMatch();
            _Buffer.Sort(index, count, comparer);
        }

        public void Sort(Func<T, T, int> comparison)
        {
            ThrowIfVersionNotMatch();
            _Buffer.Sort(comparison);
        }

        public PooledListBuffer<TOutput> ConvertAll<TOutput>(Func<T, TOutput> converter)
        {
            ThrowIfVersionNotMatch();
            if (converter == null) throw new ArgumentNullException(nameof(converter));
            var res = PooledListBuffer<TOutput>.Rent();
            for (int i = 0, count = _Buffer.Count; i < count; i++)
            {
                res[i] = converter(_Buffer[i]);
            }

            return res;
        }

        /// <summary>Copies this list to the given span.</summary>
        public void CopyTo(System.Span<T> span)
        {
            ThrowIfVersionNotMatch();
            _Buffer.CopyTo(span);
        }

        public bool Exists(Func<T, bool> match)
        {
            ThrowIfVersionNotMatch();
            return _Buffer.Exists(match);
        }

        public bool TryFind(Func<T, bool> match, out T result)
        {
            ThrowIfVersionNotMatch();
            return _Buffer.TryFind(match, out result);
        }

        public PooledListBuffer<T> FindAll(Func<T, bool> match)
        {
            ThrowIfVersionNotMatch();
            var lst = PooledListBuffer<T>.Rent();
            foreach (var item in _Buffer)
            {
                if (match(item)) lst.Add(item);
            }

            return lst;
        }

        public int FindIndex(Func<T, bool> match)
        {
            ThrowIfVersionNotMatch();
            return _Buffer.FindIndex(match);
        }

        public int FindIndex(int startIndex, Func<T, bool> match)
        {
            ThrowIfVersionNotMatch();
            return _Buffer.FindIndex(startIndex, match);
        }

        public int FindIndex(int startIndex, int count, Func<T, bool> match)
        {
            ThrowIfVersionNotMatch();
            return _Buffer.FindIndex(startIndex, count, match);
        }

        public bool TryFindLast(Func<T, bool> match, out T result)
        {
            ThrowIfVersionNotMatch();
            return _Buffer.TryFindLast(match, out result);
        }

        public int FindLastIndex(Func<T, bool> match)
        {
            ThrowIfVersionNotMatch();
            return _Buffer.FindLastIndex(match);
        }

        public int FindLastIndex(int startIndex, Func<T, bool> match)
        {
            ThrowIfVersionNotMatch();
            return _Buffer.FindLastIndex(startIndex, match);
        }

        public int FindLastIndex(int startIndex, int count, Func<T, bool> match)
        {
            ThrowIfVersionNotMatch();
            return _Buffer.FindLastIndex(startIndex, count, match);
        }

        /// <summary>
        /// Returns the index of the last occurrence of a given value in a range of
        /// this list. The list is searched backwards, starting at the end
        /// and ending at the first element in the list.
        /// </summary>
        public int LastIndexOf(T item)
        {
            ThrowIfVersionNotMatch();
            return _Buffer.LastIndexOf(item);
        }

        /// <summary>
        /// Returns the index of the last occurrence of a given value in a range of
        /// this list. The list is searched backwards, starting at index
        /// index and ending at the first element in the list.
        /// </summary>
        public int LastIndexOf(T item, int index)
        {
            ThrowIfVersionNotMatch();
            return _Buffer.LastIndexOf(item, index);
        }

        /// <summary>
        /// Returns the index of the last occurrence of a given value in a range of
        /// this list. The list is searched backwards, starting at index
        /// index and upto count elements
        /// </summary>
        public int LastIndexOf(T item, int index, int count)
        {
            ThrowIfVersionNotMatch();
            return _Buffer.LastIndexOf(item, index, count);
        }


        /// <summary>
        /// Inserts the elements of the given collection at a given index. If
        /// required, the capacity of the list is increased to twice the previous
        /// capacity or the new size, whichever is larger.  Ranges may be added
        /// to the end of the list by setting index to the List's size.
        /// </summary>
        public void InsertRange(int index, IEnumerable<T> collection)
        {
            ThrowIfVersionNotMatch();
            _Buffer.InsertRange(index, collection);
        }

        /// <summary>
        /// Inserts the elements of the given collection at a given index. If
        /// required, the capacity of the list is increased to twice the previous
        /// capacity or the new size, whichever is larger.  Ranges may be added
        /// to the end of the list by setting index to the List's size.
        /// </summary>
        public void InsertRange(int index, ReadOnlySpan<T> span)
        {
            ThrowIfVersionNotMatch();
            _Buffer.InsertRange(index, span);
        }

        /// <summary>
        /// Inserts the elements of the given collection at a given index. If
        /// required, the capacity of the list is increased to twice the previous
        /// capacity or the new size, whichever is larger.  Ranges may be added
        /// to the end of the list by setting index to the List's size.
        /// </summary>
        public void InsertRange(int index, T[] array)
        {
            ThrowIfVersionNotMatch();
            _Buffer.InsertRange(index, array);
        }

        /// <summary>
        /// This method removes all items which match the predicate.
        /// The complexity is O(n).
        /// </summary>
        public int RemoveAll(Func<T, bool> match)
        {
            ThrowIfVersionNotMatch();
            return _Buffer.RemoveAll(match);
        }

        /// <summary>Reverses the elements in this list.</summary>
        public void Reverse()
        {
            ThrowIfVersionNotMatch();
            _Buffer.Reverse();
        }

        /// <summary>
        /// Reverses the elements in a range of this list. Following a call to this
        /// method, an element in the range given by index and count
        /// which was previously located at index i will now be located at
        /// index index + (index + count - i - 1).
        /// </summary>
        public void Reverse(int index, int count)
        {
            ThrowIfVersionNotMatch();
            _Buffer.Reverse(index, count);
        }

        /// <summary>
        /// Sets the capacity of this list to the size of the list. This method can
        /// be used to minimize a list's memory overhead once it is known that no
        /// new elements will be added to the list. To completely clear a list and
        /// release all memory referenced by the list, execute the following
        /// statements:
        /// <code>
        /// list.Clear();
        /// list.TrimExcess();
        /// </code>
        /// </summary>
        public void TrimExcess()
        {
            ThrowIfVersionNotMatch();
            _Buffer.TrimExcess();
        }

        public bool TrueForAll(Func<T, bool> match)
        {
            ThrowIfVersionNotMatch();
            return _Buffer.TrueForAll(match);
        }

        /// <summary>
        /// ToArray returns an array containing the contents of the List.
        /// This requires copying the List, which is an O(n) operation.
        /// </summary>
        public T[] ToArray()
        {
            ThrowIfVersionNotMatch();
            return _Buffer.ToArray();
        }

        #region interface

        public IEnumerator<T> GetEnumerator()
        {
            ThrowIfVersionNotMatch();
            return _Buffer.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            ThrowIfVersionNotMatch();
            return ((IEnumerable)_Buffer).GetEnumerator();
        }

        public void Add(T item)
        {
            ThrowIfVersionNotMatch();
            _Buffer.Add(item);
        }

        int IList.Add(object value)
        {
            ThrowIfVersionNotMatch();
            return ((IList)_Buffer).Add(value);
        }

        void IList.Clear()
        {
            ThrowIfVersionNotMatch();
            ((IList)_Buffer).Clear();
        }

        bool IList.Contains(object value)
        {
            ThrowIfVersionNotMatch();
            return ((IList)_Buffer).Contains(value);
        }

        int IList.IndexOf(object value)
        {
            ThrowIfVersionNotMatch();
            return ((IList)_Buffer).IndexOf(value);
        }

        void IList.Insert(int index, object value)
        {
            ThrowIfVersionNotMatch();
            ((IList)_Buffer).Insert(index, value);
        }

        void IList.Remove(object value)
        {
            ThrowIfVersionNotMatch();
            ((IList)_Buffer).Remove(value);
        }

        void IList.RemoveAt(int index)
        {
            ThrowIfVersionNotMatch();
            ((IList)_Buffer).RemoveAt(index);
        }

        bool IList.IsFixedSize
        {
            get
            {
                ThrowIfVersionNotMatch();
                return ((IList)_Buffer).IsFixedSize;
            }
        }

        bool IList.IsReadOnly
        {
            get
            {
                ThrowIfVersionNotMatch();
                return ((IList)_Buffer).IsReadOnly;
            }
        }

        object IList.this[int index]
        {
            get
            {
                ThrowIfVersionNotMatch();
                return ((IList)_Buffer)[index];
            }
            set
            {
                ThrowIfVersionNotMatch();
                ((IList)_Buffer)[index] = value;
            }
        }

        void ICollection<T>.Clear()
        {
            ThrowIfVersionNotMatch();
            ((ICollection<T>)_Buffer).Clear();
        }

        public bool Contains(T item)
        {
            ThrowIfVersionNotMatch();
            return _Buffer.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            ThrowIfVersionNotMatch();
            _Buffer.CopyTo(array.AsSpan(arrayIndex));
        }

        public bool Remove(T item)
        {
            ThrowIfVersionNotMatch();
            return _Buffer.Remove(item);
        }

        void ICollection.CopyTo(Array array, int index)
        {
            ThrowIfVersionNotMatch();
            ((ICollection)_Buffer).CopyTo(array, index);
        }

        public int Count
        {
            get
            {
                ThrowIfVersionNotMatch();
                return ((ICollection)_Buffer).Count;
            }
        }

        bool ICollection.IsSynchronized
        {
            get
            {
                ThrowIfVersionNotMatch();
                return ((ICollection)_Buffer).IsSynchronized;
            }
        }

        object ICollection.SyncRoot
        {
            get
            {
                ThrowIfVersionNotMatch();
                return ((ICollection)_Buffer).SyncRoot;
            }
        }

        int ICollection<T>.Count
        {
            get
            {
                ThrowIfVersionNotMatch();
                return _Buffer.Count;
            }
        }

        bool ICollection<T>.IsReadOnly
        {
            get
            {
                ThrowIfVersionNotMatch();
                return ((ICollection<T>)_Buffer).IsReadOnly;
            }
        }

        public int IndexOf(T item)
        {
            ThrowIfVersionNotMatch();
            return _Buffer.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            ThrowIfVersionNotMatch();
            _Buffer.Insert(index, item);
        }

        void IList<T>.RemoveAt(int index)
        {
            ThrowIfVersionNotMatch();
            ((IList<T>)_Buffer).RemoveAt(index);
        }

        public T this[int index]
        {
            get
            {
                ThrowIfVersionNotMatch();
                return _Buffer[index];
            }
            set
            {
                ThrowIfVersionNotMatch();
                _Buffer[index] = value;
            }
        }

        int IReadOnlyCollection<T>.Count
        {
            get
            {
                ThrowIfVersionNotMatch();
                return _Buffer.Count;
            }
        }

        #endregion
    }
}