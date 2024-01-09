// using System;
// using System.Buffers;
// using System.Collections;
// using System.Collections.Generic;
// using Collections.Pooled;
// using UnityReactive.Abstractions;
// using UnityReactive.Core;
//
// namespace UnityReactive.Implement
// {
//     public class PooledUObservableCollection<T> : IUObservableCollection<T>, IDisposable, IList<T>
//     {
//         private readonly PooledList<T> _list;
//
//         public PooledUObservableCollection(int capacity)
//         {
//             _list = new PooledList<T>(capacity);
//         }
//
//         public PooledUObservableCollection()
//         {
//             _list = new();
//         }
//
//         public PooledList<T>.Enumerator GetEnumerator()
//         {
//             return _list.GetEnumerator();
//         }
//
//         IEnumerator<T> IEnumerable<T>.GetEnumerator()
//         {
//             return _list.GetEnumerator();
//         }
//
//         IEnumerator IEnumerable.GetEnumerator()
//         {
//             return GetEnumerator();
//         }
//
//         public void Add(T item)
//         {
//             Insert(_list.Count, item);
//         }
//
//         public void Clear()
//         {
//             _list.Clear();
//             if (_onClear.ObserverCount > 0)
//             {
//                 _onClear.OnNext<object>(null!);
//             }
//         }
//
//         public bool Contains(T item)
//         {
//             return _list.Contains(item);
//         }
//
//         public void CopyTo(T[] array, int arrayIndex)
//         {
//             _list.CopyTo(array.AsSpan(arrayIndex));
//         }
//
//         public bool Remove(T item)
//         {
//             var idx = _list.IndexOf(item);
//             if (idx < 0) return false;
//             if (_onRemove.ObserverCount > 0)
//             {
//                 var ev = _removeEvent ??= new();
//                 ev.Element = item;
//                 _onRemove.OnNext(ev);
//             }
//
//             return isRemoved;
//         }
//
//         public int Count => _list.Count;
//
//         public bool IsReadOnly => false;
//
//         public int IndexOf(T item)
//         {
//             return _list.IndexOf(item);
//         }
//
//         public void Insert(int index, T item)
//         {
//             bool isAnyOb = _onInsert.ObserverCount > 0;
//             InsertEvent<T>? ev = null;
//             if (isAnyOb)
//             {
//                 ev = _insertEvent ??= new();
//                 var oldEv = 
//             }
//
//             _list.Insert(index, item);
//         }
//
//         public void RemoveAt(int index)
//         {
//             throw new NotImplementedException();
//         }
//
//         public T this[int index]
//         {
//             get => _list[index];
//             set
//             {
//                 if (index > _list.Count) throw new ArgumentOutOfRangeException(nameof(index));
//                 var ogElement = _list[index];
//                 _list[index] = value;
//                 _version.Value++;
//                 if (_onReplace != null)
//                 {
//                     _replaceEvent.OldElement = ogElement;
//                     _replaceEvent.NewElement = value;
//                     _replaceEvent.Index = index;
//                     _onReplace.Value.OnNext(_replaceEvent);
//                 }
//             }
//         }
//
//         public IUReadOnlyReactiveProperty<int> Version => _version;
//         private readonly UReactiveProperty<int> _version = new UReactiveProperty<int>();
//
//         public Observable onInsert
//         {
//             get
//             {
//                 var ob = _onInsert ??= Observable.Creat(typeof(InsertEvent<T>));
//                 _insertEvent = new();
//                 return ob;
//             }
//         }
//
//         private Observable? _onInsert;
//         private InsertEvent<T>? _insertEvent;
//
//         public Observable onRemove
//         {
//             get
//             {
//                 var ob = _onRemove ??= Observable.Creat(typeof(RemoveEvent<T>));
//                 _removeEvent = new();
//                 return ob;
//             }
//         }
//
//         private Observable? _onRemove;
//         private RemoveEvent<T>? _removeEvent;
//
//         public Observable onClear
//         {
//             get
//             {
//                 _onClear ??= Observable.Creat(typeof(object));
//                 return _onClear.Value;
//             }
//         }
//
//         private Observable? _onClear;
//
//         public Observable onReplace
//         {
//             get
//             {
//                 _onReplace ??= Observable.Creat(typeof(ReplaceEvent<T>));
//                 _replaceEvent ??= new();
//                 return _onReplace.Value;
//             }
//         }
//
//         private Observable? _onReplace;
//         private ReplaceEvent<T>? _replaceEvent;
//
//         public IList<T> AsList => this;
//
//         public void Dispose()
//         {
//             _onInsert?.Dispose();
//             _onRemove?.Dispose();
//             _onClear?.Dispose();
//             _onReplace?.Dispose();
//             _list.Dispose();
//         }
//     }
// }