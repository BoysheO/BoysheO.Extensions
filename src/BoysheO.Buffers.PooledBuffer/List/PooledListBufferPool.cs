using System;
using System.Collections.Concurrent;
using System.Threading;
//using BoysheO.Extensions;

namespace BoysheO.Buffers
{
    internal class PooledListBufferPool<T>
    {
        public static readonly PooledListBufferPool<T> Share = new PooledListBufferPool<T>();

        private readonly WeakReference<ConcurrentBag<ListProxy<T>>> _reference =
            new WeakReference<ConcurrentBag<ListProxy<T>>>(new ConcurrentBag<ListProxy<T>>());

        private readonly object gate = new object();

        public PooledListBuffer<T> Rent()
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
            return new PooledListBuffer<T>(lst);
        }

        public void Return(PooledListBuffer<T> list)
        {
            if (list.Version != list.BufferProxy.Version) return;
            Interlocked.Increment(ref list.BufferProxy.Version);
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

            list.BufferProxy.Buffer.Clear();
            list.BufferProxy.Buffer.Dispose();
            _pool.Add(list.BufferProxy);
        }
    }
}