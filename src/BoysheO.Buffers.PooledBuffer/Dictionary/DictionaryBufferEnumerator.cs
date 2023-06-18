using System;
using System.Collections;
using System.Collections.Generic;
using BoysheO.Buffers;
using Collections.Pooled;

namespace BoysheO.Buffer.PooledBuffer
{
    public struct DictionaryBufferEnumerator<TK, TV> : IEnumerator<KeyValuePair<TK, TV>>, IDictionaryEnumerator
    {
        private readonly DictionaryProxy<TK, TV> list;
        private readonly long version;
        private PooledDictionary<TK, TV>.Enumerator _enumerator;

        internal DictionaryBufferEnumerator(DictionaryProxy<TK, TV> list, long version)
        {
            this.list = list;
            this.version = version;
            _enumerator = list.Buffer.GetEnumerator();
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
            ((IEnumerator) _enumerator).Reset();
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

        DictionaryEntry IDictionaryEnumerator.Entry =>
            new DictionaryEntry(_enumerator.Current.Key, _enumerator.Current.Value);

        object IDictionaryEnumerator.Key => _enumerator.Current.Key;
        object IDictionaryEnumerator.Value => _enumerator.Current.Value;
    }
}