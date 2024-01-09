using System;
using System.Collections.Generic;
using UnityReactive.Core;

namespace UnityReactive.Abstractions
{
    public interface IUObservableCollection<T> : IUReadOnlyObservableCollection<T>
    {
        public IList<T> AsList { get; }
    }

    public interface IUReadOnlyObservableCollection<T> : IReadOnlyList<T>
    {
        IUReadOnlyReactiveProperty<int> Version { get; }
        Observable onInsert { get; }
        Observable onRemove { get; }
        Observable onClear { get; }
        Observable onReplace { get; }
    }

    public sealed class InsertEvent<T>
    {
        public int Index { get; set; }
        public T Element { get; set; }
    }

    public sealed class RemoveEvent<T>
    {
        public int Index { get; set; }
        public T Element { get; set; }
    }

    public sealed class ReplaceEvent<T>
    {
        public int Index { get; set; }
        public T NewElement { get; set; }
        public T OldElement { get; set; }
    }
}