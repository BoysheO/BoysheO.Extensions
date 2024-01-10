// See https://aka.ms/new-console-template for more information

using DrawPoolStringSystem;

Console.WriteLine("Hello, World!");

const string str = "1[1,2~4];2[5];1[6]";
//出现概率百分比应为 1,2,3,4=0.0625 ，5=0.5，6=0.25
// const string str = "1[1];1[2]";

var ins = DrawPoolString.Creat(str);
List<int> allList = new List<int>();

for (int i = 0; i < 1000000; i++)
{
    var v = ins.Draw(Random.Shared.Next(), Random.Shared.Next());
    allList.Add(v);
}

var chance1 = allList.Count(v => v == 1) * 1f / allList.Count;
var chance2 = allList.Count(v => v == 2) * 1f / allList.Count;
var chance3 = allList.Count(v => v == 3) * 1f / allList.Count;
var chance4 = allList.Count(v => v == 4) * 1f / allList.Count;
var chance5 = allList.Count(v => v == 5) * 1f / allList.Count;
var chance6 = allList.Count(v => v == 6) * 1f / allList.Count;
Console.WriteLine(new
{
    chance1,
    chance2,
    chance3,
    chance4,
    chance5,
    chance6
});