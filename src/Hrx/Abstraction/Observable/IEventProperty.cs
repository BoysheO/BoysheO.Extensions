namespace Hrx
{
    public interface IEventProperty<out T>
    {
        T Value { get; }
        IEvent<T> onChanged { get; }
    }
}