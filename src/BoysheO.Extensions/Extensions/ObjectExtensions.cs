using System;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace BoysheO.Extensions
{
    public static class ObjectExtensions
    {
        /// <summary>
        ///     将单个元素包装成拥有1个元素的数组
        /// </summary>
        public static T[] WarpToArray<T>(this T obj)
        {
            if (obj is null) throw new ArgumentNullException(nameof(obj), "null element is rejected"); //阻止null元素
            return new[] {obj};
        }

        /// <summary>
        ///     反射获取<see cref="DescriptionAttribute.Description" />
        /// </summary>
        /// <param name="obj">拥有<see cref="DescriptionAttribute.Description" />属性的成员的宿主对象</param>
        /// <param name="memberName">拥有<see cref="DescriptionAttribute.Description" />属性的成员签名</param>
        public static string? GetDescription(this object obj, string memberName)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            if (memberName.IsNullOrEmpty()) throw new ArgumentNullException(nameof(memberName));
            if (obj is Enum @enum) return @enum.GetDescription();
            var type = obj.GetType();
            var field = type.GetField(memberName);
            if (field != null)
            {
                var atr = field.GetCustomAttribute<DescriptionAttribute>();
                return atr?.Description;
            }

            var property = type.GetField(memberName);
            // ReSharper disable once InvertIf
            if (property != null)
            {
                var atr = property.GetCustomAttribute<DescriptionAttribute>();
                return atr?.Description;
            }

            return null;
        }

        /// <summary>
        ///     反射获取<see cref="DescriptionAttribute.Description" />，根据Enum获取
        /// </summary>
        public static string? GetDescription(this Enum @enum)
        {
            var type = @enum.GetType();
            var field = type.GetField(@enum.ToString());
            var atr = field.GetCustomAttribute<DescriptionAttribute>();
            return atr?.Description;
        }

        /// <summary>
        ///     等价于(T)obj强制类型转换。减少强转语法对代码可读性伤害
        ///     不赶时间的话使用（T）更好，可以获得IDE支持，提前发现类型转换错误
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T CastType<T>(this object obj)
        {
            return (T) obj;
        }
    }
}