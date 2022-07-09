using System;

namespace BoysheO.Extensions
{
    public static class GuidExtensions
    {
        /// <summary>
        /// 输出Guid值的前几位
        /// </summary>
        public static string ToShortString(Guid guid, int keep = 8)
        {
            return guid.ToString().Substring(0, keep);
        }
    }
}