using System;

namespace BoysheO.Util
{
    /// <summary>
    ///     能用list优先用list
    /// </summary>
    [Obsolete("not useful enough,it will be removed in future")]
    public static class ArrayUtil
    {
        /// <summary>
        ///     将元素加到空位上（如已满则resize）
        /// </summary>
        public static void Add<T>(ref T?[] array, T item) where T : class
        {
            if (array == null) throw new ArgumentNullException(nameof(array));
            if (item == null) throw new Exception("null element rejected");
            for (var p = 0; p < array.Length; p++)
                if (array[p] == null)
                {
                    array[p] = item;
                    return;
                }

            Array.Resize(ref array, array.Length + 1);
            array[array.Length-1] = item;
        }

        /// <summary>
        ///     将元素加到空位上（如已满则resize）
        /// </summary>
        public static void Add<T>(ref T[] array, T item, Func<T, bool> isCanBeReplace)
        {
            if (array == null) throw new ArgumentNullException(nameof(array));
            if (item is null) throw new Exception("null element rejected");
            if (isCanBeReplace == null) throw new ArgumentNullException(nameof(isCanBeReplace));
            for (var p = 0; p < array.Length; p++)
                if (isCanBeReplace(array[p]))
                {
                    array[p] = item;
                    return;
                }

            Array.Resize(ref array, array.Length + 1);
            array[array.Length-1] = item;
        }

        /// <summary>
        ///     resize并添加到最后一位，会创建array
        ///     保留这个API，提醒自己不能这样用
        /// </summary>
        [Obsolete("chaos about new", true)]
        public static void AddToNew<T>(ref T[]? array, T item)
        {
            if (array == null)
            {
                array = new[] {item};
                return;
            }

            Array.Resize(ref array, array.Length + 1);
            array[array.Length-1] = item;
        }

        /// <summary>
        ///     如果数组已满则插入失败。IsCanBeReplace为true的元素视作空元素会被替换
        /// </summary>
        public static bool Insert<T>(ref T[] array, int idx, T item, Func<T, bool> isCanBeReplace)
        {
            if (idx >= array.Length) throw new ArgumentOutOfRangeException(nameof(idx));
            if (array == null) throw new ArgumentNullException(nameof(array));
            if (isCanBeReplace == null) throw new ArgumentNullException(nameof(isCanBeReplace));
            if (isCanBeReplace(array[idx]))
            {
                array[idx] = item;
                return true;
            }

            var p = idx;
            var len = array.Length;
            while (p < len && isCanBeReplace(array[p]) == false) p++;
            if (p == len) return false; //array is full
            for (; p > idx; p--) array[p] = array[p - 1];
            array[idx] = item;
            return true;
        }

        /// <summary>
        ///     如果数组已满则插入失败。
        /// </summary>
        public static bool Insert<T>(ref T?[] array, int idx, T item) where T : class
        {
            if (idx >= array.Length) throw new ArgumentOutOfRangeException(nameof(idx));
            if (array == null) throw new ArgumentNullException(nameof(array));
            if (array[idx] == null)
            {
                array[idx] = item;
                return true;
            }

            var p = idx;
            var len = array.Length;
            while (p < len && array[p] != null) p++;
            if (p == len) return false; //array is full
            for (; p > idx; p--) array[p] = array[p - 1];
            array[idx] = item;
            return true;
        }
    }
}