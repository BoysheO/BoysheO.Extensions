using System;
using System.Collections.Concurrent;
using System.Threading;

namespace BoysheO.Buffers
{
    internal class PooledDictionaryBufferPool<TK, TV>
    {
        public static readonly PooledDictionaryBufferPool<TK, TV> Share = new PooledDictionaryBufferPool<TK, TV>();

        private readonly WeakReference<ConcurrentBag<DictionaryProxy<TK, TV>>> _reference =
            new WeakReference<ConcurrentBag<DictionaryProxy<TK, TV>>>(new ConcurrentBag<DictionaryProxy<TK, TV>>());

        private readonly object gate = new object();

        public PooledDictionaryBuffer<TK, TV> Rent()
        {
            // ReSharper disable once InconsistentlySynchronizedField
            if (!_reference.TryGetTarget(out var _pool))
            {
                lock (gate)
                {
                    if (!_reference.TryGetTarget(out _pool))
                    {
                        _pool = new ConcurrentBag<DictionaryProxy<TK, TV>>();
                        _reference.SetTarget(_pool);
                    }
                }
            }

            if (!_pool.TryTake(out var lst))
            {
                lst = new DictionaryProxy<TK, TV>();
            }

            //if (!lst.List.IsEmpty()) throw new Exception("dirty buffer");
            return new PooledDictionaryBuffer<TK, TV>(lst);
        }

        public void Return(PooledDictionaryBuffer<TK, TV> list)
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
                        _pool = new ConcurrentBag<DictionaryProxy<TK, TV>>();
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