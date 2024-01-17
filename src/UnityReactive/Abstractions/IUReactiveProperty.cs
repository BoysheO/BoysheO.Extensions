using UnityReactive.Core;

namespace UnityReactive.Abstractions
{
    public interface IUReactiveProperty<T> : IUReadOnlyReactiveProperty<T>
    {
        new T Value { set; }
    }

    public interface IUReadOnlyReactiveProperty<T>
    {
        T Value { get; }
        IUObservable<T> onValueChanged { get; }
    }
}