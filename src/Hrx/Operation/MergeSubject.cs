using System;
using System.Collections.Immutable;

namespace Hrx.Operation
{
    public sealed class MergeSubject<T> : IEvent<T>, IEvent<Unit>
    {
        private readonly ImmutableArray<IEvent<T>> _events;

        public MergeSubject(ImmutableArray<IEvent<T>> events)
        {
            _events = events;
        }

        event Action<T, Exception?>? IEvent<T>.onNext
        {
            add
            {
                //add value to all events
                foreach (var ev in _events)
                {
                    ev.onNext += value;
                }
            }
            remove
            {
                //remove value to all events
                foreach (var ev in _events)
                {
                    ev.onNext -= value;
                }
            }
        }

        event Action<Unit, Exception?>? IEvent<Unit>.onNext
        {
            add
            {
                //add value to all events AsUnit
                foreach (var ev in _events)
                {
                    ev.AsUnit.onNext += value;
                }
            }
            remove
            {
                //remove value to all events AsUnit
                foreach (var ev in _events)
                {
                    ev.AsUnit.onNext -= value;
                }
            }
        }

        public IEvent<Unit> AsUnit => this;
    }
}