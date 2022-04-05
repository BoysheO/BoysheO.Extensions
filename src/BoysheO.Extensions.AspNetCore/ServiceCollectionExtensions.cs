using System.IO.Compression;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.DependencyInjection;

namespace BoysheO.AspNetCore;

/// <summary>
/// 一些扩展
/// </summary>
public  static partial class ServiceCollectionExtensions
{
    /// <summary>
    ///     构建后需要调用app.UseResponseCompression();
    /// </summary>
    public static IServiceCollection AddGZipAndBrotli(this IServiceCollection services)
    {
        return services
            .Configure<BrotliCompressionProviderOptions>(options => { options.Level = CompressionLevel.Optimal; })
            .Configure<GzipCompressionProviderOptions>(options => { options.Level = CompressionLevel.Optimal; })
            .AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
                options.Providers.Add<BrotliCompressionProvider>();
                options.Providers.Add<GzipCompressionProvider>();
                options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[]
                {
                    "text/html; charset=utf-8",
                    "application/xhtml+xml",
                    "application/atom+xml",
                    "image/svg+xml"
                });
            });
    }
}