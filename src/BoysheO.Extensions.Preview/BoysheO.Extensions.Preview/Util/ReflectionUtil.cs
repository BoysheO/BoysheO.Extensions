using System;
using System.Reflection;
using BoysheO.Extensions;

namespace BoysheO.Util
{
    public static class ReflectionUtil
    {
        public static bool TryGetFieldOrProperty<T>(object ins, string name, out T res)
        {
            var type = ins.GetType();
            var field = type.GetRuntimeField(name);
            if (field != null)
            {
                if (!typeof(T).IsAssignableFrom(field.FieldType))
                {
                    res = default!;
                    return false;
                }

                res = (T) field.GetValue(ins);
                return true;
            }

            var property = type.GetRuntimeProperty(name);
            if (property != null)
            {
                if (typeof(T).IsAssignableFrom(property.PropertyType))
                {
                    res = default!;
                    return false;
                }

                res = (T) property.GetValue(ins);
                return true;
            }

            res = default!;
            return false;
        }

        /// <summary>
        ///     留着，纪念自己不踩类似坑；类型与成员类型对不上应该立即抛异常，否则会对使用者产生误导
        /// </summary>
        [Obsolete("see TrySetFieldOrProperty ExceptionVersion", true)]
        public static bool TrySetFieldOrProperty(object ins, string name, object value, Type valuetype)
        {
            if (!valuetype.IsInstanceOfType(value)) return false;
            var type = ins.GetType();
            var field = type.GetRuntimeField(name);
            if (field != null)
            {
                if (!valuetype.IsAssignableFrom(field.FieldType)) return false;
                field.SetValue(ins, value);
                return true;
            }

            var property = type.GetRuntimeProperty(name);
            if (property != null)
            {
                if (valuetype.IsAssignableFrom(property.PropertyType)) return false;
                property.SetValue(ins, value);
                return true;
            }

            return false;
        }

        /// <summary>
        ///     抛异常版
        /// </summary>
        public static bool TrySetFieldOrProperty(object ins, string name, object value)
        {
            var type = ins.GetType();
            var field = type.GetRuntimeField(name);
            if (field != null)
            {
                field.SetValue(ins, value);
                return true;
            }

            var property = type.GetRuntimeProperty(name);
            if (property != null)
            {
                property.SetValue(ins, value);
                return true;
            }

            return false;
        }

        /// <summary>
        ///     给予具有keyword的类型，获得该keyword
        ///     包含基础类型的nullable类型
        /// </summary>
        /// <exception cref="NotSupportedException">非keyword类型</exception>
        public static string GetKeywordTypeKeyword(Type type)
        {
            //按https://docs.microsoft.com/en-gb/dotnet/csharp/language-reference/keywords/顺序
            if (type == typeof(bool))
                return "bool";
            if (type == typeof(byte))
                return "byte";
            if (type == typeof(char))
                return "char";
            if (type == typeof(decimal))
                return "decimal";
            if (type == typeof(double))
                return "double";
            if (type == typeof(float))
                return "float";
            if (type == typeof(int))
                return "int";
            if (type == typeof(long))
                return "long";
            if (type == typeof(sbyte))
                return "sbyte";
            if (type == typeof(short))
                return "short";
            if (type == typeof(string))
                return "string";
            if (type == typeof(uint))
                return "uint";
            if (type == typeof(ulong))
                return "ulong";
            if (type == typeof(ushort))
                return "ushort";

            #region nullable

            if (type == typeof(bool?))
                return "bool?";
            if (type == typeof(byte?))
                return "byte?";
            if (type == typeof(char?))
                return "char?";
            if (type == typeof(decimal?))
                return "decimal?";
            if (type == typeof(double?))
                return "double?";
            if (type == typeof(float?))
                return "float?";
            if (type == typeof(int?))
                return "int?";
            if (type == typeof(long?))
                return "long?";
            if (type == typeof(sbyte?))
                return "sbyte?";
            if (type == typeof(short?))
                return "short?";
            if (type == typeof(uint?))
                return "uint?";
            if (type == typeof(ulong?))
                return "ulong?";
            if (type == typeof(ushort?))
                return "ushort?";

            #endregion

            throw new NotSupportedException($"{type.Name} has no keyword");
        }

        /// <summary>
        ///     <para>0.ins==null,return="null"</para>
        ///     <para>1.返回该对象在Defintion定义的Name</para>
        ///     <para>2.返回该对象"Name"字段值</para>
        ///     <para>3.返回该对象"Name"属性值</para>
        ///     <para>4.返回该对象"string Name()"结果</para>
        ///     <para>5.返回该对象Type.FullName</para>
        ///     此代码是祖传代码，留一下
        /// </summary>
        /// <param name="ins"></param>
        /// <returns></returns>
        public static string GetName(object? ins)
        {
            if (ins == null) return "null";
            //如果已经定义，则跳过反射
            //var res=DefintionSpace.Defintion.GetName(ins);
            //if(res!=null)return res;
            var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;
            var type = ins.GetType();
            var name = "Name";
            var field = type.GetField(name, flags);
            if (field != null) return field.GetValue(ins).ToString();
            var property = type.GetProperty(name, flags);
            if (property != null) return property.GetValue(ins).ToString();
            var method = type.GetMethod(name, flags);
            if (method != null && method.GetParameters().Length == 0)
                return method.Invoke(ins, null)?.ToString() ??
                       throw new NullReferenceException($"missing full type name:{type.Name}");
            return type.FullName ?? throw new NullReferenceException($"missing full type name:{type.Name}");
        }

        /// <summary>
        ///     如果对象有此函数则运行；如果函数对不上参数会抛异常
        /// </summary>
        public static bool TryInvokeMethod(object obj, string methodName, object[] args, out dynamic? res)
        {
            if (methodName.IsNullOrEmpty())
                throw new ArgumentOutOfRangeException(nameof(methodName), "methodName can not be empty");

            var method = obj.GetType().GetMethod(methodName);
            if (method == null)
            {
                res = null;
                return false;
            }

            res = method.Invoke(obj, args);
            return true;
        }
    }
}