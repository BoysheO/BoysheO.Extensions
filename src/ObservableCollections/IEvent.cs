namespace ObservableCollections;

public interface IEvent<out T>
{
    event Action<T> onEvent;
    event Action<Unit> onEventAsUnit;
}