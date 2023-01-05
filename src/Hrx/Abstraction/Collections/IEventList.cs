using System.Collections.Generic;

namespace Hrx.Abstraction.Collections
{
    public interface IEventList<out T>
    {
        public struct InsertEv
        {
            public readonly T Value;
            public readonly int Index;

            public InsertEv(T value, int index)
            {
                Value = value;
                Index = index;
            }
        }

        public struct RemoveEv
        {
            public readonly T Value;
            public readonly int Index;

            public RemoveEv(T value, int index)
            {
                Value = value;
                Index = index;
            }
        }

        public struct Replace
        {
            public readonly T OldValue;
            public readonly T NewValue;
            public readonly int Index;

            public Replace(T oldValue, T newValue, int index)
            {
                OldValue = oldValue;
                NewValue = newValue;
                Index = index;
            }
        }

        IReadOnlyList<T> Content { get; }
        IEventProperty<T> Version { get; }
        IEvent<InsertEv> onInsert { get; }
        IEvent<RemoveEv> onRemove { get; }
        IEvent<Replace> onReplace { get; }
    }
}