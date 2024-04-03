using System;
using System.Collections.Generic;
using Collections.Pooled;

namespace BoysheO.Buffer.PooledBuffer.BinSearchSortedSetSystem;

public interface ISortedSet<T>
{
    ISet<T> AsSet { get; }
    IReadOnlyList<T> AsReadOnlyList { get; }
    void AddRange(ReadOnlySpan<T> span);
    void AddRange(IReadOnlyList<T> lst);
    void AddRange(IEnumerable<T> lst);
}