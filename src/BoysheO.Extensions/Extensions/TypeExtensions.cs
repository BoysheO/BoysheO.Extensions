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
        ///     Determine the type is implement the another type (interface or class,GenericTypeDefinition not supported) and is class.<br />
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsClassAndImplement(this Type type, Type base1)
        {
            return type.IsClass && base1 != type && base1.IsAssignableFrom(type);
        }

        //这个函数似乎使用率不够高，以后或许会被移除
        /// <summary>
        ///     Determine the type is implement the another type (interface or class,GenericTypeDefinition not supported) and is struct.<br />
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsStructAndImplement(this Type type, Type base1)
        {
            return type.IsValueType && base1.IsAssignableFrom(type);
        }

        /// <summary>
        ///     Determine the type is implement the another type (interface or class,GenericTypeDefinition not supported) and is sealed.<br />
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsSealedAndImplement(this Type type, Type base1)
        {
            return type.IsSealed && base1.IsAssignableFrom(type);
        }

        /// <summary>
        ///     Determine the type is implement the another type(interface or class,GenericTypeDefinition supported)
        /// </summary>
        public static bool IsImplement(this Type type, Type baseType)
        {
            if (type == baseType) return true;
            if (baseType.IsAssignableFrom(type)) return true;
            if (!baseType.IsGenericTypeDefinition) return false;
            if (baseType.IsInterface)
            {
                var interfaces = type.GetInterfaces();
                for (var i = 0; i < interfaces.Length; i++)
                {
                    var inter = interfaces[i];
                    if (inter.IsGenericType)
                    {
                        var d = inter.GetGenericTypeDefinition();
                        if (d == baseType) return true;
                    }
                }

                return false;
            }
            else
            {
                var b = type.BaseType;
                while (b != null)
                {
                    if (b.IsGenericType)
                    {
                        var d = b.GetGenericTypeDefinition();
                        if (d == baseType) return true;
                    }

                    b = type.BaseType;
                }

                return false;
            }
        }

        //不太普适
        // public static IEnumerable<string> GetAllMembersName(this Type type, MemberTypes flags)
        // {
        //     var members = type.GetMembers();
        //     var targets = members.Where(member => (flags & member.MemberType) == 0).Select(member => member.Name);
        //     return targets;
        // }

        //不太普适
        // public static IEnumerable<string> GetFieldOrPropertyNames(this Type type)
        // {
        //     var members = type.GetMembers();
        //     var targets = members
        //         .Where(member => (MemberTypes.Field | MemberTypes.Property).HasFlag(member.MemberType))
        //         .Select(member => member.Name);
        //     return targets;
        // }

        /// <summary>
        ///     Determine the type is int,double,long .. or other math type.
        ///     No nullable type.
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
        ///     Determine the type is <see cref="Nullable{T}" />
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

        /// <summary>
        /// Get code string<br />
        /// ex.typeof(List&lt;int&gt;) => "List&lt;int&gt;"
        /// </summary>
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