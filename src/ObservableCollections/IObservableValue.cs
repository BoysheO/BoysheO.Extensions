namespace ObservableCollections;

public interface IObservableValue<out T> : IEvent<T>
{
    T Value { get; }
}