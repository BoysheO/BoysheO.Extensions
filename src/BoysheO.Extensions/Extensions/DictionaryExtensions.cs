using System;
using System.Collections.Generic;

namespace BoysheO.Extensions
{
    public static class DictionaryExtensions
    {
        /// <summary>
        ///     如果没有里表，会自动创建
        ///     <para>不会对dic判空</para>
        /// </summary>
        public static void AddToInnerList<TKey, TItem, TCollection>
            (this IDictionary<TKey, TCollection> dic, TKey key, TItem item)
            where TCollection : ICollection<TItem>, new()
        {
            if (item is null) throw new ArgumentNullException(nameof(item)); //阻止null元素
            if (!dic.ContainsKey(key)) dic.Add(key, new TCollection());
            dic[key] ??= new TCollection();
            dic[key].Add(item);
        }

        /// <summary>
        ///     如果没有里表，会自动创建
        ///     <para>不会对dic\items判空</para>
        /// </summary>
        public static void AddToInnerList<TKey, TItem, TCollection>
            (this IDictionary<TKey, TCollection> dic, TKey key, IEnumerable<TItem> items)
            where TCollection : ICollection<TItem>, new()
        {
            if (!dic.ContainsKey(key)) dic.Add(key, new TCollection());
            dic[key] ??= new TCollection();
            foreach (var i in items)
            {
                if (i is null) throw new Exception("null element was rejected");
                dic[key].Add(i);
            }
        }

        /// <summary>
        ///     如果没有里表，会自动创建
        /// </summary>
        public static void AddToInnerList<TKey, TItem, TCollection>
            (this IDictionary<TKey, TCollection> dic, TKey key, TItem item, Func<TCollection> creator)
            where TCollection : ICollection<TItem>
        {
            if (!dic.ContainsKey(key)) dic.Add(key, creator());
            dic[key] ??= creator();
            dic[key].Add(item);
        }

        /// <summary>
        ///     移除里表物体后，里表为空时删除key
        ///     如果里表继承了IDisposable，则删key时会调用
        /// </summary>
        public static bool RemoveFromInnerList<TKey, TItem, TCollection>(this IDictionary<TKey, TCollection> dic,
            TKey key,
            TItem item, bool keepEmptyList = false)
            where TCollection : ICollection<TItem>
        {
            if (!dic.TryGetValue(key, out var lst)) return false;
            if (lst == null)
                throw new Exception("the dictionary has null element rejected"); //阻止夹杂Value=null的字典操作，提前发现问题
            var res = lst.Remove(item);
            if (keepEmptyList || lst.Count != 0) return res;
            dic.Remove(key);
            (lst as IDisposable)?.Dispose();

            return res;
        }

        /// <returns>is replace success</returns>
        public static bool ReplaceValueIfKeyExist<TK, TV>(this IDictionary<TK, TV> dic, TK key, TV value)
        {
            if (!dic.ContainsKey(key)) return false;
            dic[key] = value;
            return true;
        }

        #region about add

        /// <summary>
        ///     没有返回值要求的话，优先使用索引器而不是此API
        /// </summary>
        public static bool AddOrReplaceKv<TK, TV>(this IDictionary<TK, TV> dic, TK key, TV value)
        {
            if (dic.ContainsKey(key))
            {
                dic[key] = value;
                return false;
            }

            dic.Add(key, value);
            return true;
        }

        /// <summary>
        ///     没有返回值要求的话，优先使用索引器而不是此API
        /// </summary>
        public static bool AddOrReplaceKv<TK, TV>(this IDictionary<TK, TV> dic, KeyValuePair<TK, TV> kv)
        {
            return dic.AddOrReplaceKv(kv.Key, kv.Value);
        }

        /// <summary>
        ///     加入了新key的话返回true
        /// </summary>
        public static bool AddKeyWithDefaultValue<TK, TV>(this IDictionary<TK, TV> dic, TK key)
        {
            if (dic.ContainsKey(key)) return false;
            dic.Add(key, default!);
            return true;
        }

        /// <summary>
        ///     加入了新key的话返回true
        /// </summary>
        public static bool AddKeyWithNewValue<TK, TV>(this IDictionary<TK, TV> dic, TK key) where TV : new()
        {
            if (dic.ContainsKey(key)) return false;
            dic.Add(key, new TV());
            return true;
        }

        #endregion

        #region about get

        public static T? GetValueOrDefault<TK, T>(this IDictionary<TK, T> dic, TK key)
            where T : class
        {
            return dic.TryGetValue(key, out var v) ? v : null;
        }

        public static T? GetValueOrDefault1<TK, T>(this IDictionary<TK, T> dic, TK key)
            where T : struct
        {
            return dic.TryGetValue(key, out var v) ? (T?) v : null;
        }

        /// <summary>
        ///     可能会产生具有null元素的字典
        /// </summary>
        public static TV GetValueOrCreatDefault<TK, TV>(this IDictionary<TK, TV> dic, TK key)
        {
            if (dic.TryGetValue(key, out var v)) return v;

            v = default!;
            dic.Add(key, v);
            return v;
        }

        /// <summary>
        ///     如果没有key，会返回false
        /// </summary>
        public static bool TryGetValueOrCreatDefaultOut<TK, TV>(this IDictionary<TK, TV> dic, TK k, out TV v)
        {
            if (!dic.TryGetValue(k, out v)) return false;
            if (v != null) return false;
            v = default!;
            dic[k] = v;
            return true;
        }

        #endregion
    }
}