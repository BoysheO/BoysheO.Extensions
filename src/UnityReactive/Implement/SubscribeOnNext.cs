using System;
using UnityReactive.Core;

namespace UnityReactive.Implement
{
    internal class SubscribeOnNext<T>:IUObserver<T>
    {
        private readonly Action<T> _action;

        public SubscribeOnNext(Action<T> action)
        {
            _action = action ?? throw new ArgumentNullException(nameof(action));
        }

        public void OnNext(T value)
        {
            _action(value);
        }

        public void OnDead()
        {
            //do nothing
        }
    }
}