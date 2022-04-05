using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace BoysheO.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// 添加的生命周期由provider管理
        /// 对于collection，是Transient
        /// </summary>
        public static IServiceCollection AddService<T>(this IServiceCollection collection, IServiceProvider provider)
            where T : class
        {
            return collection.AddTransient(serviceProvider => provider.GetRequiredService<T>());
        }

        public static IServiceCollection AddServices(this IServiceCollection collection, IServiceProvider provider,
            IEnumerable<Type> serviceTypes)
        {
            foreach (var type in serviceTypes)
            {
                collection.AddTransient(type, _ => provider.GetRequiredService(type));
            }

            return collection;
        }

        public static IServiceCollection AddServices(this IServiceCollection collection, IServiceProvider provider,
            params Type[] serviceTypes)
        {
            return collection.AddServices(provider, serviceTypes.AsEnumerable());
        }
    }
}