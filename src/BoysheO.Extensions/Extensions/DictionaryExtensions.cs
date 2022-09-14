using System;
using System.Collections.Generic;

namespace BoysheO.Extensions
{
    public static class DictionaryExtensions
    {
        /// <summary>
        ///     将一个element添加到key-list字典里去<br />
        ///     ！会自动添加缺少的key、list<br />
        ///     ！阻止null element添加<br />
        ///     *如果TCollection不是new出来的，那么应考虑重新封装Dic对象为独立的数据结构以减少gc<br />
        /// </summary>
        public static void AddToInnerList<TKey, TItem, TCollection>
        (this IDictionary<TKey, TCollection> dic,
            TKey key,
            TItem item)
            where TCollection : ICollection<TItem>, new()
        {
            if (dic == null) throw new ArgumentNullException(nameof(dic));
            if (item is null) throw new ArgumentNullException(nameof(item)); //阻止null元素
            if (!dic.ContainsKey(key)) dic.Add(key, new TCollection());
            dic[key] ??= new TCollection();
            dic[key].Add(item);
        }

        /// <summary>
        ///     将一组element合添加到key-list字典里去<br />
        ///     ！会自动添加缺少的key、list<br />
        ///     ！阻止null element添加<br />
        ///     *如果TCollection不是new出来的，那么应考虑重新封装Dic对象为独立的数据结构以减少gc<br />
        /// </summary>
        public static void AddRangeToInnerList<TKey, TItem, TCollection>
            (this IDictionary<TKey, TCollection> dic, TKey key, IEnumerable<TItem> items)
            where TCollection : ICollection<TItem>, new()
        {
            if (dic == null) throw new ArgumentNullException(nameof(dic));
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (items == null) throw new ArgumentNullException(nameof(items));
            if (!dic.ContainsKey(key)) dic.Add(key, new TCollection());
            dic[key] ??= new TCollection();
            foreach (var i in items)
            {
                if (i is null) throw new Exception("null element was rejected");
                dic[key].Add(i);
            }
        }

        /// <summary>
        ///     移除一个元素
        ///     ！keepEmptyList决定Key-List的List为空时是否删除key-List对
        ///     ！删除key-list对时，如果List继承了IDisposable，会调用dispose
        /// </summary>
        public static bool RemoveFromInnerList<TKey, TItem, TCollection>(this IDictionary<TKey, TCollection> dic,
            TKey key,
            TItem item,
            bool keepEmptyList = false)
            where TCollection : ICollection<TItem>
        {
            if (dic == null) throw new ArgumentNullException(nameof(dic));
            if (!dic.TryGetValue(key, out var lst)) return false;
            if (lst == null)
                throw new Exception("the dictionary has null element rejected"); //阻止夹杂list为null的字典操作，提前发现问题
            var res = lst.Remove(item);
            if (keepEmptyList || lst.Count != 0) return res;
            dic.Remove(key);
            (lst as IDisposable)?.Dispose();

            return res;
        }

        /// <returns>is replace success</returns>
        public static bool TryReplaceIfKeyExist<TK, TV>(this IDictionary<TK, TV> dic, TK key, TV value)
        {
            if (!dic.ContainsKey(key)) return false;
            dic[key] = value;
            return true;
        }
    }
}