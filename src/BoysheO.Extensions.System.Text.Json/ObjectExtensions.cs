using System.Text.Json;

namespace BoysheO.Extensions.System.Text.Json
{
    public static class ObjectExtensions
    {
        public static string ToJsonBySystemTextJson<T>(this T obj, JsonSerializerOptions? options = null)
        {
            return JsonSerializer.Serialize(obj, options);
        }
    }
}