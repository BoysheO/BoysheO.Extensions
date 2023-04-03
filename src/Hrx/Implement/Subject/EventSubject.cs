using System;

namespace Hrx
{
    public class EventSubject<T> : IEvent<T>, IEvent<Unit>
    {
        private Exception? _exception;
        private event Action<T, Exception?>? _onNext;
        private event Action<Unit, Exception?>? _onUnit;

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

        event Action<Unit, Exception?>? IEvent<Unit>.onNext
        {
            add
            {
                if (value == null) return;
                if (_exception != null)
                {
                    value.Invoke(default, _exception);
                }

                _onUnit += value;
            }
            remove => _onUnit -= value;
        }

        public IEvent<Unit> AsUnit => this;

        public void OnNext(T value)
        {
            if (_exception != null) return;
            _onNext?.Invoke(value, null);
            _onUnit?.Invoke(Unit.Default, null);
        }

        public void OnDead(Exception? ex)
        {
            if (_exception != null) return;
            _exception = ex;
            _onNext?.Invoke(default, _exception);
            _onNext = null;
            _onUnit?.Invoke(Unit.Default, _exception);
            _onUnit = null;
        }
    }
}