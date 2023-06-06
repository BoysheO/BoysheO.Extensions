namespace ObservableCollections;

public interface IObservableCollection<T>
{
    IEvent<InsertEv<T>> onInsert { get; }
    IEvent<RemoveEv<T>> onRemove { get; }

    /// <summary>
    /// always invoke after onInsert or onRemove
    /// </summary>
    IEvent<Unit> onChanged { get; }
}