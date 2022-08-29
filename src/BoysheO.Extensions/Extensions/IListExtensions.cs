using System.Collections.Generic;

namespace BoysheO.Extensions
{
    public static class IListExtensions
    {
        //bug排序量达到40w后无法自己终止,end和start来回替换
        public static void QSort<T>(this IList<T> list, IList<int> compareValue, int start, int end)
        {
            if (list.Count < 2) return;

            void Swap(int idx1, int idx2)
            {
                (list[idx2], list[idx1]) = (list[idx1], list[idx2]);
                (compareValue[idx2], compareValue[idx1]) = (compareValue[idx1], compareValue[idx2]);
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
                            Swap(start, tempend);
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
                            Swap(tempstart, end);
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
    }
}