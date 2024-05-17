using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using BoysheO.Extensions.Util;

namespace BoysheO.Buffers
{
    public class SimplePooledList<T> : IList<T>, IReadOnlyList<T>
    {
        private T[] _items = ArrayPool<T>.Shared.Rent(0);
        private int _count;

        public Span<T> AsSpan => _items.AsSpan(0, _count);

        public void AddRange(ReadOnlySpan<T> span)
        {
            RefArrayPoolUtil.AddRange(ref _items, ref _count, span);
        }

        public IEnumerator<T> GetEnumerator()
        {
            foreach (var item in _items)
            {
                yield return item;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(T item)
        {
            RefArrayPoolUtil.Add(ref _items, ref _count, item);
        }

        public void Clear()
        {
            Array.Clear(_items, 0, _count);
            _count = 0;
        }

        public bool Contains(T item)
        {
            return IndexOf(item) >= 0;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            Array.Copy(_items, 0, array, arrayIndex, _count);
        }

        public bool Remove(T item)
        {
            return RefArrayPoolUtil.Remove(_items, ref _count, item);
        }

        public int Count => _count;

        public bool IsReadOnly => false;

        public int IndexOf(T item)
        {
            return Array.IndexOf(_items, item);
        }

        public void Insert(int index, T item)
        {
            RefArrayPoolUtil.Insert(ref _items, ref _count, item, index);
        }

        public void RemoveAt(int index)
        {
            RefArrayPoolUtil.RemoveAt(_items, ref _count, index);
        }

        public T this[int index]
        {
            get
            {
                if (index >= _count) throw new ArgumentOutOfRangeException(nameof(index));
                return _items[index];
            }
            set
            {
                if (index > _count) throw new ArgumentOutOfRangeException(nameof(index));
                if (index == _count)
                {
                    Insert(index, value);
                }
                else
                {
                    _items[index] = value;
                }
            }
        }

        public void Release()
        {
            Clear();
            ArrayPool<T>.Shared.Return(_items);
            _items = ArrayPool<T>.Shared.Rent(0);
        }

        public int BinarySearch(int index, int count, T item, IComparer<T> comparer)
        {
            count = Math.Min(count, _count);
            return Array.BinarySearch<T>(_items, index, count, item, comparer);
        }

        public void Sort(int index, int count, IComparer<T> comparer)
        {
            count = Math.Min(count, _count);
            Array.Sort(_items, index, count, comparer);
        }
    }
}