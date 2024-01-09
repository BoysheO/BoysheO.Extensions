using UnityReactive.Core;

namespace UnityReactive.Abstractions
{
    public interface IUReactiveProperty<T> : IUReadOnlyReactiveProperty<T>
    {
        new T Value { set; }
    }

    public interface IUReadOnlyReactiveProperty<out T>
    {
        T Value { get; }
        Observable onValueChanged { get; }
    }
}