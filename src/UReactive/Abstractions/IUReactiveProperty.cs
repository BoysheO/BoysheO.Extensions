namespace UReactive.Abstractions
{
    public interface IUReactiveProperty<T> : IUReadOnlyReactiveProperty<T>
    {
        T Value { set; }
    }

    public interface IUReadOnlyReactiveProperty<out T>
    {
        T Value { get; }
        IUObservable<T> onValueChanged { get; }
    }
}