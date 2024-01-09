using System;
using UnityReactive.Core;

namespace UnityReactive.Implement
{
    internal class SubscribeOnCompeted : IUObserver
    {
        private readonly Action _onCompeted;

        public SubscribeOnCompeted(Action onCompeted)
        {
            _onCompeted = onCompeted ?? throw new ArgumentNullException(nameof(onCompeted));
        }

        public void OnDead()
        {
            _onCompeted();
        }
    }
}