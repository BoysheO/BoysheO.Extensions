using System;
using System.Collections.Concurrent;
using Collections.Pooled;

namespace BoysheO.Buffers
{
    public class SimplePooledListPool<T>
    {
        public readonly SimplePooledListPool<T> Share = new();

        private readonly WeakReference<ConcurrentBag<PooledList<T>>> _reference = new(new ConcurrentBag<PooledList<T>>());
        private readonly object gate = new object();

        public PooledList<T> Rent()
        {
            // ReSharper disable once InconsistentlySynchronizedField
            if (!_reference.TryGetTarget(out var _pool))
            {
                lock (gate)
                {
                    if (!_reference.TryGetTarget(out _pool))
                    {
                        _pool = new ConcurrentBag<PooledList<T>>();
                        _reference.SetTarget(_pool);
                    }
                }
            }

            if (_pool.TryTake(out var lst)) return lst;
            lst = new PooledList<T>();
            return lst;
        }

        public void Return(PooledList<T> list)
        {
            // ReSharper disable once InconsistentlySynchronizedField
            if (!_reference.TryGetTarget(out var _pool))
            {
                lock (gate)
                {
                    if (!_reference.TryGetTarget(out _pool))
                    {
                        _pool = new ConcurrentBag<PooledList<T>>();
                        _reference.SetTarget(_pool);
                    }
                }
            }

            list.Clear();
            list.Dispose();
            _pool.Add(list);
        }
        
    }
}