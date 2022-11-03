using System;
using System.Collections.Generic;

namespace BoysheO.Toolkit
{
    [Obsolete("use Comparer.Creat instead")]
    public class ComparerAdapter<T> : Comparer<T>
    {
        private readonly Func<T, T, int> _comparator;

        public ComparerAdapter(Func<T, T, int> comparator)
        {
            _comparator = comparator ?? throw new ArgumentNullException(nameof(comparator));
        }

        public override int Compare(T x, T y)
        {
            return _comparator(x, y);
        }
    }
}