using System;

namespace Hrx
{
    /// <summary>
    /// 此事件源只可能发出OnNext事件
    /// </summary>
    public interface INObservable<out T>
    {
        void Subscribe(IObserver<T> observer);
        void Unsubscribe(IObserver<T> observer);
    }
}