using System;
using System.Collections;
using System.Collections.Generic;
using Collections.Pooled;

namespace BoysheO.Buffer.PooledBuffer
{
    public struct PooledSortedListBufferEnumerator<TK, TV> : IEnumerator<KeyValuePair<TK, TV>>
    {
        private readonly UnsafePooledSortedList<TK, TV> list;
        private int index;
        private readonly long LifeVersion;

        internal PooledSortedListBufferEnumerator(UnsafePooledSortedList<TK, TV> list)
        {
            this.list = list;
            index = -1;
            LifeVersion = list.LifeVersion;
        }

        private void ThrowIfVersionNotMatch()
        {
            if (list == null || LifeVersion != list.LifeVersion)
                throw new ObjectDisposedException("this buffer is disposed");
        }

        public bool MoveNext()
        {
            ThrowIfVersionNotMatch();
            if (index < list.Count - 1)
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

        public KeyValuePair<TK, TV> Current
        {
            get
            {
                ThrowIfVersionNotMatch();
                return new KeyValuePair<TK, TV>(list.Keys[index], list.Values[index]);
            }
        }

        object IEnumerator.Current => Current;

        public void Dispose()
        {
            // do nothing
        }
    }
}