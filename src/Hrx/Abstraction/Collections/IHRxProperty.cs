namespace Hrx
{
    public interface IHRxProperty<out T>:INObservable<T>
    {
        T Value { get; }
    }
}