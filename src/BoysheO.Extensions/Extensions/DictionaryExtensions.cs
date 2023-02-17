//普适性不高，删之
// using System;
// using System.Collections.Generic;
//
// namespace BoysheO.Extensions
// {
//     public static class DictionaryExtensions
//     {
//         /// <summary>
//         ///     Add element to a dictionary value type is collection<br />
//         ///     If the key missing in dictionary,the method will creat one and put it in dic. 
//         /// </summary>
//         public static void AddToInnerList<TKey, TItem, TCollection>
//             (this IDictionary<TKey, TCollection> dic, TKey key, TItem item)
//             where TCollection : ICollection<TItem>, new()
//         {
//             if (dic == null) throw new ArgumentNullException(nameof(dic));
//             if (key == null) throw new ArgumentNullException(nameof(key));
//             if (item is null) throw new ArgumentNullException(nameof(item)); //阻止null元素
//             if (!dic.ContainsKey(key)) dic.Add(key, new TCollection());
//             dic[key] ??= new TCollection();
//             dic[key].Add(item);
//         }
//
//         /// <summary>
//         ///     Add elements to a dictionary
//         ///     <para>不会对dic\items判空</para>
//         /// </summary>
//         public static void AddToInnerList<TKey, TItem, TCollection>
//             (this IDictionary<TKey, TCollection> dic, TKey key, IEnumerable<TItem> items)
//             where TCollection : ICollection<TItem>, new()
//         {
//             if (dic == null) throw new ArgumentNullException(nameof(dic));
//             if (key == null) throw new ArgumentNullException(nameof(key));
//             if (items == null) throw new ArgumentNullException(nameof(items));
//             if (!dic.ContainsKey(key)) dic.Add(key, new TCollection());
//             dic[key] ??= new TCollection();
//             foreach (var i in items)
//             {
//                 if (i is null) throw new Exception("null element was rejected");
//                 dic[key].Add(i);
//             }
//         }
//
//         /// <summary>
//         ///     移除里表物体后，里表为空时删除key
//         ///     如果里表继承了IDisposable，则删key时会调用
//         /// </summary>
//         public static bool RemoveFromInnerList<TKey, TItem, TCollection>(this IDictionary<TKey, TCollection> dic,
//             TKey key,
//             TItem item, bool keepEmptyList = false)
//             where TCollection : ICollection<TItem>
//         {
//             if (!dic.TryGetValue(key, out var lst)) return false;
//             if (lst == null)
//                 throw new Exception("the dictionary has null element rejected"); //阻止夹杂Value=null的字典操作，提前发现问题
//             var res = lst.Remove(item);
//             if (keepEmptyList || lst.Count != 0) return res;
//             dic.Remove(key);
//             (lst as IDisposable)?.Dispose();
//
//             return res;
//         }
//     }
// }
