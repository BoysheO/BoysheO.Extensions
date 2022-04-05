using System;

namespace BoysheO.Toolkit
{
    public class SynchronizeRandom : Random
    {
        private readonly object _gate = new object();

        public override int Next(int maxValue)
        {
            lock (_gate)
            {
                return base.Next(maxValue);
            }
        }

        public override int Next()
        {
            lock (_gate)
            {
                return base.Next();
            }
        }

        /// <summary>
        ///     value in [min,max)
        /// </summary>
        public override int Next(int minValue, int maxValue)
        {
            lock (_gate)
            {
                return base.Next(minValue, maxValue);
            }
        }

        public override void NextBytes(byte[] buffer)
        {
            lock (_gate)
            {
                base.NextBytes(buffer);
            }
        }

        public override double NextDouble()
        {
            lock (_gate)
            {
                return base.NextDouble();
            }
        }
    }
}