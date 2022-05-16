using System;

namespace BoysheO.Extensions.Primitives
{
    /// <summary>
    /// {v,v∈[0,float.Max],v is float}
    /// </summary>
    public readonly struct Float0Max
    {
        public readonly float Value;

        public Float0Max(float value)
        {
#if PERFORMANCE
            Value = value;
            return;
#endif
            if (value >= 0 && value <= float.MaxValue)
            {
                Value = value;
                return;
            }

            throw new ArgumentOutOfRangeException(nameof(value), $"value={value} not be in [0,float.Max]");
        }
    }
    /// <summary>
    /// {v,v∈[0,double.Max],v is double}
    /// </summary>
    public readonly struct Double0Max
    {
        public readonly double Value;

        public Double0Max(double value)
        {
#if PERFORMANCE
            Value = value;
            return;
#endif
            if (value >= 0 && value <= double.MaxValue)
            {
                Value = value;
                return;
            }

            throw new ArgumentOutOfRangeException(nameof(value), $"value={value} not be in [0,double.Max]");
        }
    }
    /// <summary>
    /// {v,v∈[0,decimal.Max],v is decimal}
    /// </summary>
    public readonly struct Decimal0Max
    {
        public readonly decimal Value;

        public Decimal0Max(decimal value)
        {
#if PERFORMANCE
            Value = value;
            return;
#endif
            if (value >= 0 && value <= decimal.MaxValue)
            {
                Value = value;
                return;
            }

            throw new ArgumentOutOfRangeException(nameof(value), $"value={value} not be in [0,decimal.Max]");
        }
    }
    /// <summary>
    /// {v,v∈[0,sbyte.Max],v is sbyte}
    /// </summary>
    public readonly struct Sbyte0Max
    {
        public readonly sbyte Value;

        public Sbyte0Max(sbyte value)
        {
#if PERFORMANCE
            Value = value;
            return;
#endif
            if (value >= 0 && value <= sbyte.MaxValue)
            {
                Value = value;
                return;
            }

            throw new ArgumentOutOfRangeException(nameof(value), $"value={value} not be in [0,sbyte.Max]");
        }
    }
    /// <summary>
    /// {v,v∈[0,short.Max],v is short}
    /// </summary>
    public readonly struct Short0Max
    {
        public readonly short Value;

        public Short0Max(short value)
        {
#if PERFORMANCE
            Value = value;
            return;
#endif
            if (value >= 0 && value <= short.MaxValue)
            {
                Value = value;
                return;
            }

            throw new ArgumentOutOfRangeException(nameof(value), $"value={value} not be in [0,short.Max]");
        }
    }
    /// <summary>
    /// {v,v∈[0,int.Max],v is int}
    /// </summary>
    public readonly struct Int0Max
    {
        public readonly int Value;

        public Int0Max(int value)
        {
#if PERFORMANCE
            Value = value;
            return;
#endif
            if (value >= 0 && value <= int.MaxValue)
            {
                Value = value;
                return;
            }

            throw new ArgumentOutOfRangeException(nameof(value), $"value={value} not be in [0,int.Max]");
        }
    }
    /// <summary>
    /// {v,v∈[0,long.Max],v is long}
    /// </summary>
    public readonly struct Long0Max
    {
        public readonly long Value;

        public Long0Max(long value)
        {
#if PERFORMANCE
            Value = value;
            return;
#endif
            if (value >= 0 && value <= long.MaxValue)
            {
                Value = value;
                return;
            }

            throw new ArgumentOutOfRangeException(nameof(value), $"value={value} not be in [0,long.Max]");
        }
    }

}