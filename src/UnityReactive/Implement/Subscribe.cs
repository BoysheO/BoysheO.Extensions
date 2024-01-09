using System;
using UnityReactive.Core;

namespace UnityReactive.Implement
{
    internal class Subscribe<T>:IUObserver<T>
    {
        private readonly Action<T> _action;
        private readonly Action _onCompeted;

        public Subscribe(Action<T> action, Action onCompeted)
        {
            _action = action ?? throw new ArgumentNullException(nameof(action));
            _onCompeted = onCompeted ?? throw new ArgumentNullException(nameof(onCompeted));
        }

        public void OnNext(T value)
        {
            _action(value);
        }

        public void OnDead()
        {
            _onCompeted();
        }
    }
}