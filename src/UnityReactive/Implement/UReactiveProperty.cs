using System;
using UnityReactive.Abstractions;
using UnityReactive.Core;

namespace UnityReactive.Implement
{
    public sealed class UReactiveProperty<T> : IUReactiveProperty<T>, IDisposable
    {
        private T _value;

        public T Value
        {
            get => _value;
            set
            {
                _value = value;
                _onValueChanged.OnNext(_value);
            }
        }

        public IUObservable<T> onValueChanged => _onValueChanged;
        private readonly USubject<T> _onValueChanged = new USubject<T>();

        public void Dispose()
        {
            _onValueChanged.Dispose();
        }
    }
}