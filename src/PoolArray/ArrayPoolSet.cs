// using System.Buffers;
// using System.Diagnostics.SymbolStore;
// using Extensions;
//
// namespace PoolArray;
//
// public ref struct ArrayPoolSet<T>
// {
//     private readonly IComparer<T> _comparer;
//     private T[] _ary;
//     private int _count;
//
//     public ArrayPoolSet(int capacity, IComparer<T> comparer)
//     {
//         if (comparer == null) throw new ArgumentNullException(nameof(comparer));
//         if (capacity <= 0) throw new ArgumentOutOfRangeException(nameof(capacity));
//         _comparer = comparer;
//         this._ary = ArrayPool<T>.Shared.Rent(capacity);
//     }
//
//     public bool Add(T element)
//     {
//         var idx = Array.BinarySearch(_ary, 0, _count, element, _comparer);
//         if (idx < 0)
//         {
//             idx = ~idx;
//             _count++;
//             if (_count >= _ary.Length) _ary = ArrayPoolUtil.Resize(_ary, idx + 1);
//             for (int i = _count - 1; i > idx; i--)
//             {
//                 _ary[i] = _ary[i - 1];
//             }
//
//             _ary[idx] = element;
//             return true;
//         }
//
//         return false;
//     }
//
//     public Span<T> AsSpan
//     {
//         get
//         {
//             return _ary.AsSpan(0, _count);
//         }
//     }
//
//     public void Dispose()
//     {
//         if (!typeof(T).IsValueType)
//         {
//             Array.Clear(_ary, 0, _count);
//         }
//
//         ArrayPool<T>.Shared.Return(_ary);
//         _ary = null;
//     }
// }