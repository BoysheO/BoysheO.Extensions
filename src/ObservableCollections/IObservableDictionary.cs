namespace ObservableCollections;

public interface IObservableDictionary<TKey, TValue> : IObservableCollection<KeyValuePair<TKey, TValue>>
{
}