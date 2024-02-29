// See https://aka.ms/new-console-template for more information

using DrawPoolStringSystem;

Console.WriteLine("Hello, World!");

const string str = "1[1,2~4]2[5]1[6](10~20)";
//出现概率百分比应为 1,2,3,4=0.0625 ，5=0.5，6=0.25

// const string str = "1[1,2~4];2[5];1[6]";// 1[(1,1,1),(2,2,2),(3,1,1)~(3,1,2)];...;*（1～10）
//出现概率百分比应为 1,2,3,4=0.0625 ，5=0.5，6=0.25
// const string str = "1[1];1[2]";

var ins = DrawPoolString3.Creat(str);
List<int> allList = new List<int>();

for (int i = 0; i < 1000000; i++)
{
    var lst = new List<int>();
    ins.Draw(lst, Random.Shared.Next());
    allList.AddRange(lst);
}

var chance1 = allList.Count(v => v == 1) * 1d / allList.Count;
var chance2 = allList.Count(v => v == 2) * 1d / allList.Count;
var chance3 = allList.Count(v => v == 3) * 1d / allList.Count;
var chance4 = allList.Count(v => v == 4) * 1d / allList.Count;
var chance5 = allList.Count(v => v == 5) * 1d / allList.Count;
var chance6 = allList.Count(v => v == 6) * 1d / allList.Count;
Console.WriteLine(new
{
    count=allList.Count,
    chance1,
    chance2,
    chance3,
    chance4,
    chance5,
    chance6
});