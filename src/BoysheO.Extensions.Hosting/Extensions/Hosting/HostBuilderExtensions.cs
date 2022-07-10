using System;
using System.IO;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BoysheO.Extensions.Hosting
{
    public static class HostBuilderExtensions
    {
        /// <summary>
        ///     注入POCO配置对象
        ///     某些场合下使用
        /// </summary>
        [Obsolete("use ServiceCollection.Config<TOption> instead")]
        public static IHostBuilder AddOptionInfo<T>(this IHostBuilder builder, T model) where T : class
        {
            builder.ConfigureAppConfiguration(configurationBuilder =>
            {
                var json = JsonSerializer.Serialize(model);
                var strBuild = new StringBuilder();
                strBuild.Append(@"{""");
                strBuild.Append(typeof(T).Name);
                strBuild.Append(@""":");
                strBuild.Append(json);
                strBuild.Append("}");
                var bytes = Encoding.UTF8.GetBytes(strBuild.ToString());
                var mem = new MemoryStream(bytes);
                //以下这段代码等义于上文，但是IL2CPP会报错
                // var dic = new Dictionary<string, T>()
                // {
                //     {typeof(T).Name, model}
                // };
                // var bytes = JsonSerializer.SerializeToUtf8Bytes(dic);
                // var mem = new MemoryStream(bytes);
                configurationBuilder.AddJsonStream(mem);
            });
            builder.ConfigureServices((context, collection) =>
            {
                collection.Configure<T>(context.Configuration.GetSection(typeof(T).Name));
            });
            return builder;
        }
    }
}