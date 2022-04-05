using System.Collections.Generic;

namespace BoysheO.Extensions
{
    public static class KeyValuePairExtensions
    {
        public static (TK key, TV value) Deconstruct<TK, TV>(this KeyValuePair<TK, TV> kvp)
        {
            return (kvp.Key, kvp.Value);
        }
    }
}