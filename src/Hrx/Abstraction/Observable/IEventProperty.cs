namespace Hrx
{
    public interface IEventProperty<out T>:IEvent<T>
    {
        T Value { get; }
    }
}