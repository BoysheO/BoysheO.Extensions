using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace BoysheO.AspNetCore.SwaggerGen;

/// <summary>
/// swagger扩展
/// </summary>
public static class SwaggerGenOptionsExtensions
{
    /// <summary>
    /// 给swagger添加Authorize支持
    /// </summary>
    public static SwaggerGenOptions UseAuthorizeSupport(this SwaggerGenOptions options)
    {
        var securityScheme = new OpenApiSecurityScheme
        {
            Description =
                "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
            Name = "Authorization",
            //参数添加在头部
            In = ParameterLocation.Header,
            //使用Authorize头部
            Type = SecuritySchemeType.Http,
            //内容为以 bearer开头
            Scheme = "bearer",
            BearerFormat = "JWT"
        };
        //把所有方法配置为增加bearer头部信息
        var securityRequirement = new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "bearerAuth"
                    }
                },
                new string[] { }
            }
        };
        //注册到swagger中
        options.AddSecurityDefinition("bearerAuth", securityScheme);
        options.AddSecurityRequirement(securityRequirement);
        return options;
    }

    /// <summary>
    ///     仅debug
    /// </summary>
    public static SwaggerGenOptions UseXmlCommonSupport<T>(this SwaggerGenOptions options)
    {
        var type = typeof(T);
        var assembly = type.Assembly;
        // Set the comments path for the Swagger JSON and UI.
        var xmlFile = $"{assembly.GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        options.IncludeXmlComments(xmlPath);
        return options;
    }
}