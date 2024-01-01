using System.Collections.Generic;

namespace UReactive.Abstractions
{
    public interface IUObservableDictionary<K, V> : IUReadOnlyObservableDictionary<K, V>, IDictionary<K, V>
    {
        
    }
    
    public interface IUReadOnlyObservableDictionary<K,V>:IReadOnlyDictionary<K,V>
    {
        IUReadOnlyReactiveProperty<int> Version { get; }
        IUObservable<InsertEvent<K,V>> onInsert { get; }
        IUObservable<RemoveEvent<K,V>> onRemove { get; }
        IUObservable<ReplaceEvent<K,V>> onReplace { get; }
        IUObservable<Unit> onClear { get; }
    }

    public class InsertEvent<K,V>
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