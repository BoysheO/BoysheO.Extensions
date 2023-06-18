using Collections.Pooled;

namespace BoysheO.Buffers
{
    internal sealed class DictionaryProxy<TK,TV>
    {
        internal long Version = 1;
        internal readonly PooledDictionary<TK,TV> Buffer = new PooledDictionary<TK,TV>();
    }
}