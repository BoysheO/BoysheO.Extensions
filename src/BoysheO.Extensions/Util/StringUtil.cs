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
            TB = 4
        }

        /// <summary>
        /// 1023=>1023b 1024=>1k 1048576=>1m
        /// </summary>
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
    }
}