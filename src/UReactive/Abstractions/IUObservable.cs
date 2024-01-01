using System;
using System.Collections.Generic;

namespace UReactive.Abstractions
{
    public interface IUObservable<out T>
    {
        Guid Subscribe(IObserver<T> observer);
        void Unsubscribe(Guid id);
    }
}