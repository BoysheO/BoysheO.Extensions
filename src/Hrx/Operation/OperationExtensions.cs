using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Hrx.Operation
{
    public static class OperationExtensions
    {
        public static (TTarget, Exception?) Select<TSource, TTarget>(
            (TSource, Exception?) ev,
            Func<TSource, TTarget> selector)
        {
            var target = selector(ev.Item1);
            return (target, ev.Item2);
        }

        public static (TTarget, Exception?)? Select<TSource, TTarget>(
            (TSource, Exception?)? ev,
            Func<TSource, TTarget> selector)
        {
            if (ev == null) return null;
            var target = selector(ev.Value.Item1);
            return (target, ev.Value.Item2);
        }

        public static (TSource, Exception?)? Where<TSource>(
            (TSource, Exception?)? ev,
            Func<TSource, bool> filter)
        {
            if (ev == null) return null;
            return filter(ev.Value.Item1) ? ev : null;
        }

        public static IEvent<T> Merge<T>(this IEvent<T> source, IReadOnlyCollection<IEvent<T>> events)
        {
            var builder = ImmutableArray.CreateBuilder<IEvent<T>>(events.Count + 1);
            builder.Add(source);
            builder.AddRange(events);
            return new MergeSubject<T>(builder.MoveToImmutable());
        }

        public static IEvent<T> Merge<T>(this ImmutableArray<IEvent<T>> events)
        {
            return new MergeSubject<T>(events);
        }

        public static IEvent<T> Sample<T>(this IEvent<T> source, IEvent<Unit> sample)
        {
            return new SampleSubject<T>(source, sample);
        }
    }
}