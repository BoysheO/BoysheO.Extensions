using System;
using System.Collections.Concurrent;
using System.Threading;
//using BoysheO.Extensions;

namespace BoysheO.Buffers
{
    internal class PooledBufferPool<T>
    {
        public static readonly PooledBufferPool<T> Share = new PooledBufferPool<T>();

        private readonly WeakReference<ConcurrentBag<ListProxy<T>>> _reference =
            new WeakReference<ConcurrentBag<ListProxy<T>>>(new ConcurrentBag<ListProxy<T>>());

        private readonly object gate = new object();

        public PooledBuffer<T> Rent()
        {
            // ReSharper disable once InconsistentlySynchronizedField
            if (!_reference.TryGetTarget(out var _pool))
            {
                lock (gate)
                {
                    if (!_reference.TryGetTarget(out _pool))
                    {
                        _pool = new ConcurrentBag<ListProxy<T>>();
                        _reference.SetTarget(_pool);
                    }
                }
            }

            if (!_pool.TryTake(out var lst))
            {
                lst = new ListProxy<T>();
            }

            //if (!lst.List.IsEmpty()) throw new Exception("dirty buffer");
            return new PooledBuffer<T>(lst);
        }

        public void Return(PooledBuffer<T> list)
        {
            if (list.Version != list.ListProxy.Version) return;
            Interlocked.Increment(ref list.ListProxy.Version);
            // ReSharper disable once InconsistentlySynchronizedField
            if (!_reference.TryGetTarget(out var _pool))
            {
                lock (gate)
                {
                    if (!_reference.TryGetTarget(out _pool))
                    {
                        _pool = new ConcurrentBag<ListProxy<T>>();
                        _reference.SetTarget(_pool);
                    }
                }
            }

            list.ListProxy.List.Clear();
            list.ListProxy.List.Dispose();
            _pool.Add(list.ListProxy);
        }
    }
}