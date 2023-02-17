using System;
using System.Collections;
using System.Collections.Generic;
using Collections.Pooled;
using UniRx;

//设计上兼容UniRx和Reactive两库，根据需要自行切换
namespace PooledReactiveCollectionScript
{
    public sealed class PooledReactiveCollection<T> : IDisposable, IList<T>, IReadOnlyList<T>, IList
    {
        #region Types

        public readonly struct CollectionAddEvent
        {
            public readonly int Index;
            public readonly T Value;

            public CollectionAddEvent(int index, T value)
                : this()
            {
                Index = index;
                Value = value;
            }

            public override string ToString()
            {
                return  $"Index:{Index} Value:{Value}";
            }
        }

        public readonly struct CollectionRemoveEvent
        {
            public readonly int Index;
            public readonly T Value;

            public CollectionRemoveEvent(int index, T value)
                : this()
            {
                Index = index;
                Value = value;
            }

            public override string ToString()
            {
                return $"Index:{Index} Value:{Value}";
            }
        }

        public readonly struct CollectionMoveEvent
        {
            public readonly int OldIndex;
            public readonly int NewIndex;
            public readonly T Value;

            public CollectionMoveEvent(int oldIndex, int newIndex, T value)
                : this()
            {
                OldIndex = oldIndex;
                NewIndex = newIndex;
                Value = value;
            }

            public override string ToString()
            {
                return $"OldIndex:{OldIndex} NewIndex:{NewIndex} Value:{Value}";
            }
        }

        public readonly struct CollectionReplaceEvent
        {
            public readonly int Index;
            public readonly T OldValue;
            public readonly T NewValue;

            public CollectionReplaceEvent(int index, T oldValue, T newValue)
                : this()
            {
                Index = index;
                OldValue = oldValue;
                NewValue = newValue;
            }

            public override string ToString()
            {
                return $"Index:{Index} OldValue:{OldValue} NewValue:{NewValue}";
            }
        }

        #endregion

        public readonly PooledList<T> PooledList;

        [NonSerialized] bool isDisposed = false;

        public PooledReactiveCollection() : this(new PooledList<T>())
        {
        }

        public PooledReactiveCollection(PooledList<T> source)
        {
            PooledList = source ?? throw new ArgumentNullException(nameof(source));
        }

        public void Clear()
        {
            var beforeCount = PooledList.Count;
            PooledList.Clear();

            collectionReset?.OnNext(Unit.Default);
            if (beforeCount > 0) countChanged?.OnNext(PooledList.Count);

            collectionAddMoveRemoveReplaceReset?.OnNext(Unit.Default);
        }

        public void Insert(int index, T item)
        {
            PooledList.Insert(index, item);

            collectionAdd?.OnNext(new CollectionAddEvent(index, item));
            countChanged?.OnNext(PooledList.Count);
            collectionAddMoveRemoveReplaceReset?.OnNext(Unit.Default);
        }

        public void Move(int oldIndex, int newIndex)
        {
            T item = PooledList[oldIndex];
            PooledList.RemoveAt(oldIndex);
            PooledList.Insert(newIndex, item);

            collectionMove?.OnNext(new CollectionMoveEvent(oldIndex, newIndex, item));
            collectionAddMoveRemoveReplaceReset?.OnNext(Unit.Default);
        }

        public void RemoveAt(int index)
        {
            T item = PooledList[index];
            PooledList.RemoveAt(index);

            collectionRemove?.OnNext(new CollectionRemoveEvent(index, item));
            countChanged?.OnNext(PooledList.Count);
            collectionAddMoveRemoveReplaceReset?.OnNext(Unit.Default);
        }

        public void Set(int index, T item)
        {
            T oldItem = PooledList[index];
            PooledList[index] = item;

            collectionReplace?.OnNext(new CollectionReplaceEvent(index, oldItem, item));
            collectionAddMoveRemoveReplaceReset?.OnNext(Unit.Default);
        }


        [NonSerialized] Subject<int> countChanged = null;

        public UniRx.IObservable<int> ObserveCountChanged(bool notifyCurrentCount = false)
        {
            if (isDisposed) return Observable.Empty<int>();

            var subject = countChanged ?? (countChanged = new Subject<int>());
            if (notifyCurrentCount)
            {
                // return subject.StartWith(() => _pooledList.Count);
                return subject.StartWith(PooledList.Count);
            }
            else
            {
                return subject;
            }
        }

        [NonSerialized] Subject<Unit> collectionReset = null;

        public UniRx.IObservable<Unit> ObserveReset()
        {
            if (isDisposed) return Observable.Empty<Unit>();
            return collectionReset ?? (collectionReset = new Subject<Unit>());
        }

        [NonSerialized] Subject<Unit> collectionAddMoveRemoveReplaceReset = null;

        public UniRx.IObservable<Unit> ObserveAnyChanged()
        {
            if (isDisposed) return Observable.Empty<Unit>();
            return collectionAddMoveRemoveReplaceReset ?? (collectionAddMoveRemoveReplaceReset = new Subject<Unit>());
        }

        public void Notify()
        {
            collectionAddMoveRemoveReplaceReset?.OnNext(Unit.Default);
        }

        void ICollection.CopyTo(Array array, int index)
        {
            ((IList)PooledList).CopyTo(array, index);
        }

        [NonSerialized] Subject<CollectionAddEvent> collectionAdd = null;

        public int Count => PooledList.Count;

        public T this[int index]
        {
            get => PooledList[index];
            set => Set(index, value);
        }

        public UniRx.IObservable<CollectionAddEvent> ObserveAdd()
        {
            if (isDisposed) return Observable.Empty<CollectionAddEvent>();
            return collectionAdd ?? (collectionAdd = new Subject<CollectionAddEvent>());
        }

        [NonSerialized] Subject<CollectionMoveEvent> collectionMove = null;

        public UniRx.IObservable<CollectionMoveEvent> ObserveMove()
        {
            if (isDisposed) return Observable.Empty<CollectionMoveEvent>();
            return collectionMove ?? (collectionMove = new Subject<CollectionMoveEvent>());
        }

        [NonSerialized] Subject<CollectionRemoveEvent> collectionRemove = null;

        public UniRx.IObservable<CollectionRemoveEvent> ObserveRemove()
        {
            if (isDisposed) return Observable.Empty<CollectionRemoveEvent>();
            return collectionRemove ?? (collectionRemove = new Subject<CollectionRemoveEvent>());
        }

        [NonSerialized] Subject<CollectionReplaceEvent> collectionReplace = null;

        public UniRx.IObservable<CollectionReplaceEvent> ObserveReplace()
        {
            if (isDisposed) return Observable.Empty<CollectionReplaceEvent>();
            return collectionReplace ?? (collectionReplace = new Subject<CollectionReplaceEvent>());
        }

        void DisposeSubject<TSubject>(ref Subject<TSubject> subject)
        {
            if (subject == null) return;
            try
            {
                subject.OnCompleted();
            }
            finally
            {
                subject.Dispose();
                subject = null;
            }
        }

        #region IDisposable Support

        private bool disposedValue = false;

        private void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    DisposeSubject(ref collectionReset);
                    DisposeSubject(ref collectionAdd);
                    DisposeSubject(ref collectionMove);
                    DisposeSubject(ref collectionRemove);
                    DisposeSubject(ref collectionReplace);
                    DisposeSubject(ref countChanged);
                    PooledList.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion

        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>)PooledList).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(T item)
        {
            PooledList.Add(item);

            collectionAdd?.OnNext(new CollectionAddEvent(PooledList.Count - 1, item));
            countChanged?.OnNext(PooledList.Count);
            collectionAddMoveRemoveReplaceReset?.OnNext(Unit.Default);
        }

        public bool Contains(T item)
        {
            return PooledList.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            ((IList<T>)PooledList).CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            var idx = PooledList.IndexOf(item);
            if (idx < 0) return false;
            RemoveAt(idx);
            return true;
        }

        public int IndexOf(T item)
        {
            return PooledList.IndexOf(item);
        }

        bool ICollection<T>.IsReadOnly => false;

        bool ICollection.IsSynchronized => ((ICollection)PooledList).IsSynchronized;
        object ICollection.SyncRoot => ((ICollection)PooledList).SyncRoot;
        bool IList.IsFixedSize => ((IList)PooledList).IsFixedSize;
        bool IList.IsReadOnly => ((IList)PooledList).IsReadOnly;

        object IList.this[int index]
        {
            get => this[index];
            set => this[index] = (T)value;
        }

        int IList.Add(object value)
        {
            Add((T)value);
            return PooledList.Count - 1;
        }

        bool IList.Contains(object value)
        {
            return ((IList)PooledList).Contains(value);
        }

        int IList.IndexOf(object value)
        {
            return ((IList)PooledList).IndexOf(value);
        }

        void IList.Insert(int index, object value)
        {
            Insert(index, (T)value);
        }

        void IList.Remove(object value)
        {
            Remove((T)value);
        }
    }
}