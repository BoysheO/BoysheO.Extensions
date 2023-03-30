using Collections.Pooled;

namespace BoysheO.Buffers
{
    internal sealed class DictionaryProxy<TK,TV>
    {
        internal int Version = 1;
        internal readonly PooledDictionary<TK,TV> Map = new PooledDictionary<TK,TV>();
    }
}