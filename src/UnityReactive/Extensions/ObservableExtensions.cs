using System;
using UnityReactive.Core;
using UnityReactive.Implement;

namespace UnityReactive.Extensions
{
    public static class ObservableExtensions
    {
        public static Unsubscribe Subscribe<T>(this Observable observable,
            Action<T>? onNext = null,
            Action? onCompeted = null)
        {
            if (onNext == null && onCompeted == null) throw new Exception("one of them must not null");
            if (onNext == null)
            {
                return observable.Subscribe(new SubscribeOnCompeted(onCompeted!));
            }

            if (onCompeted == null)
            {
                return observable.Subscribe(new SubscribeOnNext<T>(onNext));
            }

            return observable.Subscribe(new Subscribe<T>(onNext, onCompeted));
        }
    }
}