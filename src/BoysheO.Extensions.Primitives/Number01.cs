using System;

namespace BoysheO.Extensions.Primitives
{
    /// <summary>
    /// {v,v∈[0,1],v is float}
    /// </summary>
    public readonly struct Float01
    {
        public readonly float Value;

        public Float01(float value)
        {
#if PERFORMANCE
            Value = value;
            return;
#endif
            if (value >= 0 && value <= 1)//float有可能是NaN值，不可用<0和>1判断
            {
                Value = value;
                return;
            }

            throw new ArgumentOutOfRangeException(nameof(value), $"value={value} not be in [0,1]");
        }
    }
    /// <summary>
    /// {v,v∈[0,1],v is double}
    /// </summary>
    public readonly struct Double01
    {
        public readonly double Value;

        public Double01(double value)
        {
#if PERFORMANCE
            Value = value;
            return;
#endif
            if (value >= 0 && value <= 1)//float有可能是NaN值，不可用<0和>1判断
            {
                Value = value;
                return;
            }

            throw new ArgumentOutOfRangeException(nameof(value), $"value={value} not be in [0,1]");
        }
    }
    /// <summary>
    /// {v,v∈[0,1],v is decimal}
    /// </summary>
    public readonly struct Decimal01
    {
        public readonly decimal Value;

        public Decimal01(decimal value)
        {
#if PERFORMANCE
            Value = value;
            return;
#endif
            if (value >= 0 && value <= 1)//float有可能是NaN值，不可用<0和>1判断
            {
                Value = value;
                return;
            }

            throw new ArgumentOutOfRangeException(nameof(value), $"value={value} not be in [0,1]");
        }
    }
}