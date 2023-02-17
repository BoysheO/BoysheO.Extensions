using System;

namespace Hrx
{
    public interface IEvent<out T>
    {
        event Action<T, Exception?> onNext;
    }
}