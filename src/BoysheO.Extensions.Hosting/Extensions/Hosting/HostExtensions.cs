using BoysheO.Extensions.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;

namespace BoysheO.Extensions.Hosting
{
    public static class HostExtensions
    {
        public static void LogConfiguration<T>(this IHost host, LogLevel logLevel = LogLevel.Information)
        {
            var conf = host.Services.GetRequiredService<IConfiguration>();
            var str = conf.PrintAllConfigure();
            var logger = host.Services.GetRequiredService<ILogger<T>>();
            logger.Log(logLevel, str);
        }
    }
}