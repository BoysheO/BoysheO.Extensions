using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace BoysheO.Buffers
{
    /// <summary>
    /// The SimpleListPool design like ArrayPool&lt;T&gt; and all features are most same.Enjoy!<br />
    /// Rent and Return are thread safe<br />
    /// report any problems to github.com/BoysheO
    /// </summary>
    public sealed class SimpleListPool<T>
    {
        public readonly SimpleListPool<T> Share = new SimpleListPool<T>();

        private readonly WeakReference<ConcurrentBag<List<T>>> _reference =
            new WeakReference<ConcurrentBag<List<T>>>(new ConcurrentBag<List<T>>());

        private readonly object gate = new object();

        public List<T> Rent()
        {
            // ReSharper disable once InconsistentlySynchronizedField
            if (!_reference.TryGetTarget(out var _pool))
            {
                lock (gate)
                {
                    if (!_reference.TryGetTarget(out _pool))
                    {
                        _pool = new ConcurrentBag<List<T>>();
                        _reference.SetTarget(_pool);
                    }
                }
            }

            if (_pool.TryTake(out var lst)) return lst;
            lst = new List<T>();
            _pool.Add(lst);
            return lst;
        }

        public void Return(List<T> list, bool clearList = false)
        {
            // ReSharper disable once InconsistentlySynchronizedField
            if (!_reference.TryGetTarget(out var _pool))
            {
                lock (gate)
                {
                    if (!_reference.TryGetTarget(out _pool))
                    {
                        _pool = new ConcurrentBag<List<T>>();
                        _reference.SetTarget(_pool);
                    }
                }
            }

            if (clearList) list.Clear();
            _pool.Add(list);
        }
    }
}