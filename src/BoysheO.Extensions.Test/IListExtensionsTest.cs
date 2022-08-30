using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using NUnit.Framework;

namespace BoysheO.Extensions.Test;

public class IListExtensionsTest
{
    [TestCase]
    public void QSort()
    {
        var rand = new Random();
        var list = new List<string>(rand.Next(0,40000));
        var count = list.Capacity;
        var compareList = new List<int>(count);
        for (int i = 0; i < count; i++)
        {
            var v = rand.Next();
            list.Add(v.ToString());
            compareList.Add(v);
        }
        var clone = new List<string>(list);
        list.QSort(compareList, 0, count-1);
        
        //valid
        clone.Sort((a,b)=>int.Parse(a)-int.Parse(b));
        Assert.AreEqual(list,clone);
    }
    [TestCase]
    public void Main()
    {
        int[] array = new int[1000];
        Random rand = new Random();
        for (int i = 0; i < array.Length; i++)
        {
            array[i] = rand.Next(1001);
        }

        Console.WriteLine("原始数据");
        foreach (var item in array)
        {
            Console.Write(item + "  ");
        }

        Console.WriteLine();

        Console.WriteLine();
        int starttime = DateTime.Now.Millisecond;
        int[] result = IListExtensions.KSsort(array, 0, array.Length - 1);
        foreach (var item in result)
        {
            Console.Write(item + "  ");
        }

        Console.WriteLine("排序耗时 " + (DateTime.Now.Millisecond - starttime));
    }
}