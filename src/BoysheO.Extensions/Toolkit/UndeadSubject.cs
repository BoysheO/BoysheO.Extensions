using System;

namespace BoysheO.Toolkit
{
    /// <summary>
    ///     线程安全
    /// </summary>
    public sealed class UndeadSubject<T> : IObservable<T>, IObserver<T>
    {
        private readonly object _gate = new object();
        private readonly Node _prefix = new Node();

        public IDisposable Subscribe(IObserver<T> observer)
        {
            lock (_gate)
            {
                _prefix.Next = new Node
                {
                    Next = _prefix.Next,
                    Value = observer
                };
                return new Disposable(this, _prefix.Next);
            }
        }

        public void OnCompleted()
        {
            lock (_gate)
            {
                var p = _prefix.Next;
                _prefix.Next = null;
                while (p != null)
                {
                    p.Value.OnCompleted();
                    p = p.Next;
                }
            }
        }

        public void OnError(Exception error)
        {
            lock (_gate)
            {
                var p = _prefix.Next;
                _prefix.Next = null;
                while (p != null)
                {
                    p.Value.OnError(error);
                    p = p.Next;
                }
            }
        }

        public void OnNext(T value)
        {
            lock (_gate)
            {
                var p = _prefix.Next;
                while (p != null)
                {
                    p.Value.OnNext(value);
                    p = p.Next;
                }
            }
        }

        private class Node
        {
            public Node? Next;
            public IObserver<T> Value = null!;
        }

        private sealed class Disposable : IDisposable
        {
            private readonly Node _node;
            private readonly UndeadSubject<T> _subject;

            public Disposable(UndeadSubject<T> subject, Node node)
            {
                _subject = subject;
                _node = node;
            }

            public void Dispose()
            {
                lock (_subject._gate)
                {
                    var p = _subject._prefix;
                    while (p.Next != null)
                    {
                        if (p.Next != _node) continue;
                        p.Next = p.Next.Next;
                        return;
                    }
                }
            }
        }
    }
}