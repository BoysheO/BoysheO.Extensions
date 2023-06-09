using System.Collections.Generic;

namespace BoysheO.Extensions
{
    public static class KeyValuePairExtensions
    {
        public static void Deconstruct<TK, TV>(this KeyValuePair<TK, TV> kvp, out TK key, out TV value)
        {
            key = kvp.Key;
            value = kvp.Value;
        }
    }
}