using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace BoysheO.Buffer.PooledBuffer.BinSearchSortedSetSystem;

internal class BinSearchSortedSetPool<T>
{
    public static readonly BinSearchSortedSetPool<T> Share = new();

    private readonly WeakReference<ConcurrentBag<SortedSet<T>>> _reference = new(new());
    private long _lifeVersion;

    public SortedSet<T> Rent(IComparer<T> comparer, int capacity = 1)
    {
        SortedSet<T> lst = null;
        if (_reference.TryGetTarget(out var bag))
        {
            if (bag.TryTake(out lst))
            {
                //do nothing
            }
        }

        if (lst == null) lst = new SortedSet<T>();
        var lifeVersion = Interlocked.Increment(ref _lifeVersion);
        lst.Reuse(comparer, lifeVersion, capacity);
        return lst;
    }

    public void Return(SortedSet<T> lst)
    {
        lst.Recycle();
        if (!_reference.TryGetTarget(out var bag))
        {
            bag = new();
        }

        bag.Add(lst);
    }
}