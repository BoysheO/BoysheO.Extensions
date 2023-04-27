using System;
using System.Collections.Generic;

namespace BoysheO.Buffers.PooledBuffer.Linq
{
    public static class EnumerableExtensions
    {
        public static PooledListBuffer<T> ToPooledListBuffer<T>(this IEnumerable<T> source)
        {
            var buff = PooledListBuffer<T>.Rent();
            buff.AddRange(source);
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
            Func<TS, TV> valueSelector)
        {
            var buff = PooledSortedListBuffer<TK, TV>.Rent();
            foreach (var x1 in source)
            {
                buff.Add(keySelector(x1), valueSelector(x1));
            }

            return buff;
        }
    }
}