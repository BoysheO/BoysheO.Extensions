using System;
using System.Runtime.CompilerServices;

namespace BoysheO.Extensions
{
    public static class EnumExtensions
    {
        /// <summary>
        /// Converts an enum to an int in a straightforward manner.
        /// *Note: If the enum's underlying type exceeds the range of int (e.g., long), 
        /// the result will be incorrect and no warning will be given.
        /// This API is designed based on the expectation that it will be heavily used, 
        /// while enums with long or ulong as their underlying types are rare. 
        /// This consideration led to the implementation of this API.<br/>
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