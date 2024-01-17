using System;

namespace UnityReactive.Core
{
    /// <summary>
    /// call dispose at the end or cause memory leak!
    /// </summary>
    public readonly struct Observable : IEquatable<Observable>, IDisposable
    {
        public Type Type { get; init; }
        private long Ptr { get; init; }
        public bool IsSubscribeOnly { get; init; }

        public bool IsDead
        {
            get
            {
                return UnityReactiveManager.Instance.IsDead(Ptr);
            }
        }

        /// <summary>
        /// dirty read
        /// </summary>
        public int ObserverCount
        {
            get { return UnityReactiveManager.Instance.ObserverCount(Ptr); }
        }

        private void ThrowIfInvalidPtr()
        {
            if (Ptr == 0) throw new Exception("invalid observable");
        }

        public Observable ToSubscribeOnly
        {
            get
            {
                ThrowIfInvalidPtr();
                return new Observable()
                {
                    Type = this.Type,
                    Ptr = Ptr,
                    IsSubscribeOnly = true
                };
            }
        }

        public Unsubscribe Subscribe<T>(IUObserver<T> observer)
        {
            ThrowIfInvalidPtr();
            if (typeof(T) != Type) throw new Exception($"type mismatch obType={Type},observerType={typeof(T)}");
            return Subscribe(observer);
        }

        public Unsubscribe Subscribe(IUObserver observer)
        {
            ThrowIfInvalidPtr();
            var ptr = UnityReactiveManager.Instance.Subscribe(Ptr, observer);
            return new Unsubscribe()
            {
                Ptr = ptr
            };
        }

        public void OnNext<T>(T element)
        {
            ThrowIfInvalidPtr();
            if (IsSubscribeOnly) throw new Exception("IsSubscribeReadOnly");
            if (typeof(T) != Type) throw new Exception($"type mismatch obType={Type},elementType={typeof(T)}");
            UnityReactiveManager.Instance.OnNext(Ptr, element);
        }
        
        public static Observable Creat(Type type, string? trackingMsg = null)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            var ptr = UnityReactiveManager.Instance.CreatObservable(type, trackingMsg);
            return new Observable()
            {
                Ptr = ptr,
                Type = type
            };
        }

        public bool Equals(Observable other)
        {
            ThrowIfInvalidPtr();
            return Ptr == other.Ptr;
        }

        public void Dispose()
        {
            if (Ptr == 0) return;
            if (!IsSubscribeOnly) UnityReactiveManager.Instance.OnCompeted(Ptr);
        }
    }
}