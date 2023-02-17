using System.Collections.Generic;

namespace Hrx.Abstraction.Collections
{
    public struct InsertEv<T>
    {
        public readonly T Value;
        public readonly int Index;

        public InsertEv(T value, int index)
        {
            Value = value;
            Index = index;
        }
    }

    public struct RemoveEv<T>
    {
        public readonly T Value;
        public readonly int Index;

        public RemoveEv(T value, int index)
        {
            Value = value;
            Index = index;
        }
    }

    public struct Replace<T>
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

    public interface IEventList<T>
    {
        IReadOnlyList<T> Content { get; }
        IEventProperty<T> Version { get; }
        IEvent<InsertEv<T>> onInsert { get; }
        IEvent<RemoveEv<T>> onRemove { get; }
        IEvent<Replace<T>> onReplace { get; }
    }
}