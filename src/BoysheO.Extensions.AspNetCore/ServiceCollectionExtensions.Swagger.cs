using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace BoysheO.AspNetCore;

public static partial class ServiceCollectionExtensions
{
    /// <summary>
    /// 在工厂已经设置输出xml注释的前提下，可使用此API将注释展示到swagger页面上
    /// </summary>
    public static IServiceCollection AddSwaggerWithXmlCommit(this IServiceCollection services)
    {
        return services.AddSwaggerGen(o =>
        {
            // Set the comments path for the Swagger JSON and UI.
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            o.IncludeXmlComments(xmlPath);
        });
    }
}