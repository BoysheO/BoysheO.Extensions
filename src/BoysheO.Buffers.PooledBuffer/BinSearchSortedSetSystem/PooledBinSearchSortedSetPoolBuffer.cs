using System;
using System.Collections.Generic;
using BoysheO.Buffer.PooledBuffer.BinSearchSortedSetSystem;
using Collections.Pooled;

namespace BoysheO.Buffer.PooledBuffer
{
    public readonly struct PooledBinSearchSortedSetPoolBuffer<T> : IDisposable
    {
        private readonly long _lifeVersion;
        private readonly BinSearchSortedSet<T> _lst;

        private PooledBinSearchSortedSetPoolBuffer(BinSearchSortedSet<T> lst)
        {
            _lifeVersion = lst.LifeVersion;
            _lst = lst;
        }

        public static PooledBinSearchSortedSetPoolBuffer<T> Rent(IComparer<T> comparer, int capacity = 1)
        {
            var ins = BinSearchSortedSetPool<T>.Share.Rent(comparer, capacity);
            return new PooledBinSearchSortedSetPoolBuffer<T>(ins);
        }
        
        public static PooledBinSearchSortedSetPoolBuffer<T> Rent(int capacity = 1)
        {
            var ins = BinSearchSortedSetPool<T>.Share.Rent(Comparer<T>.Default, capacity);
            return new PooledBinSearchSortedSetPoolBuffer<T>(ins);
        }

        public ISet<T> AsSet
        {
            get
            {
                if (_lst == null || _lifeVersion != _lst.LifeVersion)
                    throw new ObjectDisposedException("", "this obj has disposed");
                return _lst;
            }
        }

        public IReadOnlyPooledList<T> AsReadOnlyList
        {
            get
            {
                if (_lst == null || _lifeVersion != _lst.LifeVersion)
                    throw new ObjectDisposedException("", "this obj has disposed");
                return _lst;
            }
        }

        public void Dispose()
        {
            BinSearchSortedSetPool<T>.Share.Return(_lst);
        }
    }
}