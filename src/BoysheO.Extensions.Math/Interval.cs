using System;
using System.Text;

namespace BoysheO.Extensions.Math
{
    /// <summary>
    /// 区间
    /// </summary>
    public readonly struct Interval
    {
        /// <summary>
        /// 可达性
        /// </summary>
        [Flags]
        public enum AccessibilityType
        {
            None = 0,
            Min,
            Max,
        }

        public readonly int Min;
        public readonly int Max;
        public readonly AccessibilityType Accessibility;

        public override string ToString()
        {
            return $"{(IsMinAccessible?'[':'(')}{Min},{Max}{(IsMaxAccessible?']':')')}";
        }

        public bool IsMinAccessible => (Accessibility & AccessibilityType.Min) == AccessibilityType.Min;
        public bool IsMaxAccessible => (Accessibility & AccessibilityType.Max) == AccessibilityType.Max;

        public Interval(int min, int max, AccessibilityType accessibility)
        {
            Min = min;
            Max = max;
            Accessibility = accessibility;
        }
    }
}