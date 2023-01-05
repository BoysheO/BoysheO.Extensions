using System;
using System.Buffers;

namespace Hrx.Implement
{
    public sealed class NCSubject<T> : INCObservable<T>
    {
        private IObserver<T>[]? _observers;
        private int _observersCount = 0; //-1:Dead

        public void OnNext(T value)
        {
            if (_observersCount <= 0 || _observers == null) return; //dead or no observer
            var bufCount = _observersCount;
            var buf = ArrayPool<IObserver<T>>.Shared.Rent(bufCount);
            Array.Copy(_observers, buf, bufCount);
            for (int i = 0; i < bufCount; i++)
            {
                buf[i].OnNext(value);
            }

            Array.Clear(buf, 0, bufCount);
            ArrayPool<IObserver<T>>.Shared.Return(buf);
        }

        public void OnCompete()
        {
            if (_observersCount <= 0 || _observers == null) return; //dead or no observer
            var bufCount = _observersCount;
            _observersCount = -1; //dead
            var buf = ArrayPool<IObserver<T>>.Shared.Rent(bufCount);
            Array.Copy(_observers, buf, bufCount);
            for (int i = 0; i < bufCount; i++)
            {
                buf[i].OnCompleted();
            }

            Array.Clear(buf, 0, bufCount);
            ArrayPool<IObserver<T>>.Shared.Return(buf);
        }

       public void Subscribe(IObserver<T> observer)
        {
            if (_observersCount < 0)
            {
                observer.OnCompleted();
                return;
            }

            if (_observers == null)
            {
                _observers = ArrayPool<IObserver<T>>.Shared.Rent(1);
            }
            else if (_observers.Length <= _observersCount)
            {
                var newBuf = ArrayPool<IObserver<T>>.Shared.Rent(_observersCount + 1);
                Array.Copy(_observers, newBuf, _observersCount);
                Array.Clear(_observers, 0, _observersCount);
                ArrayPool<IObserver<T>>.Shared.Return(_observers);
                _observers = newBuf;
            }

            _observers[_observersCount] = observer;
            _observersCount++;
        }

        public void Unsubscribe(IObserver<T> observer)
        {
            if (_observersCount <= 0 || _observers == null) return; //dead or no observer
            var idx = Array.IndexOf(_observers, observer);
            if (idx >= 0)
            {
                for (; idx < _observersCount - 1; idx++)
                {
                    _observers[idx] = _observers[idx + 1];
                }
            }
        }
    }
}