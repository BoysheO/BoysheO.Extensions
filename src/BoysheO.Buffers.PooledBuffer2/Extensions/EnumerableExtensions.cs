using System;
using System.Collections.Generic;
using System.Linq;
using BoysheO.Buffer.PooledBuffer;
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

        public static PooledBinSearchSortedSetPoolBuffer<T> ToPooledSetBuffer<T>(this IEnumerable<T> itor,
            IComparer<T> comparer)
        {
            if (itor == null) throw new ArgumentNullException(nameof(itor));
            if (comparer == null) throw new ArgumentNullException(nameof(comparer));
            var buffer = PooledBinSearchSortedSetPoolBuffer<T>.Rent(comparer);
            buffer.AsSortedSet.AddRange(itor);
            return buffer;
        }
        
        
        public static PooledBinSearchSortedSetPoolBuffer<T> ToPooledSetBuffer<T>(this PooledListBuffer<T> lst,
            IComparer<T> comparer)
        {
            if (comparer == null) throw new ArgumentNullException(nameof(comparer));
            var buffer = PooledBinSearchSortedSetPoolBuffer<T>.Rent(comparer);
            buffer.AsSortedSet.AddRange(lst.Span);
            return buffer;
        }
        
        public static PooledBinSearchSortedSetPoolBuffer<T> ToPooledSetBuffer<T>(this PooledListBuffer<T> lst)
        {
            return ToPooledSetBuffer(lst, Comparer<T>.Default);
        }
        
        public static PooledBinSearchSortedSetPoolBuffer<T> ToPooledSetBuffer<T>(this ReadOnlySpan<T> span,
            IComparer<T> comparer)
        {
            if (comparer == null) throw new ArgumentNullException(nameof(comparer));
            var buffer = PooledBinSearchSortedSetPoolBuffer<T>.Rent(comparer);
            buffer.AsSortedSet.AddRange(span);
            return buffer;
        }
        
        public static PooledBinSearchSortedSetPoolBuffer<T> ToPooledSetBuffer<T>(this ReadOnlySpan<T> span)
        {
            return ToPooledSetBuffer(span, Comparer<T>.Default);
        }
    }
}