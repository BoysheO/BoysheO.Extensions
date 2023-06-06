using System;

namespace AA.Abstraction
{
    public interface IObservableEvent<out T>
    {
        event Action<T> OnNext;
    }
}