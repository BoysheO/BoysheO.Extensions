using System;
using System.Runtime.CompilerServices;

namespace BoysheO.Extensions
{
    public static class EnumExtensions
    {
        /// <summary>
        /// 简易枚举转int。
        /// *如果具体的枚举实现范围超过int，例如long，那么结果将会错误且没有提示
        /// 可以预料这个API会被大量使用，而long、ulong作为枚举值的情形少之又少，基于此考量编写了此API
        /// </summary>
        public static int AsInt<T>(this T value) where T : Enum
        {
            var i = Unsafe.As<T, int>(ref value);
            return i;
        }
    }
}