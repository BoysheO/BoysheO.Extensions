namespace BoysheO.Util
{
    public static class IntUtil
    {
        /// <summary>
        /// 输出形如+1、-1，+0这类带符号的;其中0值的显示取决于zero参数<br />
        /// </summary>
        /// <param name="value"></param>
        /// <param name="zero">当value为0时，输出这个值。建议值为"0"、"+0"、"-0"、" 0"之一</param>
        /// <returns></returns>
        public static string ToStringWithSign(int value, string zero = "0")
        {
            if (value == 0) return zero;
            return value > 0 ? string.Concat("+", value.ToString()) : value.ToString();
        }
    }
}