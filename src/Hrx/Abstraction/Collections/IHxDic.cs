using System.Collections.Generic;

namespace Hrx
{
    public interface IHxDic<K, V>
    {
        IDictionary<K, V> Content { get; }
        IHRxProperty<int> Version { get; }
    }
}