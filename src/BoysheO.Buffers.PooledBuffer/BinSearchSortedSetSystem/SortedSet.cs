using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using BoysheO.Buffers.PooledBuffer.Linq;
using BoysheO.Extensions.Util;
using Collections.Pooled;

namespace BoysheO.Buffer.PooledBuffer.BinSearchSortedSetSystem;

/// <summary>
/// 基于二分查找的Set(简单实现，以后再优化）
/// </summary>
internal class SortedSet<T> : ISet<T>, IReadOnlyPooledList<T>, ISortedSet<T>
{
    private T[] _t;

    private int _count;

    //遍历体Version
    public int Version { get; private set; }

    //生命周期Version
    public long LifeVersion { get; private set; }
    public IComparer<T> Comparer { get; private set; }

    public BinSearchSortedSetEnumerator<T> GetEnumerator()
    {
        return new BinSearchSortedSetEnumerator<T>(this);
    }

    IEnumerator<T> IEnumerable<T>.GetEnumerator()
    {
        return GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void ExceptWith(IEnumerable<T> other)
    {
        if (other == null) throw new ArgumentNullException(nameof(other));
        if (_count == 0) return;
        if (ReferenceEquals(other, this))
        {
            Clear();
            return;
        }

        if (other is IReadOnlyList<T> l)
        {
            ExceptWith(l);
        }
        else
        {
            foreach (var x1 in other)
            {
                Remove(x1);
            }
        }
    }

    public void ExceptWith(IReadOnlyList<T> other)
    {
        if (other == null) throw new ArgumentNullException(nameof(other));

        if (_count == 0) return;
        if (ReferenceEquals(other, this))
        {
            Clear();
            return;
        }

        for (var index = 0; index < other.Count; index++)
        {
            var x1 = other[index];
            Remove(x1);
        }
    }

    public void IntersectWith(IEnumerable<T> other)
    {
        if (other is IReadOnlyList<T> l)
        {
            IntersectWith(l);
        }
        else
        {
            using var copySet = PooledBinSearchSortedSetPoolBuffer<T>.Rent(Comparer, _count);
            foreach (var x1 in other)
            {
                if (Contains(x1)) copySet.AsSet.Add(x1);
            }

            Clear();
            copySet.AsSet.CopyTo(_t, 0);
            _count = copySet.AsSet.Count;
        }
    }

    public void IntersectWith(IReadOnlyList<T> other)
    {
        using var copySet = PooledBinSearchSortedSetPoolBuffer<T>.Rent(Comparer, _count);
        for (var index = 0; index < other.Count; index++)
        {
            var x1 = other[index];
            if (Contains(x1)) copySet.AsSet.Add(x1);
        }

        Clear();
        copySet.AsSet.CopyTo(_t, 0);
        _count = copySet.AsSet.Count;
    }

    public bool IsProperSubsetOf(IEnumerable<T> other)
    {
        if (other == null) throw new ArgumentNullException(nameof(other));
        if (other is ICollection c)
        {
            if (Count == 0)
                return c.Count > 0;
        }

        using var copy = this.ToPooledSetBuffer(Comparer);
        bool isAnyElementNotInSet = false;
        foreach (var x1 in other)
        {
            if (!copy.AsSet.Remove(x1))
            {
                isAnyElementNotInSet = true;
            }

            if (copy.AsSet.Count == 0 && isAnyElementNotInSet) break;
        }

        return copy.AsSet.Count == 0 && isAnyElementNotInSet;
    }


    public bool IsProperSupersetOf(IEnumerable<T> other)
    {
        if (other == null) throw new ArgumentNullException(nameof(other));
        using var copy = other.ToPooledSetBuffer(Comparer);
        bool isAnyElementNotInSet = false;
        foreach (var x1 in this)
        {
            if (!copy.AsSet.Remove(x1))
            {
                isAnyElementNotInSet = true;
            }

            if (copy.AsSet.Count == 0 && isAnyElementNotInSet) break;
        }

        return copy.AsSet.Count == 0 && isAnyElementNotInSet;
    }

    public bool IsSubsetOf(IEnumerable<T> other)
    {
        if (other == null) throw new ArgumentNullException(nameof(other));
        if (other is ICollection c)
        {
            if (Count == 0)
                return c.Count > 0;
        }

        using var copy = this.ToPooledSetBuffer(Comparer);
        foreach (var x1 in other)
        {
            copy.AsSet.Remove(x1);
            if (copy.AsSet.Count == 0) break;
        }

        return copy.AsSet.Count == 0;
    }

    public bool IsSupersetOf(IEnumerable<T> other)
    {
        if (other == null) throw new ArgumentNullException(nameof(other));
        using var copy = other.ToPooledSetBuffer(Comparer);
        foreach (var x1 in this)
        {
            copy.AsSet.Remove(x1);
            if (copy.AsSet.Count == 0) break;
        }

        return copy.AsSet.Count == 0;
    }

    public bool Overlaps(IEnumerable<T> other)
    {
        if (other == null) throw new ArgumentNullException(nameof(other));
        foreach (var x1 in other)
        {
            if (Contains(x1)) return true;
        }

        return false;
    }

    public bool SetEquals(IEnumerable<T> other)
    {
        if (other == null) throw new ArgumentNullException(nameof(other));
        foreach (var x1 in other)
        {
            if (!Contains(x1)) return false;
        }

        return true;
    }

    public void SymmetricExceptWith(IEnumerable<T> other)
    {
        foreach (var x1 in other)
        {
            if (!this.Remove(x1))
            {
                this.Add(x1);
            }
        }
    }

    public void UnionWith(IEnumerable<T> other)
    {
        foreach (var x1 in other)
        {
            Add(x1);
        }
    }

    public bool Add(T item)
    {
        if (item == null) throw new ArgumentNullException(nameof(item));
        var idx = Array.BinarySearch(_t, 0, _count, item);
        if (idx >= 0) return false;
        idx = ~idx;
        RefArrayPoolUtil.Insert(ref _t, ref _count, item, idx);
        return true;
    }

    void ICollection<T>.Add(T item)
    {
        if (item == null) throw new ArgumentNullException(nameof(item));
        Add(item);
    }

    public void Clear()
    {
        Array.Clear(_t, 0, _count);
        _count = 0;
    }

    public bool Contains(T item)
    {
        var idx = Array.BinarySearch(_t, 0, _count, item);
        return idx >= 0;
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        Array.Copy(_t, 0, array, arrayIndex, _count);
    }

    public bool Remove(T item)
    {
        var idx = Array.BinarySearch(_t, 0, _count, item);
        if (idx < 0) return false;
        for (int i = idx, count = _count - 2; i < count; i++)
        {
            _t[idx] = _t[idx + 1];
        }

        _t[_count - 1] = default;
        _count--;
        return true;
    }

    public int Count => _count;
    public bool IsReadOnly => false;

    public void Reuse(IComparer<T> comparer, long liftVersion, int capacity = 1)
    {
        if (capacity <= 0) throw new ArgumentOutOfRangeException(nameof(capacity));
        if (_t != null) throw new Exception("这个集合正在使用");
        Comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
        _t = ArrayPool<T>.Shared.Rent(capacity);
        LifeVersion = liftVersion;
        Version = 0;
    }

    public ISet<T> AsSet => this;
    public IReadOnlyList<T> AsReadOnlyList => this;

    public void AddRange(ReadOnlySpan<T> span)
    {
        if (Count == 0)
        {
            RefArrayPoolUtil.AddRange(ref _t, ref _count, span);
            Array.Sort(_t, 0, _count, Comparer);
        }
        else
        {
            foreach (var x1 in span)
            {
                Add(x1);
            }
        }
    }

    public void AddRange(IReadOnlyList<T> lst)
    {
        if (Count == 0)
        {
            RefArrayPoolUtil.AddRange(ref _t, ref _count, lst);
            Array.Sort(_t, 0, _count, Comparer);
        }
        else
        {
            for (var index = 0; index < lst.Count; index++)
            {
                var x1 = lst[index];
                Add(x1);
            }
        }
    }

    public void AddRange(IEnumerable<T> lst)
    {
        foreach (var x1 in lst)
        {
            Add(x1);
        }
    }


    public void Recycle()
    {
        Clear();
        Comparer = null;
        LifeVersion = 0;
        Version = 0;
        ArrayPool<T>.Shared.Return(_t);
        _t = null;
    }

    public int IndexOf(T item)
    {
        return Array.BinarySearch(_t, 0, _count, item);
    }

    public T this[int index]
    {
        get
        {
            if (index < 0 || index >= _count) throw new ArgumentOutOfRangeException(nameof(index));
            return _t[index];
        }
    }

    public ReadOnlySpan<T> Span => _t.AsSpan(0, _count);

    // public T Min
    // {
    //     get
    //     {
    //         if (_count == 0) throw new Exception("count is 0");
    //         return _t[0];
    //     }
    // }
    //
    // public T Max
    // {
    //     get
    //     {
    //         if (_count == 0) throw new Exception("count is 0");
    //         return _t[_count - 1];
    //     }
    // }

    // public BinSearchSortedSet<T> GetViewBetween(T lowerValue, T upperValue)
    // {
    //     var idxMin = Array.BinarySearch(_t, 0, _count, lowerValue);
    //     if (idxMin < 0) idxMin = ~idxMin;
    //     var idxMax = Array.BinarySearch(_t, 0, _count, upperValue);
    //     
    // }
}