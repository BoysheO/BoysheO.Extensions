using System;

namespace BoysheO.Util
{
    public class StringUtil
    {
        public enum CapacityUnit
        {
            B = 0,
            KB = 1,
            MB = 2,
            GB = 3,
            TB = 4,
            PB = 5,
            EB = 6,
            ZB = 7
        }

        /// <summary>
        /// 1023=>1023b 1024=>1k 1048576=>1m
        /// </summary>
        [Obsolete("use BytesToReadableSize(ulong) instead")]
        public static (double, CapacityUnit) BytesToReadableSize(long bytesCount)
        {
            int order = 0;
            double adjustedSize = bytesCount;

            while (adjustedSize >= 1024)
            {
                order++;
                adjustedSize /= 1024;
            }

            return (adjustedSize, (CapacityUnit) order);
        }
        
        /// <summary>
        /// Converts a byte count into a more readable size format.
        /// Example: 1023 => 1023b, 1024 => 1k, 1048576 => 1m.
        /// Notice:The size returned may not be precise
        /// </summary>
        public static (double size, CapacityUnit unit) BytesToReadableSize(ulong bytesCount)
        {
            if (bytesCount == 0) return (0, CapacityUnit.B);
            int order = (int)Math.Floor(Math.Log(bytesCount, 1024));
            double adjustedSize = bytesCount / Math.Pow(1024, order);
            return (adjustedSize, (CapacityUnit)order);
        }
    }
}