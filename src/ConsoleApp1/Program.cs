// See https://aka.ms/new-console-template for more information

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using BoysheO.Extensions;

Console.WriteLine("Hello, World!");

// var l = new SortedList<int, int>();
// l.Add(1, 1);
// l.Add(1, 2);
// Console.WriteLine(l.Select(v => $"{v.Key}:{v.Value}").JoinAsOneString("|"));

// var ary = new[] {1, 2, 3, 4, 5};
// Array.Copy(ary, 2, ary, 3, 2);
// Console.WriteLine(ary.Select(v=>v.ToString()).JoinAsOneString());

// Func<int, bool> func = x => x > 0;
// Predicate<int> predicate = Unsafe.As<Predicate<int>>(func);
//
// Console.WriteLine(predicate(1)); // 输出: True
// Console.WriteLine(predicate(-1)); // 输出: False
//
// unsafe TOutput UnsafeConvert<TInput, TOutput>(TInput input)
//     where TInput : class
//     where TOutput : class
// {
//     // 使用指针操作进行强制类型转换
//     return *(TOutput*)&input;
// }

// var lst = Enumerable.Range(0, 3).ToList();
// lst.ConvertAll((Converter<int,object>)null);


// int[] array = {  };
//
// // 获取数组第一个元素的引用
// ref int firstElementRef = ref MemoryMarshal.GetArrayDataReference(array);
// Console.WriteLine(firstElementRef);
// // 通过引用修改数组第一个元素的值
// firstElementRef = 10;
//
// // 输出修改后的数组
// Console.WriteLine(string.Join(", ", array)); // 输出: 10, 2, 3, 4, 5

// var eq = EqualityComparer<string>.Default;
// Console.WriteLine(eq.GetType());

var ary = new int[0];
ary.First(v=>v==1);
var ins = new Dictionary<int, int>();
// ins.TryGetValue()

Console.WriteLine("done");