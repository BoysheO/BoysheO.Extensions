#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace BoysheO.Extensions
{
    public static class TypeExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object? CreatInstance(this Type type)
        {
            try
            {
                return Activator.CreateInstance(type);
            }
            catch
            {
                return null;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T? CreatInstance<T>(this Type type) where T : class
        {
            return (T?) CreatInstance(type);
        }

        /// <summary>
        ///     检查该类型是否继承目标接口、类
        ///     <br>自动排除自身和要求被检查类是Class</br>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsClassAndImplement(this Type type, Type base1)
        {
            return type.IsClass && base1 != type && base1.IsAssignableFrom(type);
        }

        /// <summary>
        ///     检查该struct类型是否继承目标接口
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsStructAndImplement(this Type type, Type base1)
        {
            return type.IsValueType && base1.IsAssignableFrom(type);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsSealedAndImplement(this Type type, Type base1)
        {
            return type.IsSealed && base1.IsAssignableFrom(type);
        }

        public static IEnumerable<string> GetAllMembersName(this Type type, MemberTypes flags)
        {
            var members = type.GetMembers();
            var targets = members.Where(member => (flags & member.MemberType) == 0).Select(member => member.Name);
            return targets;
        }

        public static IEnumerable<string> GetFieldOrPropertyNames(this Type type)
        {
            var members = type.GetMembers();
            var targets = members
                .Where(member => (MemberTypes.Field | MemberTypes.Property).HasFlag(member.MemberType))
                .Select(member => member.Name);
            return targets;
        }

        /// <summary>
        ///     判断是否数值类型
        /// </summary>
        public static bool IsNumericType(this Type type)
        {
            return type == typeof(int)
                   || type == typeof(double)
                   || type == typeof(long)
                   || type == typeof(short)
                   || type == typeof(float)
                   || type == typeof(short)
                   || type == typeof(int)
                   || type == typeof(long)
                   || type == typeof(uint)
                   || type == typeof(ushort)
                   || type == typeof(uint)
                   || type == typeof(ulong)
                   || type == typeof(sbyte)
                   || type == typeof(float)
                   || type == typeof(decimal);
        }

        /// <summary>
        ///     判断类型是否为<see cref="Nullable{T}" />类型
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNullableType(this Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        private static string GenTypeCode(Type type)
        {
            if (type.IsArray)
            {
                return GetTypeCode(type.GetElementType()!) + (type.GetArrayRank() == 1 ? "[]" : "[,]");
            }

            if (type.IsInheritsFrom(typeof(Nullable<>)))
                return GetTypeCode(type.GetGenericArguments()[0]) + "?";
            if (type.IsByRef)
                return "ref " + GetTypeCode(type.GetElementType()!);
            if (type.IsGenericParameter || !type.IsGenericType)
                return type.Name;
            StringBuilder stringBuilder = new StringBuilder();
            string name = type.Name;
            int length = name.IndexOf("`", StringComparison.Ordinal);
            if (length != -1) stringBuilder.Append(name.Substring(0, length));
            else stringBuilder.Append(name);
            stringBuilder.Append('<');
            Type[] genericArguments = type.GetGenericArguments();
            for (int index = 0; index < genericArguments.Length; ++index)
            {
                Type type1 = genericArguments[index];
                if (index != 0) stringBuilder.Append(",");
                stringBuilder.Append(GetTypeCode(type1));
            }

            stringBuilder.Append('>');
            return stringBuilder.ToString();
        }

        public static string GetTypeCode(this Type type)
        {
            return type.IsNested && !type.IsGenericParameter
                ? GetTypeCode(type.DeclaringType!) + "." + GenTypeCode(type)
                : GenTypeCode(type);
        }

        public static bool IsInheritsFrom(this Type type, Type baseType)
        {
            if (baseType.IsAssignableFrom(type)) return true;
            if (type.IsInterface && !baseType.IsInterface) return false;
            if (baseType.IsInterface) return type.GetInterfaces().Contains(baseType);
            for (Type? type1 = type; type1 != null; type1 = type1.BaseType)
            {
                if (type1 == baseType || baseType.IsGenericTypeDefinition && type1.IsGenericType &&
                    type1.GetGenericTypeDefinition() == baseType)
                    return true;
            }

            return false;
        }
    }
}