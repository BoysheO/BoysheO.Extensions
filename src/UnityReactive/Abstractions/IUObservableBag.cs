using System;
using System.Collections.Generic;
using UnityReactive.Core;
using UnityReactive.Implement;

namespace UnityReactive.Abstractions
{
    public interface IUReadOnlyObservableBag<T>
    {
        IUReadOnlyReactiveProperty<int> Version { get; }
        IUObservable<T> onAdd { get; }
        IUObservable<T> onRemoved { get; }
        IReadOnlyList<T> AsList { get; }
    }

    public class UObservableBag<T> : IUReadOnlyObservableBag<T>
    {
        IUReadOnlyReactiveProperty<int> IUReadOnlyObservableBag<T>.Version => throw new NotImplementedException();

        public IUObservable<T> onAdd { get; } = new USubject<T>();

        public IUObservable<T> onRemoved { get; } = new USubject<T>();

        public IReadOnlyList<T> AsList => throw new NotImplementedException();
    }
}