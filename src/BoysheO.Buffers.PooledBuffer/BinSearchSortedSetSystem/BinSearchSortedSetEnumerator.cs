using System;
using System.Collections;
using System.Collections.Generic;
using BoysheO.Buffer.PooledBuffer.BinSearchSortedSetSystem;

namespace BoysheO.Buffer.PooledBuffer
{
    public struct BinSearchSortedSetEnumerator<T> : IEnumerator<T>
    {
        private readonly BinSearchSortedSet<T> _set;
        private int index;
        private readonly long _lifeVersion;
        private readonly int _enumeratorVersion;

        internal BinSearchSortedSetEnumerator(BinSearchSortedSet<T> set)
        {
            this._set = set;
            index = -1;
            _lifeVersion = set.LifeVersion;
            _enumeratorVersion = set.Version;
        }

        private void ThrowIfVersionNotMatch()
        {
            if (_set == null || _lifeVersion != _set.LifeVersion)
                throw new ObjectDisposedException("this buffer is disposed");
            if (_enumeratorVersion != _set.Version)
                throw new Exception("the collect has changed");
        }

        public bool MoveNext()
        {
            ThrowIfVersionNotMatch();
            if (index < _set.Count - 1)
            {
                index++;
                return true;
            }

            return false;
        }

        void IEnumerator.Reset()
        {
            ThrowIfVersionNotMatch();
            index = -1;
        }

        public T Current
        {
            get
            {
                ThrowIfVersionNotMatch();
                return _set[index];
            }
        }

        object IEnumerator.Current => Current;

        public void Dispose()
        {
            // do nothing
        }
    }
}