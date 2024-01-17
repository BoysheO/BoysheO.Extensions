using System;
using UnityReactive.Core;

namespace UnityReactive.Implement
{
    //safe subject
    public sealed class USubject<T> : IUObservable<T>, IDisposable
    {
        private readonly Observable _observable;

        public USubject() : this(null)
        {
        }

        public USubject(string? tracking)
        {
            _observable = Observable.Creat(typeof(T), tracking);
        }

        public Unsubscribe Subscribe(IUObserver observer)
        {
            return _observable.Subscribe(observer);
        }

        public void OnNext(T value)
        {
            _observable.OnNext(value);
        }

        private void ReleaseUnmanagedResources()
        {
            _observable.Dispose();
        }

        private void Dispose(bool disposing)
        {
            ReleaseUnmanagedResources();
            if (disposing)
            {
                _observable.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public Unsubscribe Subscribe(IUObserver<T> observer)
        {
            return _observable.Subscribe(observer);
        }

        ~USubject()
        {
            Dispose(false);
        }
    }
}