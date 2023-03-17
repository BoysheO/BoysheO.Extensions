using Collections.Pooled;

namespace BoysheO.Buffers
{
    public sealed class ListProxy<T>
    {
        internal int Version = 1;
        internal readonly PooledList<T> List = new PooledList<T>();
    }
}