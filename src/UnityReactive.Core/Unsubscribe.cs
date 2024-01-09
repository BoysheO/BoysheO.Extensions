using System;

namespace UnityReactive.Core
{
    public readonly struct Unsubscribe : IEquatable<Unsubscribe>, IDisposable
    {
        internal long Ptr { get; init; }

        public bool Equals(Unsubscribe other)
        {
            if (Ptr == 0) throw new Exception("invalid observable");
            return other.Ptr == Ptr;
        }

        public void Dispose()
        {
            if (Ptr == 0) return;
            UnityReactiveManager.Instance.Unsubscribe(Ptr);
        }
    }
}