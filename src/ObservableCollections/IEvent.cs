namespace ObservableCollections;

public interface IEvent<out T>
{
    event Action<T> onEvent;
}