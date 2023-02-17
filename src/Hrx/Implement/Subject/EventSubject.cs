using System;

namespace Hrx
{
    public class EventSubject<T> : IEvent<T>
    {
        private Exception? _exception;
        private event Action<T, Exception?>? _onNext;

        public event Action<T, Exception?>? onNext
        {
            add
            {
                if (value == null) return;
                if (_exception != null)
                {
                    value.Invoke(default, _exception);
                }

                _onNext += value;
            }
            remove => _onNext -= value;
        }

        public void OnNext(T value)
        {
            if (_exception != null) return;
            _onNext?.Invoke(value, null);
        }

        public void OnDead(Exception? ex)
        {
            if (_exception != null) return;
            _exception = ex;
            _onNext?.Invoke(default, _exception);
            _onNext = null;
        }
    }
}