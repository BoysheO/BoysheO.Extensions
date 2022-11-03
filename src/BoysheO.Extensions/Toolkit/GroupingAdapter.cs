using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace BoysheO.Toolkit
{
    public class GroupingAdapter<TKey, TElement> : IGrouping<TKey, TElement>
    {
        private readonly IEnumerable<TElement> _elements;
        private readonly Func<TKey> _key;

        public GroupingAdapter(Func<TKey> key, IEnumerable<TElement> element)
        {
            _elements = element ?? throw new ArgumentNullException(nameof(element));
            _key = key ?? throw new ArgumentNullException(nameof(key));
        }

        public GroupingAdapter(TKey key, IEnumerable<TElement> element)
        {
            _elements = element ?? throw new ArgumentNullException(nameof(element));
            _key = () => key;
        }

        public TKey Key => _key();

        public IEnumerator<TElement> GetEnumerator()
        {
            return _elements.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}