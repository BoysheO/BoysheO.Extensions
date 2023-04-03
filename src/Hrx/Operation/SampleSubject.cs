using System;

namespace Hrx.Operation
{
    public sealed class SampleSubject<T> : IEvent<T>,IEvent<Unit>
    {
        private readonly IEvent<T> _event;
        private readonly IEvent<Unit> _sample;

        private T _lastValue;
        private bool _islastValueChanged;

        public SampleSubject(IEvent<T> @event, IEvent<Unit> sample)
        {
            _event = @event;
            _sample = sample;
        }

        public event Action<T, Exception?>? onNext
        {
            add { throw new NotImplementedException(); }
            remove { throw new NotImplementedException(); }
        }

        event Action<Unit, Exception?>? IEvent<Unit>.onNext
        {
            add => throw new NotImplementedException();
            remove => throw new NotImplementedException();
        }

        public IEvent<Unit> AsUnit => this;
    }
}