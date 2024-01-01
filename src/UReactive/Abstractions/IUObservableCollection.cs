using System;
using System.Collections.Generic;

namespace UReactive.Abstractions
{
    public interface IUObservableCollection<T>:IUReadOnlyObservableCollection<T>,IList<T>
    {
    }
    
    public interface IUReadOnlyObservableCollection<T>:IReadOnlyList<T>
    {
        IUReadOnlyReactiveProperty<int> Version { get; } 
        IUObservable<InsertEvent<T>> onInsert { get;  }
        IUObservable<RemoveEvent<T>> onRemove { get; }
        IUObservable<Unit> onClear { get; }
        IUObservable<ReplaceEvent<T>> onReplace { get; }
    }

    public class InsertEvent<T>
    {
        public int Index { get; set; }
        public T Element { get; set; }
    }

    public class RemoveEvent<T>
    {
        public int Index { get; set; }
        public T Element { get; set; }
    }

    public class ReplaceEvent<T>
    {
        public int Index { get; set; }
        public T NewElement { get; set; }
        public T OldElement { get; set; }
    }
}