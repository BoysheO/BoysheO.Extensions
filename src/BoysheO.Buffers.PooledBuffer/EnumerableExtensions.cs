using System;
using System.Collections.Generic;
using System.Linq;
using Collections.Pooled;

namespace BoysheO.Buffers.PooledBuffer.Linq
{
    public static class EnumerableExtensions
    {
        public static PooledListBuffer<T> ToPooledListBuffer<T>(this IEnumerable<T> source,
            Func<T, bool> predicate = null)
        {
            var buff = PooledListBuffer<T>.Rent();
            if (predicate == null)
            {
                buff.AddRange(source);
            }
            else
            {
                foreach (var x1 in source)
                {
                    if (predicate(x1)) buff.Add(x1);
                }
            }

            return buff;
        }

        public static PooledListBuffer<T> ToPooledListBuffer<T>(this PooledListBuffer<T> source,
            Func<T, bool> predicate = null)
        {
            var buff = PooledListBuffer<T>.Rent();
            if (predicate == null)
            {
                buff.AddRange(source.Span);
            }
            else
            {
                foreach (var x1 in source)
                {
                    if (predicate(x1)) buff.Add(x1);
                }
            }

            return buff;
        }

        public static PooledDictionaryBuffer<TK, TV> ToPooledDictionaryBuffer<TS, TK, TV>(
            this IEnumerable<TS> source,
            Func<TS, TK> keySelector,
            Func<TS, TV> valueSelector)
        {
            var buff = PooledDictionaryBuffer<TK, TV>.Rent();
            foreach (var x1 in source)
            {
                buff.Add(keySelector(x1), valueSelector(x1));
            }

            return buff;
        }

        public static PooledSortedListBuffer<TK, TV> ToPooledSortedListBuffer<TS, TK, TV>(
            this IEnumerable<TS> source,
            Func<TS, TK> keySelector,
            Func<TS, TV> valueSelector,
            IComparer<TK> comparer)
        {
            var buff = PooledSortedListBuffer<TK, TV>.Rent(comparer);
            foreach (var x1 in source)
            {
                buff.Add(keySelector(x1), valueSelector(x1));
            }

            return buff;
        }
    }
}