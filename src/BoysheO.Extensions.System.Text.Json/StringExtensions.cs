using System;
using System.Text.Json;

namespace BoysheO.Extensions.System.Text.Json
{
    public static class StringExtensions
    {
        public static T? ToObjectBySystemTextJson<T>(this string json, JsonSerializerOptions? options = null)
            where T : class
        {
            return JsonSerializer.Deserialize<T>(json, options);
        }

        /// <summary>
        /// 任何情况不会抛异常,只返回null
        /// </summary>
        public static T? ToObjectBySystemTextJsonWithoutException<T>(this string json, JsonSerializerOptions? options = null)
            where T : class
        {
            try
            {
                return JsonSerializer.Deserialize<T>(json, options);
            }
            catch
            {
                return null;
            }
        }
    }
}