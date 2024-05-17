// See https://aka.ms/new-console-template for more information

using BoysheO.Extensions;

Console.WriteLine("Hello, World!");

var l = new SortedList<int,int>();
l.Add(1,1);
l.Add(1,2);
Console.WriteLine(l.Select(v=>$"{v.Key}:{v.Value}").JoinAsOneString("|"));