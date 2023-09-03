#nullable enable
using System;
using System.Collections.Generic;
using System.Reflection;

namespace BoysheO.Extensions.CustomerAttributeMeta
{
    public static class CustomerAttributeMetaExtensions
    {
        private static readonly Dictionary<Type, ICustomerAttributeMeta> _type2data;

        static CustomerAttributeMetaExtensions()
        {
            var baseType = typeof(ICustomerAttributeMeta);
            _type2data = new();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (type.IsClass && baseType != type && baseType.IsAssignableFrom(type) && !type.IsAbstract)
                    {
                        var ins = Activator.CreateInstance(type) as ICustomerAttributeMeta ??
                                  throw new Exception("invalid type");
                        //if a type has IMetaData more than one,pick the last
                        _type2data[ins.Owner] = ins;
                    }
                }
            }
        }

        public static T? GetCustomerAttributeMeta<T>(this Type type, bool inherit)
            where T : Attribute
        {
            return GetCustomerAttributeMeta(type, typeof(T), inherit) as T;
        }

        public static Attribute? GetCustomerAttributeMeta(this Type type, Type attributeType, bool inherit)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (_type2data.TryGetValue(type, out var data))
            {
                if (data.CustomerAttribute != null)
                {
                    if (attributeType.IsInstanceOfType(data.CustomerAttribute)) return data.CustomerAttribute;
                }

                if (data.CustomerAttributes != null)
                {
                    for (int index = 0, size = data.CustomerAttributes.Length; index < size; index++)
                    {
                        var valueCustomerAttribute = data.CustomerAttributes[index];
                        if (attributeType.IsInstanceOfType(valueCustomerAttribute)) return valueCustomerAttribute;
                    }
                }
            }

            if (!inherit) return null;
            if (type.BaseType == null || type.BaseType == typeof(object)) return null;
            return GetCustomerAttributeMeta(type.BaseType, attributeType, inherit);
        }
    }
}