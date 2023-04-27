using System;
using System.Collections;
using System.Collections.Generic;
using BoysheO.Buffers;
using Collections.Pooled;

namespace BoysheO.Buffer.PooledBuffer
{
    public struct ListEnumerator<TK, TV> : IEnumerator<KeyValuePair<TK, TV>>
    {
        private readonly DictionaryProxy<TK, TV> list;
        private readonly long version;
        private PooledDictionary<TK, TV>.Enumerator _enumerator;

        internal ListEnumerator(DictionaryProxy<TK, TV> list, long version)
        {
            this.list = list;
            this.version = version;
            _enumerator = list.Map.GetEnumerator();
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

        public KeyValuePair<TK, TV> Current
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