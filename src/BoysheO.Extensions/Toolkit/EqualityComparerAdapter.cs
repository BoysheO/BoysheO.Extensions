using System;
using System.Collections.Generic;

namespace BoysheO.Toolkit
{
    public class EqualityComparerAdapter<T> : EqualityComparer<T>
    {
        private readonly Func<T, T, bool> _equalsComparer;
        private readonly Func<T, int> _hashCodeGetter;

        public EqualityComparerAdapter(Func<T, int> hashCodeGetter, Func<T, T, bool> equalsComparer)
        {
            _hashCodeGetter = hashCodeGetter ?? throw new ArgumentNullException(nameof(hashCodeGetter));
            _equalsComparer = equalsComparer ?? throw new ArgumentNullException(nameof(equalsComparer));
        }

        public override bool Equals(T x, T y)
        {
            return _equalsComparer(x, y);
        }

        public override int GetHashCode(T obj)
        {
            return _hashCodeGetter(obj);
        }
    }
}