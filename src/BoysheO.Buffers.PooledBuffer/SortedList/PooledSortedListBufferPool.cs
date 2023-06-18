using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using BoysheO.Buffer.PooledBuffer;

namespace BoysheO.Buffers
{
    internal class PooledSortedListBufferPool<TK, TV>
    {
        public static readonly PooledSortedListBufferPool<TK, TV> Share = new PooledSortedListBufferPool<TK, TV>();

        private readonly WeakReference<ConcurrentBag<UnsafePooledSortedList<TK, TV>>> _reference =
            new WeakReference<ConcurrentBag<UnsafePooledSortedList<TK, TV>>>(
                new ConcurrentBag<UnsafePooledSortedList<TK, TV>>());

        private readonly object gate = new object();

        public PooledSortedListBuffer<TK, TV> Rent(IComparer<TK> comparer)
        {
            if (comparer == null) throw new ArgumentNullException(nameof(comparer));
            // ReSharper disable once InconsistentlySynchronizedField
            if (!_reference.TryGetTarget(out var _pool))
            {
                lock (gate)
                {
                    if (!_reference.TryGetTarget(out _pool))
                    {
                        _pool = new ConcurrentBag<UnsafePooledSortedList<TK, TV>>();
                        _reference.SetTarget(_pool);
                    }
                }
            }

            if (!_pool.TryTake(out var buffer))
            {
                buffer = new UnsafePooledSortedList<TK, TV>();
            }

            buffer.Comparer = comparer;
            return new PooledSortedListBuffer<TK, TV>(buffer);
        }

        public void Return(PooledSortedListBuffer<TK, TV> list)
        {
            if (list.Version != list.Buffer.LifeVersion) return;
            var buffer = list.Buffer;
            Interlocked.Increment(ref buffer.LifeVersion);
            // ReSharper disable once InconsistentlySynchronizedField
            if (!_reference.TryGetTarget(out var _pool))
            {
                lock (gate)
                {
                    if (!_reference.TryGetTarget(out _pool))
                    {
                        _pool = new ConcurrentBag<UnsafePooledSortedList<TK, TV>>();
                        _reference.SetTarget(_pool);
                    }
                }
            }

            buffer.Clear();
            buffer.Dispose();
            buffer.Comparer = null;
            _pool.Add(buffer);
        }
    }
}