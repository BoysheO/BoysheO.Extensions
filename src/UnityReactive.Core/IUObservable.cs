using System;

namespace UnityReactive.Core
{
    public interface IUObservable<T>
    {
        Unsubscribe Subscribe(IUObserver<T> observer);
    }
}