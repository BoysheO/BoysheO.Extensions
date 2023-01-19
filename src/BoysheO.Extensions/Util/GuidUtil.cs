using System;

namespace BoysheO.Util
{
    public class GuidUtil
    {
        /// <summary>
        /// 输出Guid值的前几位,常用于调试打印
        /// </summary>
        public static string ToShortString(Guid guid, int keep = 8)
        {
            return guid.ToString().Substring(0, keep);
        }
    }
}