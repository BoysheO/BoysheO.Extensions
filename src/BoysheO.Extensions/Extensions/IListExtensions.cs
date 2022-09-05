using System;
using System.Buffers;
using System.Collections.Generic;
using BoysheO.Toolkit;

namespace BoysheO.Extensions
{
    public static class IListExtensions
    {
        //bug排序量达到40w后无法自己终止,end和start来回替换
        [Obsolete("调试中")]
        public static void QSort<T>(this IList<T> list, IList<int> compareValue, int start, int end)
        {
            if (list.Count < 2) return;

            void Swap(ref int idx1, ref int idx2)
            {
                (list[idx2], list[idx1]) = (list[idx1], list[idx2]);
                (compareValue[idx2], compareValue[idx1]) = (compareValue[idx1], compareValue[idx2]);
                (idx1, idx2) = (idx2, idx1);
            }

            bool flag = true;
            while (end != start)
            {
                if (flag)
                {
                    int tempend = list.Count - 1;
                    while (start < tempend)
                    {
                        if (compareValue[start] > compareValue[tempend]) //右侧找比自己小的数
                        {
                            Swap(ref start, ref tempend);
                            flag = false;
                            break;
                        }
                        else
                        {
                            tempend--;
                            if (start == tempend)
                            {
                                start++;
                                flag = false;
                            }
                        }
                    }
                }
                else
                {
                    int tempstart = 0;
                    while (tempstart < end)
                    {
                        if (compareValue[tempstart] > compareValue[end]) //左侧找比自己大的数
                        {
                            Swap(ref tempstart, ref end);
                            flag = true;
                            break;
                        }
                        else
                        {
                            tempstart++;
                            if (tempstart == end)
                            {
                                end--;
                                flag = true;
                            }
                        }
                    }
                }
            }
        }


        [Obsolete("调试中")]
        public static void QSortABC<T>(this IList<T> list, IList<int> compareValue, int start, int end)
        {
            if (list.Count < 2) return;
            var count = list.Count;
            var buff = ArrayPool<T>.Shared.Rent(count);
            list.CopyTo(buff,0);
            Array.Sort(buff, 0, count, new ComparerAdapter<T>((arg1, arg2) =>
            {
                var idx1 = list.IndexOf(arg1);
                var v = compareValue[idx1];
                var idx2 = list.IndexOf(arg2);
                var v2 = compareValue[idx2];
                return v - v2;
            }));
        }


        //自己写的方法
        [Obsolete("调试中")]
        public static void change(ref int a, ref int b)
        {
            (b, a) = (a, b);
        }

        [Obsolete("调试中")]
        public static int[] KSsort(int[] a, int start, int end)
        {
            bool flag = true;
            while (end != start)
            {
                if (flag)
                {
                    int tempend = a.Length - 1;
                    while (start < tempend)
                    {
                        if (a[start] > a[tempend]) //右侧找比自己小的数
                        {
                            change(ref a[start], ref a[tempend]);
                            flag = false;
                            break;
                        }
                        else
                        {
                            tempend--;
                            if (start == tempend)
                            {
                                start++;
                                flag = false;
                            }
                        }
                    }
                }
                else
                {
                    int tempstart = 0;
                    while (tempstart < end)
                    {
                        if (a[tempstart] > a[end]) //左侧找比自己大的数
                        {
                            change(ref a[tempstart], ref a[end]);
                            flag = true;
                            break;
                        }
                        else
                        {
                            tempstart++;
                            if (tempstart == end)
                            {
                                end--;
                                flag = true;
                            }
                        }
                    }
                }
            }

            return a;
        }
    }
}