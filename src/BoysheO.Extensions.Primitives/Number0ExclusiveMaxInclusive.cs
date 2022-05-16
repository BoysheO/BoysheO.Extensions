using System;

namespace BoysheO.Extensions.Primitives
{
    /// <summary>
    /// {v,v∈(0,float.Max],v is float}
    /// </summary>
    public readonly struct Float0ExclusiveMaxInclusive
    {
        public readonly float Value;

        public Float0ExclusiveMaxInclusive(float value)
        {
#if PERFORMANCE
            Value = value;
            return;
#endif
            if (value > 0 && value <= float.MaxValue)
            {
                Value = value;
                return;
            }

            throw new ArgumentOutOfRangeException(nameof(value), $"value={value} not be in [0,float.Max]");
        }
    }
    /// <summary>
    /// {v,v∈(0,double.Max],v is double}
    /// </summary>
    public readonly struct Double0ExclusiveMaxInclusive
    {
        public readonly double Value;

        public Double0ExclusiveMaxInclusive(double value)
        {
#if PERFORMANCE
            Value = value;
            return;
#endif
            if (value > 0 && value <= double.MaxValue)
            {
                Value = value;
                return;
            }

            throw new ArgumentOutOfRangeException(nameof(value), $"value={value} not be in [0,double.Max]");
        }
    }
    /// <summary>
    /// {v,v∈(0,decimal.Max],v is decimal}
    /// </summary>
    public readonly struct Decimal0ExclusiveMaxInclusive
    {
        public readonly decimal Value;

        public Decimal0ExclusiveMaxInclusive(decimal value)
        {
#if PERFORMANCE
            Value = value;
            return;
#endif
            if (value > 0 && value <= decimal.MaxValue)
            {
                Value = value;
                return;
            }

            throw new ArgumentOutOfRangeException(nameof(value), $"value={value} not be in [0,decimal.Max]");
        }
    }
    /// <summary>
    /// {v,v∈(0,sbyte.Max],v is sbyte}
    /// </summary>
    public readonly struct Sbyte0ExclusiveMaxInclusive
    {
        public readonly sbyte Value;

        public Sbyte0ExclusiveMaxInclusive(sbyte value)
        {
#if PERFORMANCE
            Value = value;
            return;
#endif
            if (value > 0 && value <= sbyte.MaxValue)
            {
                Value = value;
                return;
            }

            throw new ArgumentOutOfRangeException(nameof(value), $"value={value} not be in [0,sbyte.Max]");
        }
    }
    /// <summary>
    /// {v,v∈(0,short.Max],v is short}
    /// </summary>
    public readonly struct Short0ExclusiveMaxInclusive
    {
        public readonly short Value;

        public Short0ExclusiveMaxInclusive(short value)
        {
#if PERFORMANCE
            Value = value;
            return;
#endif
            if (value > 0 && value <= short.MaxValue)
            {
                Value = value;
                return;
            }

            throw new ArgumentOutOfRangeException(nameof(value), $"value={value} not be in [0,short.Max]");
        }
    }
    /// <summary>
    /// {v,v∈(0,int.Max],v is int}
    /// </summary>
    public readonly struct Int0ExclusiveMaxInclusive
    {
        public readonly int Value;

        public Int0ExclusiveMaxInclusive(int value)
        {
#if PERFORMANCE
            Value = value;
            return;
#endif
            if (value > 0 && value <= int.MaxValue)
            {
                Value = value;
                return;
            }

            throw new ArgumentOutOfRangeException(nameof(value), $"value={value} not be in [0,int.Max]");
        }
    }
    /// <summary>
    /// {v,v∈(0,long.Max],v is long}
    /// </summary>
    public readonly struct Long0ExclusiveMaxInclusive
    {
        public readonly long Value;

        public Long0ExclusiveMaxInclusive(long value)
        {
#if PERFORMANCE
            Value = value;
            return;
#endif
            if (value > 0 && value <= long.MaxValue)
            {
                Value = value;
                return;
            }

            throw new ArgumentOutOfRangeException(nameof(value), $"value={value} not be in [0,long.Max]");
        }
    }
}