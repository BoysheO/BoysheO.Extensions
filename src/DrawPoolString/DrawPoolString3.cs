using System;
using System.Collections.Generic;
using System.Linq;
using BoysheO.Extensions;

namespace DrawPoolStringSystem
{
    public class DrawPoolString3
    {
        #region en

        public class Entry
        {
            public int Power;
            public readonly List<(int start, int end)> Values = new List<(int start, int end)>();

            public int GetVirtualSize()
            {
                int count = 0;
                for (var index = 0; index < Values.Count; index++)
                {
                    var (start, end) = Values[index];
                    count += end - start + 1;
                }

                return count;
            }

            public int GetIndex(int vIdx)
            {
                for (var index = 0; index < Values.Count; index++)
                {
                    var (start, end) = Values[index];
                    if (start + vIdx <= end)
                    {
                        return start + vIdx;
                    }

                    vIdx -= end - start + 1;
                }

                throw new IndexOutOfRangeException(nameof(vIdx));
            }
        }

        #endregion

        public (int start, int end) DrawCount;
        public List<Entry> Entries = new List<Entry>();

        private DrawPoolString3()
        {
        }

        public static DrawPoolString3 Creat(string str)
        {
            var ins = new DrawPoolString3();
            var sp = str.Split(']');

            for (int i = 0; i < sp.Length - 1; i++)
            {
                var s = sp[i];
                var ss = s.Split('[');
                var power = ss[0].ParseToInt();
                var values = ss[1];
                if (power < 0) throw new Exception("power must >0");
                var entry = new Entry()
                {
                    Power = power
                };
                foreach (var value in values.Split(','))
                {
                    if (int.TryParse(value, out var v))
                    {
                        if (v < 0) throw new Exception("v<0,reject");
                        entry.Values.Add((v, v));
                    }
                    else
                    {
                        var sss = value.Split('~');
                        var start = sss[0].ParseToInt();
                        var end = sss[1].ParseToInt();
                        if (end < start) (start, end) = (end, start);
                        if (start < 0) throw new Exception($"reject sth < 0,value={value}");
                        entry.Values.Add((start, end));
                    }
                }

                ins.Entries.Add(entry);
            }

            var last = sp.Last();
            if (last.StartsWith("("))
            {
                var ss = last.Split('~');
                var first = ss[0].TrimStart('(').ParseToInt();
                var end = ss[1].TrimEnd(')').ParseToInt();
                if (first > end) (first, end) = (end, first);
                if (first <= 0) throw new Exception($"draw count <0,reject.value={last}");
                ins.DrawCount = (first, end);
            }
            else
            {
                var times = last.ParseToInt();
                if (times <= 0) throw new Exception($"draw count <0,reject.value={last}");
                ins.DrawCount = (times, times);
            }

            return ins;
        }

        public void Draw(IList<int> resultBuffer, int seed)
        {
            resultBuffer.Clear();
            var r = new Random(seed);
            int drawCount;
            if (DrawCount.start != DrawCount.end)
            {
                drawCount = DrawCount.start + r.Next() % (DrawCount.end - DrawCount.start + 1);
            }
            else drawCount = DrawCount.start;

            //var ll = Entries.Select(v => new {Entry = v, Size = v.GetVirtualSize()}).ToList();
            var powerSum = Entries.Sum(v => v.Power);

            while (drawCount > 0)
            {
                var d = r.Next() % powerSum;
                Entry selected = null;
                for (int i = 0; i < Entries.Count; i++)
                {
                    var entry = Entries[i];
                    d -= entry.Power;
                    if (d < 0)
                    {
                        selected = entry;
                        break;
                    }
                }

                if (selected == null) throw new Exception("unknown");

                var vSize = selected.GetVirtualSize();
                var d2 = r.Next() % vSize;
                var a = selected.GetIndex(d2);
                resultBuffer.Add(a);
                drawCount--;
            }
        }
    }
}