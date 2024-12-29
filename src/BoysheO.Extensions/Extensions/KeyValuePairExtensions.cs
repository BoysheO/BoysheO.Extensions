using System.Collections.Generic;

namespace BoysheO.Extensions
{
    public static class KeyValuePairExtensions
    {
#if NETFRAMEWORK || NETSTANDARD2_0
        // ReSharper disable once InconsistentNaming
        public static void Deconstruct<TK, TV>(this KeyValuePair<TK, TV> kvp, out TK key, out TV value)
        {
            key = kvp.Key;
            value = kvp.Value;
        }
#endif
    }
}