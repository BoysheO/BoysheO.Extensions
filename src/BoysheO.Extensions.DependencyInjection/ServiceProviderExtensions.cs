using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BoysheO.Extensions.DependencyInjection
{
    public static class ServiceProviderExtensions
    {
        public static bool ContainsServices(this IServiceProvider provider, IEnumerable<Type> serviceTypes)
        {
            return serviceTypes.All(serviceType => provider.GetService(serviceType) != null);
        }

        public static bool ContainsServices(this IServiceProvider provider, params Type[] serviceTypes)
        {
            return ContainsServices(provider, serviceTypes.AsEnumerable());
        }

        public static bool ContainsService(this IServiceProvider provider, Type serviceType)
        {
            return provider.GetService(serviceType) != null;
        }

        public static ILogger<T> GetLogger<T>(this IServiceProvider provider)
        {
            return provider.GetRequiredService<ILogger<T>>();
        }

        public static ILogger GetLogger(this IServiceProvider provider, string categoryName)
        {
            var fac = provider.GetRequiredService<ILoggerFactory>();
            return fac.CreateLogger(categoryName);
        }

        public static ILogger GetLoggerWithFileName(this IServiceProvider provider,
            [CallerFilePath] string sourceFilePath = "missing file"
        )
        {
            var fac = provider.GetRequiredService<ILoggerFactory>();
            var file = Path.GetFileName(sourceFilePath);
            return fac.CreateLogger(file);
        }
    }
}