using System;
using System.Collections.Generic;

namespace BoysheO.Buffers.PooledBuffer.Linq
{
    /// <summary>
    /// Every operation will dispose the argument witch is PooledBuff.
    /// It's faster and lowGc 
    /// </summary>
    public static class OperationExtensions
    {
        public static PooledBuffer<TTar> PooledSelect<TSrc, TTar>(this PooledBuffer<TSrc> source, Func<TSrc, TTar> selector)
        {
            var buff = PooledBuffer<TTar>.Rent();
            for (int index = 0, count = buff.Count; index < count; index++)
            {
                var src = source[index];
                var tar = selector(src);
                buff.Add(tar);
            }

            source.Dispose();
            return buff;
        }

        // public static PooledBuffer<TDestination> SelectMany<TElement, TDestination>(
        //     this PooledBuffer<PooledBuffer<TElement>> source, Func<TElement, TDestination> selector)
        // {
        //     var buff = PooledBuffer<TDestination>.Rent();
        //     for (int index = 0, count = buff.Count; index < count; index++)
        //     {
        //         var enumerableSrc = source[index];
        //         var itor = enumerableSrc.GetEnumerator();
        //         while (itor.MoveNext())
        //         {
        //             buff.Add(selector(itor.Current));
        //         }
        //
        //         itor.Dispose();
        //         enumerableSrc.Dispose();
        //     }
        //
        //     source.Dispose();
        //     return buff;
        // }

        public static PooledBuffer<T> PooledWhere<T>(this PooledBuffer<T> source, Func<T, bool> filter)
        {
            var buff = PooledBuffer<T>.Rent();
            var span = source.Span;
            for (int index = 0, count = buff.Count; index < count; index++)
            {
                var src = span[index];
                if(filter(src)) buff.Add(src);
            }

            source.Dispose();
            return buff;
        }

        public static PooledBuffer<T> PooledSlice<T>(this PooledBuffer<T> source, int start, int count)
        {
            var buf = PooledBuffer<T>.Rent();
            var destination = buf.GetSpanAdding(count);
            source.Span.Slice(start, count).CopyTo(destination);
            source.Dispose();
            return buf;
        }
        
        public static PooledBuffer<T> ToPooledBuffer<T>(this IEnumerable<T> source)
        {
            var buff = PooledBuffer<T>.Rent();
            foreach (var x1 in source)
            {
                buff.Add(x1);
            }

            return buff;
        }
    }
}