//code from https://pastebin.com/pxTvHJWL and refactor by boysheo

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace BoysheO.Toolkit
{
    /// <summary>
    /// Thread not safe
    /// Float and double maybe perform in different CPU.
    /// 性能会比原生的弱点，因为原生的有针对平台优化。浮点数计算随CPU架构，所以使用时要留意
    /// </summary>
    public sealed class SerializableRandom
    {
        public static int GenSeedDefault() => (int)DateTimeOffset.Now.ToUnixTimeMilliseconds();
        
        public SerializableRandom()
            // Used to generate a varying seed, regardless of close-frequency allocation
            : this(GenSeedDefault())
        {
        }

        public SerializableRandom(int[] saveState)
        {
            LoadState(saveState);
        }

        public SerializableRandom(int seed)
        {
            Seed = seed;
        }

        /// <summary>
        /// clone
        /// </summary>
        public SerializableRandom(SerializableRandom source)
        {
            _seed = source._seed;
            _inext = source._inext;
            _inextp = source._inextp;
            _seedArray = source._seedArray.ToArray();
        }

        private int _seed { get; set; }
        private int _inext { get; set; }
        private int _inextp { get; set; }
        private int[] _seedArray { get; set; } = new int[56];

        /// <summary>
        ///     The current seed of this instance.
        /// </summary>
        private int Seed
        {
            get => _seed;
            set
            {
                _seed = value;
                var subtraction = _seed == int.MinValue ? int.MaxValue : Math.Abs(_seed);
                var mj = 0x9a4ec86 - subtraction;
                _seedArray[0x37] = mj;
                var mk = 1;
                for (var i = 1; i < 0x37; i++)
                {
                    var ii = 0x15 * i % 0x37;
                    _seedArray[ii] = mk;
                    mk = mj - mk;
                    if (mk < 0x0) mk += int.MaxValue;

                    mj = _seedArray[ii];
                }

                for (var k = 1; k < 0x5; k++)
                for (var i = 1; i < 0x38; i++)
                {
                    _seedArray[i] -= _seedArray[1 + (i + 0x1e) % 0x37];
                    if (_seedArray[i] < 0) _seedArray[i] += int.MaxValue;
                }

                _inext = 0;
                _inextp = 21;
            }
        }

        /// <summary>
        ///     Resets this instance using it's current seed.
        ///     This means that the RNG will start over again,
        ///     repeating the same values that it originally had.
        /// </summary>
        public void Reset()
        {
            Reseed(Seed);
        }

        /// <summary>
        ///     Reseeds this instance using a new GUID Hashcode
        /// </summary>
        public void Reseed()
        {
            Reseed(GenSeedDefault());
        }

        /// <summary>
        ///     Reseeds this instance using a given integer seed.
        /// </summary>
        /// <param name="seed"></param>
        public void Reseed(int seed)
        {
            Seed = seed;
        }

        public int NextInteger()
        {
            return NextSample();
        }

        public void NextIntegers(Span<int> buffer)
        {
            for (var i = 0; i < buffer.Length; i++) buffer[i] = NextInteger();
        }

        public int[] NextIntegers(int quantity)
        {
            var buffer = new int[quantity];
            NextIntegers(buffer);
            return buffer;
        }

        public int NextInteger(int minInclusive, int maxExclusive)
        {
            if (minInclusive >= maxExclusive)
                throw new ArgumentOutOfRangeException(nameof(minInclusive), "minValue must be less than maxValue");
            return NoCheckNextInteger(minInclusive,maxExclusive);
        }
        
        private int NoCheckNextInteger(int minInclusive, int maxExclusive)
        {
            return (int) (NextSample() * (1.0D / int.MaxValue) * (maxExclusive - minInclusive)) + minInclusive;
        }

        public void NextIntegers(int minInclusive, int maxExclusive, Span<int> buffer)
        {
            if (minInclusive >= maxExclusive)
                throw new ArgumentOutOfRangeException(nameof(minInclusive), "minValue must be less than maxValue");
            for (var i = 0; i < buffer.Length; i++) buffer[i] = NoCheckNextInteger(minInclusive, maxExclusive);
        }

        public int[] NextIntegers(int minInclusive, int maxExclusive, int quantity)
        {
            if (minInclusive >= maxExclusive)
                throw new ArgumentOutOfRangeException(nameof(minInclusive), "minValue must be less than maxValue");
            var buffer = new int[quantity];
            NextIntegers(minInclusive, maxExclusive, buffer);
            return buffer;
        }

        /// <summary>
        ///     不含最大值
        /// </summary>
        /// <param name="maxExclusive"></param>
        /// <returns></returns>
        public int NextInteger(int maxExclusive)
        {
            return NextInteger(0, maxExclusive);
        }

        public void NextIntegers(int maxExclusive, int[] buffer)
        {
            for (var i = 0; i < buffer.Length; i++) buffer[i] = NextInteger(maxExclusive);
        }

        public int[] NextIntegers(int maxExclusive, int quantity)
        {
            var buffer = new int[quantity];
            NextIntegers(maxExclusive, buffer);
            return buffer;
        }

        /// <summary>
        /// Notice:Not perform same in different CPU
        /// </summary>
        public double NextDouble()
        {
            return NextSample() * (1.0D / int.MaxValue);
        }

        /// <summary>
        /// Notice:Not perform same in different CPU
        /// </summary>
        public void NextDoubles(double[] buffer)
        {
            for (var i = 0; i < buffer.Length; i++) buffer[i] = NextDouble();
        }

        /// <summary>
        /// Notice:Not perform same in different CPU
        /// </summary>
        public double[] NextDoubles(int quantity)
        {
            var buffer = new double[quantity];
            NextDoubles(buffer);
            return buffer;
        }

        /// <summary>
        /// Notice:Not perform same in different CPU
        /// </summary>
        public double NextDouble(double minInclusive, double maxInclusive)
        {
            return NextDouble() * (maxInclusive - minInclusive) + minInclusive;
        }

        /// <summary>
        /// Notice:Not perform same in different CPU
        /// </summary>
        public void NextDoubles(double minValue, double maxValue, double[] buffer)
        {
            for (var i = 0; i < buffer.Length; i++) buffer[i] = NextDouble(minValue, maxValue);
        }

        /// <summary>
        /// Notice:Not perform same in different CPU
        /// </summary>
        public double[] NextDoubles(double minValue, double maxValue, int quantity)
        {
            var buffer = new double[quantity];
            NextDoubles(minValue, maxValue, buffer);
            return buffer;
        }

        /// <summary>
        /// Notice:Not perform same in different CPU
        /// </summary>
        public float NextFloat()
        {
            return (float) (NextSample() * (1.0D / int.MaxValue));
        }

        /// <summary>
        /// Notice:Not perform same in different CPU
        /// </summary>
        public void NextFloats(float[] buffer)
        {
            for (var i = 0; i < buffer.Length; i++) buffer[i] = NextFloat();
        }

        /// <summary>
        /// Notice:Not perform same in different CPU
        /// </summary>
        public float[] NextFloats(int quantity)
        {
            var buffer = new float[quantity];
            NextFloats(buffer);
            return buffer;
        }

        /// <summary>
        /// Notice:Not perform same in different CPU
        /// </summary>
        public float NextFloat(float minValue, float maxValue)
        {
            return NextFloat() * (maxValue - minValue) + minValue;
        }

        /// <summary>
        /// Notice:Not perform same in different CPU
        /// </summary>
        public void NextFloats(float minValue, float maxValue, float[] buffer)
        {
            for (var i = 0; i < buffer.Length; i++) buffer[i] = NextFloat(minValue, maxValue);
        }

        /// <summary>
        /// Notice:Not perform same in different CPU
        /// </summary>
        public float[] NextFloats(float minValue, float maxValue, int quantity)
        {
            var buffer = new float[quantity];
            NextFloats(minValue, maxValue, buffer);
            return buffer;
        }

        // public int NextRange(Range range)
        // {
        //     return NextInteger(range.Start.Value, range.End.Value + 1);
        // }
        //
        // public void NextRanges(Range range, int[] buffer)
        // {
        //     for (var i = 0; i < buffer.Length; i++) buffer[i] = NextRange(range);
        // }
        //
        // public int[] NextRanges(Range range, int quantity)
        // {
        //     var buffer = new int[quantity];
        //     NextRanges(range, buffer);
        //     return buffer;
        // }

        // public int NextRange(int minValue, int maxValue)
        // {
        //     return NextInteger(minValue, maxValue + 1);
        // }

        // public void NextRanges(int minValue, int maxValue, int[] buffer)
        // {
        //     for (var i = 0; i < buffer.Length; i++) buffer[i] = NextRange(minValue, maxValue);
        // }
        //
        // public int[] NextRanges(int minValue, int maxValue, int quantity)
        // {
        //     var buffer = new int[quantity];
        //     NextRanges(minValue, maxValue, buffer);
        //     return buffer;
        // }

        // public byte NextByte()
        // {
        //     return (byte) NextInteger(0, 256);
        // }
        //
        // public void NextBytes(Span<byte> buffer)
        // {
        //     for (var i = 0; i < buffer.Length; i++) buffer[i] = NextByte();
        // }
        //
        // public byte[] NextBytes(int quantity)
        // {
        //     var buffer = new byte[quantity];
        //     NextBytes(buffer);
        //     return buffer;
        // }
        //
        // public byte NextByte(byte minValue, byte maxValue)
        // {
        //     return (byte) NextInteger(minValue, maxValue);
        // }
        //
        // public void NextBytes(byte minValue, byte maxValue, Span<byte> buffer)
        // {
        //     for (var i = 0; i < buffer.Length; i++) buffer[i] = NextByte(minValue, maxValue);
        // }
        //
        // public byte[] NextBytes(byte minValue, byte maxValue, int quantity)
        // {
        //     var buffer = new byte[quantity];
        //     NextBytes(minValue, maxValue, buffer);
        //     return buffer;
        // }
        //
        // public byte NextByte(byte maxValue)
        // {
        //     return (byte) NextInteger(0, maxValue);
        // }
        //
        // public void NextBytes(byte maxValue, Span<byte> buffer)
        // {
        //     for (var i = 0; i < buffer.Length; i++) buffer[i] = NextByte(maxValue);
        // }
        //
        // public byte[] NextBytes(byte maxValue, int quantity)
        // {
        //     var buffer = new byte[quantity];
        //     NextBytes(maxValue, buffer);
        //     return buffer;
        // }

        // public bool NextBool()
        // {
        //     return NextInteger(0, 2) == 1;
        // }
        //
        // public void NextBools(bool[] buffer)
        // {
        //     for (var i = 0; i < buffer.Length; i++) buffer[i] = NextBool();
        // }
        //
        // public bool[] NextBools(int quantity)
        // {
        //     var buffer = new bool[quantity];
        //     NextBools(buffer);
        //     return buffer;
        // }

        // public string NextString(char[] possibleCharacters, int length)
        // {
        //     var buffer = new char[length];
        //     for (var i = 0; i < buffer.Length; i++)
        //         buffer[i] = possibleCharacters[NextInteger(0, possibleCharacters.Length)];
        //
        //     return new string(buffer);
        // }
        //
        // public void NextStrings(char[] possibleCharacters, int length, string[] buffer)
        // {
        //     for (var i = 0; i < buffer.Length; i++) buffer[i] = NextString(possibleCharacters, length);
        // }
        //
        // public string[] NextStrings(char[] possibleCharacters, int length, int quantity)
        // {
        //     var buffer = new string[quantity];
        //     NextStrings(possibleCharacters, length, buffer);
        //     return buffer;
        // }

        // public void Shuffle<T>(List<T> list)
        // {
        //     var n = list.Count;
        //     while (n > 1)
        //     {
        //         n--;
        //         var k = NextInteger(0, n + 1);
        //         var value = list[k];
        //         list[k] = list[n];
        //         list[n] = value;
        //     }
        // }
        //
        // public void Shuffle<T>(T[] list)
        // {
        //     var n = list.Length;
        //     while (n > 1)
        //     {
        //         n--;
        //         var k = NextInteger(0, n + 1);
        //         var value = list[k];
        //         list[k] = list[n];
        //         list[n] = value;
        //     }
        // }

        // public int NextIndex<T>(T[] array)
        // {
        //     return NextInteger(array.Length);
        // }
        //
        // public int NextIndex<T>(List<T> list)
        // {
        //     return NextInteger(list.Count);
        // }
        
        // public T Choose<T>(T[] items)
        // {
        //     return items[NextInteger(items.Length)];
        // }
        //
        // public void Choose<T>(T[] items, T[] resultBuffer)
        // {
        //     for (var i = 0; i < resultBuffer.Length; i++) resultBuffer[i] = Choose(items);
        // }
        //
        // public T[] Choose<T>(T[] items, int quantity)
        // {
        //     var buffer = new T[quantity];
        //     Choose(items, buffer);
        //     return buffer;
        // }
        //
        // public T Choose<T>(List<T> items)
        // {
        //     return items[NextInteger(0, items.Count)];
        // }
        //
        // public List<T> Choose<T>(List<T> items, int quantity)
        // {
        //     var buffer = new List<T>(quantity);
        //     for (var i = 0; i < quantity; i++) buffer.Add(Choose(items));
        //
        //     return buffer;
        // }

        // public bool NextProbability(float percent)
        // {
        //     if (percent >= 1.0f) return true;
        //     if (percent <= 0.0f) return false;
        //     return NextDouble() <= percent;
        // }
        //
        // public void NextProbability(float percent, bool[] buffer)
        // {
        //     for (var i = 0; i < buffer.Length; i++) buffer[i] = NextProbability(percent);
        // }
        //
        // public bool[] NextProbabilities(float percent, int quantity)
        // {
        //     var buffer = new bool[quantity];
        //     NextProbability(percent, buffer);
        //     return buffer;
        // }
        //
        // public bool NextProbability(int percent)
        // {
        //     if (percent >= 100) return true;
        //     if (percent <= 0) return false;
        //     return NextInteger(101) <= percent;
        // }
        //
        // public void NextProbabilities(int percent, bool[] buffer)
        // {
        //     for (var i = 0; i < buffer.Length; i++) buffer[i] = NextProbability(percent);
        // }
        //
        // public bool[] NextProbabilities(int percent, int quantity)
        // {
        //     var buffer = new bool[quantity];
        //     NextProbabilities(percent, buffer);
        //     return buffer;
        // }

        // public bool NextOdds(int a, int b)
        // {
        //     return NextProbability((float) a / b);
        // }
        //
        // public void NextOdds(int a, int b, bool[] buffer)
        // {
        //     for (var i = 0; i < buffer.Length; i++) buffer[i] = NextOdds(a, b);
        // }
        //
        // public bool[] NextOdds(int a, int b, int quantity)
        // {
        //     var buffer = new bool[quantity];
        //     NextOdds(a, b, buffer);
        //     return buffer;
        // }

        // public Vector3 NextVector3(float xMin, float xMax, float yMin, float yMax, float zMin, float zMax)
        // {
        //     return new Vector3(NextFloat(xMin, xMax), NextFloat(yMin, yMax), NextFloat(zMin, zMax));
        // }
        //
        // public Vector3 NextVector3(Vector3 min, Vector3 max)
        // {
        //     return NextVector3(min.X, max.X, min.Y, max.Y, min.Z, max.Z);
        // }
        //
        // public Vector3 NextVector3(float min, float max)
        // {
        //     return NextVector3(min, max, min, max, min, max);
        // }

        private int NextSample()
        {
            var locINext = _inext;
            var locINextp = _inextp;
            if (++locINext >= 56) locINext = 1;

            if (++locINextp >= 56) locINextp = 1;

            var retVal = _seedArray[locINext] - _seedArray[locINextp];
            if (retVal == int.MaxValue) retVal--;

            if (retVal < 0) retVal += int.MaxValue;

            _seedArray[locINext] = retVal;
            _inext = locINext;
            _inextp = locINextp;
            return retVal;
        }

        public int[] GetState()
        {
            var state = new int[59];
            state[0] = _seed;
            state[1] = _inext;
            state[2] = _inextp;
            for (var i = 3; i < _seedArray.Length; i++) state[i] = _seedArray[i - 3];

            return state;
        }

        public void LoadState(int[] saveState)
        {
            if (saveState.Length != 59) throw new Exception("saveState was corrupted!");

            _seed = saveState[0];
            _inext = saveState[1];
            _inextp = saveState[2];
            _seedArray = new int[59];
            for (var i = 3; i < _seedArray.Length; i++) _seedArray[i - 3] = saveState[i];
        }
    }
}