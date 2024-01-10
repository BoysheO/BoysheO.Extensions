using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BoysheO.Extensions;

namespace DrawPoolStringSystem
{
    public class DrawPoolString
    {
        public class RewardPool
        {
            public int Power;
            public int VirtualSize;
            public readonly List<(int start, int end)> Ranged = new List<(int start, int end)>();

            public int GetValue(int virtualIdx)
            {
                foreach (var (start, end) in Ranged)
                {
                    for (int i = start; i <= end; i++)
                    {
                        if (virtualIdx <= 0) return i;
                        virtualIdx--;
                    }
                }

                throw new ArgumentOutOfRangeException(nameof(virtualIdx)); //越界
            }
        }

        private readonly List<RewardPool> _pool = new List<RewardPool>();
        private int _sumPower;

        public static DrawPoolString Creat(string drawPoolString)
        {
            var ins = new DrawPoolString();
            var entries = drawPoolString.Split(';');
            foreach (var entry in entries)
            {
                var idx = entry.IndexOf('[');
                var powerStr = entry.Substring(0, idx);
                var power = int.Parse(powerStr);
                if (power <= 0) throw new Exception($"invalid power={power}");
                var rewards = new RewardPool()
                {
                    Power = power
                };
                ins._pool.Add(rewards);
                var poolStr = entry.Substring(idx + 1, entry.Length - idx - 2);
                var values = poolStr.Split(',');
                foreach (var value in values)
                {
                    if (int.TryParse(value, out var iv))
                    {
                        if (iv < 0) throw new Exception($"negative reject.v={iv}");
                        rewards.Ranged.Add((iv, iv));
                        rewards.VirtualSize++;
                    }
                    else
                    {
                        var ranged = value.Split('~');
                        var start = int.Parse(ranged[0]);
                        var end = int.Parse(ranged[1]);
                        if (start < 0) throw new Exception($"negative reject.v={start}");
                        if (end < 0) throw new Exception($"negative reject.v={end}");
                        if (start > end) (start, end) = (end, start);
                        rewards.Ranged.Add((start, end));
                        rewards.VirtualSize += end - start + 1;
                    }
                }
            }

            ins.UpdateSumPower();

            Console.WriteLine(ins._pool.Select(v => new
            {
                v.Power,
                Range = v.Ranged.Select(v2 => v2.ToString()).JoinAsOneString(),
                v.VirtualSize,
            }.ToString()).JoinAsOneString());
            Console.WriteLine(ins._sumPower);

            return ins;
        }

        public void AddRewardPool(int power, IReadOnlyList<(int start, int end)> ranges)
        {
            if (power <= 0) throw new ArgumentOutOfRangeException(nameof(power));
            if (ranges.Count == 0) throw new ArgumentOutOfRangeException(nameof(ranges));
            var rewards = new RewardPool()
            {
                Power = power,
            };

            for (var index = 0; index < ranges.Count; index++)
            {
                var (start, end) = ranges[index];
                if (start < 0) throw new Exception($"negative reject.v={start}");
                if (end < 0) throw new Exception($"negative reject.v={end}");
                if (start > end) (start, end) = (end, start);
                rewards.Ranged.Add((start, end));
                rewards.VirtualSize += end - start + 1;
            }

            _pool.Add(rewards);
            _sumPower = checked(_sumPower + power);
        }

        public int Draw(int randNum1, int randNum2)
        {
            if (_pool.Count == 0) throw new Exception("empty pool");
            var power = randNum1 % _sumPower + 1;
            for (int i = 0; i < _pool.Count; i++)
            {
                var pool = _pool[i];
                power -= pool.Power;
                if (power > 0) continue;
                var ii = randNum2 % pool.VirtualSize;
                return pool.GetValue(ii);
            }

            throw new Exception("invalid sumPower");
        }

        private void UpdateSumPower()
        {
            _sumPower = _pool.Sum(v => v.Power);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            for (var index = 0; index < _pool.Count; index++)
            {
                var rewards = _pool[index];
                sb.Append(rewards.Power);
                sb.Append('[');
                for (var index1 = 0; index1 < rewards.Ranged.Count; index1++)
                {
                    var (start, end) = rewards.Ranged[index1];
                    if (start == end)
                    {
                        sb.Append(start);
                    }
                    else
                    {
                        sb.Append(start);
                        sb.Append('~');
                        sb.Append(end);
                    }

                    if (index1 != rewards.Ranged.Count - 1) sb.Append(',');
                }

                sb.Append(']');
                if (index != _pool.Count - 1) sb.Append(';');
            }

            return sb.ToString();
        }
    }
}