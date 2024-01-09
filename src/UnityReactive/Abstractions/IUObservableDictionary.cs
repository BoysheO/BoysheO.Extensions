using System.Collections.Generic;
using UnityReactive.Core;

namespace UnityReactive.Abstractions
{
    public interface IUObservableDictionary<K, V> : IUReadOnlyObservableDictionary<K, V>, IDictionary<K, V>
    {
    }

    public interface IUReadOnlyObservableDictionary<K, V> : IReadOnlyDictionary<K, V>
    {
        IUReadOnlyReactiveProperty<int> Version { get; }
        Observable onInsert { get; }
        Observable onRemove { get; }
        Observable onReplace { get; }
        Observable onClear { get; }
    }

    public class InsertEvent<K, V>
    {
        public K Key { get; set; }
        public V Value { get; set; }
    }

    public class RemoveEvent<K, V>
    {
        public K Key { get; set; }
        public V Value { get; set; }
    }

    public class ReplaceEvent<K, V>
    {
        public K Key { get; set; }
        public V OldValue { get; set; }
        public V NewValue { get; set; }
    }
}