using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BoysheO.Extensions;

namespace BoysheO.Toolkit
{
    /// <summary>
    ///     一个只读集合，包装了一个IEnumerable{T}元素
    ///     <para>所有方法都是通过Linq实现的</para>
    /// </summary>
    public class CollectionAdapter<T> : ICollection<T>
    {
        private IEnumerable<T> _vs;

        public CollectionAdapter(IEnumerable<T> vs)
        {
            _vs = vs ?? throw new ArgumentNullException(nameof(vs));
        }

        public int Count => _vs.Count();

        public bool IsReadOnly => true;

        public void Add(T item)
        {
            _vs = _vs.Append(item);
        }

        public void Clear()
        {
            _vs = Enumerable.Empty<T>();
        }

        public bool Contains(T item)
        {
            return _vs.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _vs.CopyTo(array, arrayIndex);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _vs.GetEnumerator();
        }

        public bool Remove(T item)
        {
            if (_vs.Contains(item))
            {
                _vs = _vs.Except(new[] {item});
                return true;
            }

            return false;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _vs.GetEnumerator();
        }
    }
}