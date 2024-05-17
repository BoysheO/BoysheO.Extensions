using System;
using System.Collections;
using System.Collections.Generic;

namespace BoysheO.Buffers
{
    /// <summary>
    /// don't use again after disposable
    /// </summary>
    public readonly struct PooledListBuffer<T> : IDisposable, IList<T>, IList, IReadOnlyList<T>
    {
        private readonly long _version;
        private readonly SimplePooledList<T> _buffer;

        private PooledListBuffer(long version, SimplePooledList<T> buffer)
        {
            _version = version;
            _buffer = buffer;
        }


        /// <summary>
        /// NoBoxing IReadOnlyList(but call IEnumerable.GetEnumerator() will boxing the internal enumerator)
        /// *DON'T keep the instance returned,using the instance after buff disposed cause undefined behavior* 
        /// </summary>
        public SimplePooledList<T> InternalBuffer
        {
            get => _buffer;
        }

        public void Dispose()
        {
            if (IsAlive)
            {
                _buffer.Clear();
                ObjectPool<SimplePooledList<T>>.Share.Return(_buffer);
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
                return _buffer != null && _version != 0 &&
                       ObjectPool<SimplePooledList<T>>.Share.GetVersion(_buffer) == _version;
            }
        }

        public static PooledListBuffer<T> Rent()
        {
            var ins = ObjectPool<SimplePooledList<T>>.Share.Rent(out var version);
            return new PooledListBuffer<T>(version, ins);
        }

        private void ThrowIfVersionNotMatch()
        {
            if (!IsAlive) throw new ObjectDisposedException("this buffer is disposed");
        }

        public void Push(T item)
        {
            Add(item);
        }

        public T Pop()
        {
            if (!TryPop(out var item)) throw new InvalidOperationException("stack is empty");
            return item;
        }

        public bool TryPop(out T item)
        {
            var r = TryPeek(out item);
            if (r) _buffer.RemoveAt(_buffer.Count - 1);
            return r;
        }

        public bool TryPeek(out T item)
        {
            if (Count == 0)
            {
                item = default;
                return false;
            }

            var lst = _buffer;
            var idx = lst.Count - 1;
            item = lst[idx];
            return true;
        }

        /// <summary>
        /// remove duplicates by loop.element order is not changed.
        /// O(n*n)
        /// </summary>
        public void RemoveDuplicates()
        {
            RemoveDuplicates(EqualityComparer<T>.Default);
        }

        /// <summary>
        /// remove duplicates by loop.element order is not changed.
        /// O(n*n)
        /// </summary>
        public void RemoveDuplicates(Func<T, T, bool> comparer)
        {
            ThrowIfVersionNotMatch();
            var lst = _buffer;
            for (int i = 0; i < lst.Count; i++)
            {
                for (int j = i + 1; j < lst.Count; j++)
                {
                    if (comparer(lst[i], lst[j]))
                    {
                        lst.RemoveAt(j);
                        j--;
                    }
                }
            }
        }

        /// <summary>
        /// remove duplicates by loop.element order is not changed.
        /// O(n*n)
        /// </summary>
        public void RemoveDuplicates(IEqualityComparer<T> comparer)
        {
            ThrowIfVersionNotMatch();
            var lst = _buffer;
            for (int i = 0; i < lst.Count; i++)
            {
                for (int j = i + 1; j < lst.Count; j++)
                {
                    if (comparer.Equals(lst[i], lst[j]))
                    {
                        lst.RemoveAt(j);
                        j--;
                    }
                }
            }
        }

        public void RemoveLast()
        {
            ThrowIfVersionNotMatch();
            var lst = _buffer;
            lst.RemoveAt(lst.Count - 1);
        }


        /// <summary>
        /// Adds the elements of the given array to the end of this list. If
        /// required, the capacity of the list is increased to twice the previous
        /// capacity or the new size, whichever is larger.
        /// </summary>
        public PooledListBuffer<T> AddRange(T[] array)
        {
            ThrowIfVersionNotMatch();
            _buffer.AddRange(array);
            return this;
        }

        /// <summary>
        /// design for ArrayPool&lt;T&gt;
        /// </summary>
        public PooledListBuffer<T> AddRange(T[] ary, int count)
        {
            ThrowIfVersionNotMatch();
            _buffer.AddRange(ary.AsSpan(0, count));
            return this;
        }

        public PooledListBuffer<T> AddRange(ReadOnlySpan<T> span)
        {
            ThrowIfVersionNotMatch();
            _buffer.AddRange(span);
            return this;
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
            return _buffer.BinarySearch(index, count, item, comparer);
        }

        /// <summary>
        /// Searches the list for a given element using a binary search
        /// algorithm. If the item implements <see cref="T:System.IComparable`1" />
        /// then that is used for comparison, otherwise <see cref="P:System.Collections.Generic.Comparer`1.Default" /> is used.
        /// </summary>
        public int BinarySearch(T item)
        {
            ThrowIfVersionNotMatch();
            return _buffer.BinarySearch(0, _buffer.Count, item, Comparer<T>.Default);
        }

        /// <summary>
        /// Searches the list for a given element using a binary search
        /// algorithm. If the item implements <see cref="T:System.IComparable`1" />
        /// then that is used for comparison, otherwise <see cref="P:System.Collections.Generic.Comparer`1.Default" /> is used.
        /// </summary>
        public int BinarySearch(T item, IComparer<T> comparer)
        {
            ThrowIfVersionNotMatch();
            return _buffer.BinarySearch(0, _buffer.Count, item, comparer);
        }

        public Span<T> Span
        {
            get
            {
                ThrowIfVersionNotMatch();
                return _buffer.AsSpan;
            }
        }

        /// <summary>
        /// Sorts the elements in this list.  Uses the default comparer and
        /// Array.Sort.
        /// </summary>
        public PooledListBuffer<T> Sort()
        {
            ThrowIfVersionNotMatch();
            _buffer.Sort(0, _buffer.Count, Comparer<T>.Default);
            return this;
        }

        /// <summary>
        /// Sorts the elements in this list.  Uses Array.Sort with the
        /// provided comparer.
        /// </summary>
        /// <param name="comparer"></param>
        public PooledListBuffer<T> Sort(IComparer<T> comparer)
        {
            ThrowIfVersionNotMatch();
            _buffer.Sort(0, _buffer.Count, comparer);
            return this;
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
        public PooledListBuffer<T> Sort(int index, int count, IComparer<T> comparer)
        {
            ThrowIfVersionNotMatch();
            _buffer.Sort(index, count, comparer);
            return this;
        }

        public PooledListBuffer<T> Sort(Func<T, T, int> comparison)
        {
            ThrowIfVersionNotMatch();
            _buffer.Sort(0, _buffer.Count, Comparer<T>.Create((a, b) => comparison(a, b)));
            return this;
        }

        public bool All(Func<T, bool> predicate)
        {
            ThrowIfVersionNotMatch();
            return _buffer.Buffer.All(predicate);
        }

        public bool Any(Func<T, bool> predicate)
        {
            ThrowIfVersionNotMatch();
            return _buffer.Buffer.Any(predicate);
        }

        public T First()
        {
            ThrowIfVersionNotMatch();
            return _buffer.Buffer.First();
        }

        public T First(Func<T, bool> predicate)
        {
            ThrowIfVersionNotMatch();
            return _buffer.Buffer.First(predicate);
        }

        public T FirstOrDefault()
        {
            ThrowIfVersionNotMatch();
            return _buffer.Buffer.FirstOrDefault();
        }

        public T FirstOrDefault(Func<T, bool> predicate)
        {
            ThrowIfVersionNotMatch();
            return _buffer.Buffer.FirstOrDefault(predicate);
        }

        public T Last()
        {
            ThrowIfVersionNotMatch();
            return _buffer.Buffer.Last();
        }

        public T Last(Func<T, bool> predicate)
        {
            ThrowIfVersionNotMatch();
            return _buffer.Buffer.Last(predicate);
        }

        public T LastOrDefault()
        {
            ThrowIfVersionNotMatch();
            return _buffer.Buffer.LastOrDefault();
        }

        public T LastOrDefault(Func<T, bool> predicate)
        {
            ThrowIfVersionNotMatch();
            return _buffer.Buffer.LastOrDefault(predicate);
        }

        public PooledListBuffer<TOutput> ConvertAll<TOutput>(Func<T, TOutput> converter)
        {
            ThrowIfVersionNotMatch();
            if (converter == null) throw new ArgumentNullException(nameof(converter));
            var res = PooledListBuffer<TOutput>.Rent();
            for (int i = 0, count = _buffer.Buffer.Count; i < count; i++)
            {
                res[i] = converter(_buffer.Buffer[i]);
            }

            return res;
        }

        /// <summary>Copies this list to the given span.</summary>
        public void CopyTo(Span<T> span)
        {
            ThrowIfVersionNotMatch();
            _buffer.Buffer.CopyTo(span);
        }

        public bool Exists(Func<T, bool> match)
        {
            ThrowIfVersionNotMatch();
            return _buffer.Buffer.Exists(match);
        }

        public bool TryFind(Func<T, bool> match, out T result)
        {
            ThrowIfVersionNotMatch();
            return _buffer.Buffer.TryFind(match, out result);
        }

        public PooledListBuffer<T> FindAll(Func<T, bool> match)
        {
            ThrowIfVersionNotMatch();
            var lst = PooledListBuffer<T>.Rent();
            foreach (var item in _buffer.Buffer)
            {
                if (match(item)) lst.Add(item);
            }

            return lst;
        }

        public int FindIndex(Func<T, bool> match)
        {
            ThrowIfVersionNotMatch();
            return _buffer.Buffer.FindIndex(match);
        }

        public int FindIndex(int startIndex, Func<T, bool> match)
        {
            ThrowIfVersionNotMatch();
            return _buffer.Buffer.FindIndex(startIndex, match);
        }

        public int FindIndex(int startIndex, int count, Func<T, bool> match)
        {
            ThrowIfVersionNotMatch();
            return _buffer.Buffer.FindIndex(startIndex, count, match);
        }

        public bool TryFindLast(Func<T, bool> match, out T result)
        {
            ThrowIfVersionNotMatch();
            return _buffer.Buffer.TryFindLast(match, out result);
        }

        public int FindLastIndex(Func<T, bool> match)
        {
            ThrowIfVersionNotMatch();
            return _buffer.Buffer.FindLastIndex(match);
        }

        public int FindLastIndex(int startIndex, Func<T, bool> match)
        {
            ThrowIfVersionNotMatch();
            return _buffer.Buffer.FindLastIndex(startIndex, match);
        }

        public int FindLastIndex(int startIndex, int count, Func<T, bool> match)
        {
            ThrowIfVersionNotMatch();
            return _buffer.Buffer.FindLastIndex(startIndex, count, match);
        }

        /// <summary>
        /// Returns the index of the last occurrence of a given value in a range of
        /// this list. The list is searched backwards, starting at the end
        /// and ending at the first element in the list.
        /// </summary>
        public int LastIndexOf(T item)
        {
            ThrowIfVersionNotMatch();
            return _buffer.Buffer.LastIndexOf(item);
        }

        /// <summary>
        /// Returns the index of the last occurrence of a given value in a range of
        /// this list. The list is searched backwards, starting at index
        /// index and ending at the first element in the list.
        /// </summary>
        public int LastIndexOf(T item, int index)
        {
            ThrowIfVersionNotMatch();
            return _buffer.Buffer.LastIndexOf(item, index);
        }

        /// <summary>
        /// Returns the index of the last occurrence of a given value in a range of
        /// this list. The list is searched backwards, starting at index
        /// index and upto count elements
        /// </summary>
        public int LastIndexOf(T item, int index, int count)
        {
            ThrowIfVersionNotMatch();
            return _buffer.Buffer.LastIndexOf(item, index, count);
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
            _buffer.Buffer.InsertRange(index, collection);
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
            _buffer.Buffer.InsertRange(index, span);
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
            _buffer.Buffer.InsertRange(index, array);
        }

        /// <summary>
        /// This method removes all items which match the predicate.
        /// The complexity is O(n).
        /// </summary>
        public int RemoveAll(Func<T, bool> match)
        {
            ThrowIfVersionNotMatch();
            return _buffer.Buffer.RemoveAll(match);
        }

        /// <summary>Reverses the elements in this list.</summary>
        public void Reverse()
        {
            ThrowIfVersionNotMatch();
            _buffer.Buffer.Reverse();
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
            _buffer.Buffer.Reverse(index, count);
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
            _buffer.Buffer.TrimExcess();
        }

        public bool TrueForAll(Func<T, bool> match)
        {
            ThrowIfVersionNotMatch();
            return _buffer.Buffer.TrueForAll(match);
        }

        /// <summary>
        /// ToArray returns an array containing the contents of the List.
        /// This requires copying the List, which is an O(n) operation.
        /// </summary>
        public T[] ToArray()
        {
            ThrowIfVersionNotMatch();
            return _buffer.Buffer.ToArray();
        }

        public ListBufferEnumerator<T> GetEnumerator()
        {
            ThrowIfVersionNotMatch();
            return new ListBufferEnumerator<T>(_buffer, _buffer.Version);
        }

        #region interface

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            ThrowIfVersionNotMatch();
            return GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            ThrowIfVersionNotMatch();
            return GetEnumerator();
        }

        public void Add(T item)
        {
            ThrowIfVersionNotMatch();
            _buffer.Buffer.Add(item);
        }

        int IList.Add(object value)
        {
            ThrowIfVersionNotMatch();
            return ((IList) _buffer.Buffer).Add(value);
        }

        public void Clear()
        {
            ThrowIfVersionNotMatch();
            _buffer.Buffer.Clear();
        }

        bool IList.Contains(object value)
        {
            ThrowIfVersionNotMatch();
            return ((IList) _buffer.Buffer).Contains(value);
        }

        int IList.IndexOf(object value)
        {
            ThrowIfVersionNotMatch();
            return ((IList) _buffer.Buffer).IndexOf(value);
        }

        void IList.Insert(int index, object value)
        {
            ThrowIfVersionNotMatch();
            ((IList) _buffer.Buffer).Insert(index, value);
        }

        void IList.Remove(object value)
        {
            ThrowIfVersionNotMatch();
            ((IList) _buffer.Buffer).Remove(value);
        }

        void IList.RemoveAt(int index)
        {
            ThrowIfVersionNotMatch();
            ((IList) _buffer.Buffer).RemoveAt(index);
        }

        bool IList.IsFixedSize
        {
            get
            {
                ThrowIfVersionNotMatch();
                return ((IList) _buffer.Buffer).IsFixedSize;
            }
        }

        bool IList.IsReadOnly
        {
            get
            {
                ThrowIfVersionNotMatch();
                return ((IList) _buffer.Buffer).IsReadOnly;
            }
        }

        object IList.this[int index]
        {
            get
            {
                ThrowIfVersionNotMatch();
                return ((IList) _buffer.Buffer)[index];
            }
            set
            {
                ThrowIfVersionNotMatch();
                ((IList) _buffer.Buffer)[index] = value;
            }
        }

        void ICollection<T>.Clear()
        {
            ThrowIfVersionNotMatch();
            ((ICollection<T>) _buffer.Buffer).Clear();
        }

        public bool Contains(T item)
        {
            ThrowIfVersionNotMatch();
            return _buffer.Buffer.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            ThrowIfVersionNotMatch();
            _buffer.Buffer.CopyTo(array.AsSpan(arrayIndex));
        }

        public bool Remove(T item)
        {
            ThrowIfVersionNotMatch();
            return _buffer.Buffer.Remove(item);
        }

        void ICollection.CopyTo(Array array, int index)
        {
            ThrowIfVersionNotMatch();
            ((ICollection) _buffer.Buffer).CopyTo(array, index);
        }

        public int Count
        {
            get
            {
                ThrowIfVersionNotMatch();
                return ((ICollection) _buffer.Buffer).Count;
            }
        }

        bool ICollection.IsSynchronized
        {
            get
            {
                ThrowIfVersionNotMatch();
                return ((ICollection) _buffer.Buffer).IsSynchronized;
            }
        }

        object ICollection.SyncRoot
        {
            get
            {
                ThrowIfVersionNotMatch();
                return ((ICollection) _buffer.Buffer).SyncRoot;
            }
        }

        int ICollection<T>.Count
        {
            get
            {
                ThrowIfVersionNotMatch();
                return _buffer.Buffer.Count;
            }
        }

        bool ICollection<T>.IsReadOnly
        {
            get
            {
                ThrowIfVersionNotMatch();
                return ((ICollection<T>) _buffer.Buffer).IsReadOnly;
            }
        }

        public int IndexOf(T item)
        {
            ThrowIfVersionNotMatch();
            return _buffer.Buffer.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            ThrowIfVersionNotMatch();
            _buffer.Buffer.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            ThrowIfVersionNotMatch();
            ((IList<T>) _buffer.Buffer).RemoveAt(index);
        }

        public T this[int index]
        {
            get
            {
                ThrowIfVersionNotMatch();
                return _buffer.Buffer[index];
            }
            set
            {
                ThrowIfVersionNotMatch();
                _buffer.Buffer[index] = value;
            }
        }

        int IReadOnlyCollection<T>.Count
        {
            get
            {
                ThrowIfVersionNotMatch();
                return _buffer.Buffer.Count;
            }
        }

        #endregion
    }
}