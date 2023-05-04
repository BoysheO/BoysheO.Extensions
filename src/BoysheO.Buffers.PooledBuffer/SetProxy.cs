// using System;
// using System.Buffers;
// using System.Collections;
// using System.Collections.Generic;
// using BoysheO.Buffer.PooledBuffer;
// using Collections.Pooled;
//
// namespace BoysheO.Buffers
// {
//     /// <summary>
//     /// SortedSet using ArrayPool
//     /// </summary>
//     /// <typeparam name="T"></typeparam>
//     internal sealed class SortedSet<T> : ISet<T>, IReadOnlyList<T>
//     {
//         public long Version = 1;
//         private T[] array;
//         private int count;
//         const int DefaultCapacity = 4;
//         private IComparer<T> comparer;
//
//         public SortedSet(IComparer<T> comparer)
//         {
//             SortedSet
//             this.comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
//         }
//
//         public struct Enumerable : IEnumerator<T>
//         {
//             private readonly int SetVersion;
//             private readonly SortedSet<T> set;
//             private int index;
//
//             public Enumerable(int setVersion, SortedSet<T> set)
//             {
//                 SetVersion = setVersion;
//                 this.set = set ?? throw new ArgumentNullException(nameof(set));
//                 index = -1;
//             }
//
//             void ThrowIfVersionNotMatch()
//             {
//                 if (SetVersion != set.Version)
//                 {
//                     throw new InvalidOperationException("collection has changed");
//                 }
//             }
//
//             public bool MoveNext()
//             {
//                 ThrowIfVersionNotMatch();
//                 var nextIdx = index + 1;
//                 if (set.Count <= nextIdx) return false;
//                 index = nextIdx;
//                 return true;
//             }
//
//             public void Reset()
//             {
//                 ThrowIfVersionNotMatch();
//                 index = -1;
//             }
//
//             public T Current
//             {
//                 get
//                 {
//                     ThrowIfVersionNotMatch();
//                     return set[index];
//                 }
//             }
//
//             object IEnumerator.Current => Current;
//
//             public void Dispose()
//             {
//                 // do nothing
//             }
//         }
//
//         public Enumerable GetEnumerator()
//         {
//             return new Enumerable((int)Version, this);
//         }
//
//         IEnumerator<T> IEnumerable<T>.GetEnumerator()
//         {
//             return GetEnumerator();
//         }
//
//         IEnumerator IEnumerable.GetEnumerator()
//         {
//             return GetEnumerator();
//         }
//
//         public bool Add(T item)
//         {
//             if (array == null) array = ArrayPool<T>.Shared.Rent(DefaultCapacity);
//             var idx = Array.BinarySearch(array, 0, count, item, comparer);
//             if (idx >= 0) return false;
//             idx = ~idx;
//             if (array.Length == count)
//             {
//                 var newArray = ArrayPool<T>.Shared.Rent(count * 2);
//                 Array.Copy(array, newArray, idx);
//                 Array.Copy(array, idx, newArray, idx + 1, count - idx);
//                 ArrayPool<T>.Shared.Return(array);
//                 array = newArray;
//             }
//             else
//             {
//                 Array.Copy(array, idx, array, idx + 1, count - idx);
//             }
//
//             array[idx] = item;
//             count++;
//             Version++;
//             return true;
//         }
//
//         public void AddRange(IEnumerable<T> items)
//         {
//             //todo make it resize only once
//             foreach (var item in items)
//             {
//                 Add(item);
//             }
//         }
//
//         void ICollection<T>.Add(T item)
//         {
//             Add(item);
//         }
//
//         public void ExceptWith(IEnumerable<T> other)
//         {
//             if (array == null) return;
//             var rawCount = count;
//             foreach (var x1 in other)
//             {
//                 var idx = Array.BinarySearch(array, 0, count, x1, comparer);
//                 if (idx < 0) continue;
//                 Array.Copy(array, idx + 1, array, idx, count - idx - 1);
//                 count--;
//             }
//
//             if (rawCount != count) Version++;
//         }
//
//         public void IntersectWith(IEnumerable<T> other)
//         {
//             if (array == null) return;
//             AddRange(other);
//         }
//
//         private bool IsEmpty(IEnumerable<T> enumerable)
//         {
//             using var itor = enumerable.GetEnumerator();
//             return !itor.MoveNext();
//         }
//
//         public bool IsProperSubsetOf(IEnumerable<T> other)
//         {
//             if (array == null)
//             {
//                 return !IsEmpty(other);
//             }
//
//             
//         }
//
//         public bool IsProperSupersetOf(IEnumerable<T> other)
//         {
//             if (array == null) return false;
//             int local_count = 0;
//             foreach (var x1 in other)
//             {
//                 var idx = Array.BinarySearch(array, 0, local_count, x1, comparer);
//                 if (idx < 0) return false;
//                 local_count++;
//             }
//         }
//
//         public bool IsSubsetOf(IEnumerable<T> other)
//         {
//             throw new System.NotImplementedException();
//         }
//
//         public bool IsSupersetOf(IEnumerable<T> other)
//         {
//             throw new System.NotImplementedException();
//         }
//
//         public bool Overlaps(IEnumerable<T> other)
//         {
//             throw new System.NotImplementedException();
//         }
//
//         public bool SetEquals(IEnumerable<T> other)
//         {
//             throw new System.NotImplementedException();
//         }
//
//         public void SymmetricExceptWith(IEnumerable<T> other)
//         {
//             throw new System.NotImplementedException();
//         }
//
//         public void UnionWith(IEnumerable<T> other)
//         {
//             throw new System.NotImplementedException();
//         }
//
//         bool ISet<T>.Add(T item)
//         {
//             throw new System.NotImplementedException();
//         }
//
//         public void Clear()
//         {
//             throw new System.NotImplementedException();
//         }
//
//         public bool Contains(T item)
//         {
//             throw new System.NotImplementedException();
//         }
//
//         public void CopyTo(T[] array, int arrayIndex)
//         {
//             throw new System.NotImplementedException();
//         }
//
//         public bool Remove(T item)
//         {
//             throw new System.NotImplementedException();
//         }
//
//         public int Count => throw new System.NotImplementedException();
//
//         public bool IsReadOnly => throw new System.NotImplementedException();
//
//         public int IndexOf(T item)
//         {
//             throw new NotImplementedException();
//         }
//
//         public void Insert(int index, T item)
//         {
//             throw new NotImplementedException();
//         }
//
//         public void RemoveAt(int index)
//         {
//             throw new NotImplementedException();
//         }
//
//         public T this[int index]
//         {
//             get => throw new NotImplementedException();
//             set => throw new NotImplementedException();
//         }
//     }
// }