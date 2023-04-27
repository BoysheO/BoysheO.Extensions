using System;
using System.Collections;
using System.Collections.Generic;
using BoysheO.Buffers;
using Collections.Pooled;

namespace BoysheO.Buffer.PooledBuffer
{
    public struct ListEnumerator<T> : IEnumerator<T>
    {
        private readonly ListProxy<T> list;
        private readonly long version;
        private PooledList<T>.Enumerator _enumerator;

        internal ListEnumerator(ListProxy<T> list, long version)
        {
            this.list = list;
            this.version = version;
            _enumerator = list.List.GetEnumerator();
        }

        private void ThrowIfVersionNotMatch()
        {
            if (list == null || version != list.Version)
                throw new ObjectDisposedException("this buffer is disposed");
        }

        public bool MoveNext()
        {
            ThrowIfVersionNotMatch();
            return _enumerator.MoveNext();
        }

        void IEnumerator.Reset()
        {
            ThrowIfVersionNotMatch();
            ((IEnumerator)_enumerator).Reset();
        }

        public T Current
        {
            get
            {
                ThrowIfVersionNotMatch();
                return _enumerator.Current;
            }
        }

        object IEnumerator.Current => Current;

        public void Dispose()
        {
            _enumerator.Dispose();
        }
    }
}