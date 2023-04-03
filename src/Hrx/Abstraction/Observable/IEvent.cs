using System;

namespace Hrx
{
    public interface IEvent<out T>
    {
        event Action<T, Exception?> onNext;
        IEvent<Unit> AsUnit { get; }
    }
}